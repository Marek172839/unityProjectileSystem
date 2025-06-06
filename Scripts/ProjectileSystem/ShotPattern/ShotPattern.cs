using System;
using System.Collections;
using MD.Gameplay;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MD.ProjectileSystem
{
    public class ShotPattern : MonoBehaviour
    {
        #region Fields

        [Header("===== Common Settings =====")]
        public GameObject bulletPrefab;

        [DisableInEditorMode]
        public Spaceship creatorShip;

        [HideInInspector]
        public ShotPatternObjectPool ProjectilePool;

        private float scriptDelay = 0.05f;

        [Range(0.01f, 10f)]
        public float fireRate = 0.2f;

        public float bulletSpeed = 2f;

        public float bulletAcceleration = 0.1f;

        public bool useMaxSpeed = false;

        [ShowIf("@useMaxSpeed")]
        public float maxSpeed = 0f;

        public bool useMinSpeed = false;

        [ShowIf("@useMinSpeed")]
        public float bulletMinSpeed = 0f;

        public float bulletAccelTurn = 0f;

        #region PAUSE_AND_RESUME_PATTERN

        public bool pauseAndResumePattern = false;

        [ShowIf("@pauseAndResumePattern"), Range(0.01f, 1000f)]
        public float pauseResumePatternWait = 3f;

        [ShowIf("@pauseAndResumePattern"), Range(0.01f, 1000f)]
        public float pauseResumePatternResume = 1f;
        #endregion

        #region PAUSE_AND_RESUME_PROJECTILE
        public bool pauseAndResumeProjectile = false;

        [ShowIf("@pauseAndResumeProjectile"), Range(0.01f, 1000f)]
        public float projectilePauseTime = 0.01f;

        [ShowIf("@pauseAndResumeProjectile"), Range(0.01f, 1000f)]
        public float projectileResumeTime = 0.01f;

        #endregion
        public bool autoDestroy = false;

        [ShowIf("@autoDestroy")]
        public float destroyAfterTime = 10f;

        public bool destroyOnShipColl = true;

        [Header("===== Projectile Settings =====")]
        public bool hitPlayer;

        public bool hitEnemy;

        [Range(0.01f, 100)]
        public float damageMultiplier = 1f;

        public bool IsShooting = false;
        private float nextFireTime = 0.0f;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            ProjectilePool = GetComponent<ShotPatternObjectPool>();
            creatorShip = GetComponentInParent<Spaceship>();

            if (creatorShip != null)
                creatorShip.ShipStats.OnStatsChanged += UpdateReloadSpeed;
        }

        protected virtual void Start()
        {
            StartCoroutine(BeginAfterDelay());
        }

        protected virtual void FixedUpdate()
        {
            TryShoot();
        }

        #endregion

        public virtual void Init()
        {
            UpdateReloadSpeed();
        }

        private IEnumerator BeginAfterDelay()
        {
            yield return new WaitForSeconds(scriptDelay);
            Init();
            if (pauseAndResumePattern)
            {
                StartCoroutine(PauseResumePatternRoutine());
            }
        }

        private void TryShoot()
        {
            if (IsShooting && Time.time >= nextFireTime)
            {
                CreateProjectile();
                nextFireTime = Time.time + fireRate;
            }
        }

        private IEnumerator PauseResumePatternRoutine()
        {
            WaitForSeconds pauseTime = new(pauseResumePatternWait);
            WaitForSeconds resumeTime = new(pauseResumePatternResume);

            while (true)
            {
                IsShooting = false;
                yield return pauseTime;

                IsShooting = true;
                yield return resumeTime;
            }
        }

        protected virtual void CreateProjectile()
        {
            var angle = transform.rotation.eulerAngles.z;
            var projectile = ProjectilePool.GetFromPool();
            projectile.GetComponent<ProjectileController>().Init(this, angle);
            projectile.GetComponent<Projectile>().Init(creatorShip, this);
        }

        private void UpdateReloadSpeed()
        {
            if (creatorShip != null)
            {
                // update reload speed of the ship
                fireRate = Mathf.Clamp(
                    fireRate * creatorShip.ShipStats.GetStatValue("stat_Reload"),
                    0.01f,
                    float.MaxValue
                );
            }
        }

        private void OnDestroy()
        {
            if (creatorShip != null)
                creatorShip.ShipStats.OnStatsChanged -= UpdateReloadSpeed;
        }

        #region Debugging && Editor

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // Draw blac arrow indicating the direction of the shot pattern
            Quaternion baseRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
            float adjustedZRotation = 270f - transform.rotation.eulerAngles.z;
            Quaternion adjustedRotation = baseRotation * Quaternion.Euler(adjustedZRotation, 0, 0);
            Handles.color = Color.black;
            Handles.ArrowHandleCap(
                0,
                transform.position,
                adjustedRotation,
                0.33f,
                EventType.Repaint
            );
        }

        [Button(Style = ButtonStyle.Box)]
        private void ApplyChanges()
        {
            StopAllCoroutines();
            Start();
        }
#endif
        # endregion
    }
}
