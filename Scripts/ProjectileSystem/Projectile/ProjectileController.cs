using Sirenix.OdinInspector;
using UnityEngine;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Handles movement behaviour for a projectile (straight, homing, sine‑wave, acceleration, etc.).
    /// </summary>
    [RequireComponent(typeof(Projectile))]
    public class ProjectileController : MonoBehaviour
    {
        #region Fields

        private Projectile projectile;
        private Transform transformCache;

        // Base movement
        private float speed;
        private float angle;

        // Acceleration
        private float accelSpeed;
        private float accelTurn;

        // Homing
        private bool homing;
        private Transform homingTarget;
        private float homingAngleSpeed;

        // Sin‑wave
        private bool sinWave;
        private float sinWaveSpeed;
        private float sinWaveRangeSize;
        private bool sinWaveInverse;

        // Pause / Resume
        private bool pauseAndResume;
        private float pauseTime;
        private float resumeTime;

        // Auto‑destroy
        private bool autoDestroy;
        private float destroyAfterTime;

        // Limits
        private bool useMaxSpeed;
        private float maxSpeed;
        private bool useMinSpeed;
        private float minSpeed;

        // Internal state
        private float baseAngle;
        private float selfFrameCnt;
        private float selfTimeCount;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            projectile = GetComponent<Projectile>();
            transformCache = transform;
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion

        #region Public API

        public void Init(ShotPattern shotPattern)
        {
            // Reset timers
            selfTimeCount = 0f;
            selfFrameCnt = 0f;

            // Position at pattern origin
            transformCache.position = shotPattern.transform.position;

            // Basic
            speed = shotPattern.bulletSpeed;
            accelSpeed = shotPattern.bulletAcceleration;
            accelTurn = shotPattern.bulletAccelTurn;

            // Pause/Resume
            pauseAndResume = shotPattern.pauseAndResumeProjectile;
            pauseTime = shotPattern.projectilePauseTime;
            resumeTime = shotPattern.projectileResumeTime;

            // Auto‑destroy
            autoDestroy = shotPattern.autoDestroy;
            destroyAfterTime = shotPattern.destroyAfterTime;
        }

        public void Init(ShotPattern pattern, float angle)
        {
            Init(pattern);
            this.angle = angle;
        }

        public void Init(ShotPattern pattern, float angle, Vector3 position)
        {
            Init(pattern, angle);
            transformCache.position = position;
        }

        public void Init(ShotPattern pattern, float speed, float angle)
        {
            Init(pattern, angle);
            this.speed = speed;
        }

        public void InitHoming(HomingShotPattern pattern, float angle)
        {
            Init(pattern, angle);
            homing = true;
            homingTarget = pattern.m_targetTransform;
            homingAngleSpeed = pattern.m_homingAngleSpeed;
        }

        #endregion

        #region Movement Logic

        private void Move()
        {
            selfTimeCount += Time.fixedDeltaTime;

            if (HandleAutoDestroy())
                return;
            if (HandlePauseAndResume())
                return;

            Quaternion currentRotation = transformCache.rotation;
            Vector3 currentAngles = currentRotation.eulerAngles;
            Quaternion newRotation;

            if (homing)
                newRotation = HomingMove(currentAngles, currentRotation);
            else if (sinWave)
                newRotation = SinWaveMove(currentAngles, currentRotation);
            else
                newRotation = Rotate(currentAngles);

            ApplyAcceleration();
            ClampSpeed();

            var newPosition =
                transformCache.position + transformCache.up * (speed * Time.fixedDeltaTime);
            transformCache.SetPositionAndRotation(newPosition, newRotation);
        }

        private bool HandleAutoDestroy()
        {
            if (!autoDestroy)
                return false;

            if (destroyAfterTime <= 0f)
            {
                projectile.Destroy();
                return true;
            }

            destroyAfterTime -= Time.fixedDeltaTime;
            return false;
        }

        private bool HandlePauseAndResume()
        {
            if (!pauseAndResume)
                return false;
            return selfTimeCount >= pauseTime && selfTimeCount < resumeTime;
        }

        private void ApplyAcceleration() => speed += accelSpeed * Time.fixedDeltaTime;

        private void ClampSpeed()
        {
            if (useMaxSpeed && speed > maxSpeed)
                speed = maxSpeed;
            if (useMinSpeed && speed < minSpeed)
                speed = minSpeed;
        }

        private Quaternion Rotate(Vector3 myAngles)
        {
            float addAngle = accelTurn * Time.fixedDeltaTime;
            return Quaternion.Euler(myAngles.x, myAngles.y, myAngles.z + addAngle);
        }

        private Quaternion SinWaveMove(Vector3 myAngles, Quaternion newRot)
        {
            angle += accelTurn * Time.fixedDeltaTime;

            if (sinWaveSpeed > 0f && sinWaveRangeSize > 0f)
            {
                float waveAngle =
                    angle
                    + sinWaveRangeSize
                        / 2f
                        * Mathf.Sin(selfFrameCnt * sinWaveSpeed / 100f)
                        * (sinWaveInverse ? -1f : 1f);

                newRot = Quaternion.Euler(myAngles.x, myAngles.y, baseAngle + waveAngle);
            }

            selfFrameCnt += Time.fixedDeltaTime;
            return newRot;
        }

        private Quaternion HomingMove(Vector3 myAngles, Quaternion newRot)
        {
            if (homingTarget == null || homingAngleSpeed <= 0f)
                return newRot;

            float targetAngle = Utils.GetAngleFromTwoPosition(transformCache, homingTarget);
            float myAngle = myAngles.z;
            float toAngle = Mathf.MoveTowardsAngle(
                myAngle,
                targetAngle,
                Time.fixedDeltaTime * homingAngleSpeed
            );

            return Quaternion.Euler(myAngles.x, myAngles.y, toAngle);
        }

        #endregion
    }
}
