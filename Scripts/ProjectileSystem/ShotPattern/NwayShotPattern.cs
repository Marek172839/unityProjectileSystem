using Sirenix.OdinInspector;
using UnityEngine;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Spawns a burst of projectiles arranged either in evenly spaced angles (pizza-shape)
    /// or in parallel lines separated by <see cref="betweenDistance"/>.
    /// </summary>
    [RequireComponent(typeof(ShotPatternObjectPool))]
    public class NwayShotPattern : ShotPattern
    {
        #region Fields

        [Header("===== N-Way Shot Settings =====")]
        [Tooltip("If true, pizza shape. If false, line.")]
        public bool useAngleBetweenShots;

        [Tooltip("Number of projectiles in the burst.")]
        [Range(1, 25)]
        public int wayNum = 3;

        // Angle mode
        [ShowIf("@useAngleBetweenShots"), Range(0f, 360f)]
        [Tooltip("Central direction of the fan (deg).")]
        public float centerAngle = 100f;

        [ShowIf("@useAngleBetweenShots"), Range(0f, 360f)]
        [Tooltip("Angle between neighbouring shots (deg).")]
        public float betweenAngle = 100f;

        // Distance mode
        [ShowIf("@!useAngleBetweenShots"), Range(0f, 1f)]
        [Tooltip("Lateral spacing between neighbouring shots (world units).")]
        public float betweenDistance = 0.15f;

        # endregion

        protected override void CreateProjectile()
        {
            if (useAngleBetweenShots)
                CreateAngledProjectiles();
            else
                CreateParallelProjectiles();
        }

        /// <summary>
        /// Spawns a fan of projectiles whose directions are separated by <see cref="betweenAngle"/>.
        /// </summary>
        private void CreateAngledProjectiles()
        {
            // For even counts, shift the base so the fan is centred on <see cref="centerAngle"/>.
            float baseAngle = wayNum % 2 == 0 ? centerAngle - betweenAngle / 2f : centerAngle;
            float shipRotationZ = transform.rotation.eulerAngles.z;

            for (int i = 0; i < wayNum; i++)
            {
                float localAngle = Utils.GetShiftedAngle(i, baseAngle, betweenAngle);
                float finalAngleZ = shipRotationZ + localAngle;

                GameObject proj = ProjectilePool.GetFromPool();
                proj.GetComponent<ProjectileController>().Init(this, finalAngleZ);
                proj.GetComponent<Projectile>().Init(creatorShip, this);
                Utils.SetEulerAnglesZ(proj.transform, finalAngleZ);
            }
        }

        /// <summary>
        /// Spawns projectiles in parallel, offset laterally by <see cref="betweenDistance"/>.
        /// </summary>
        private void CreateParallelProjectiles()
        {
            float finalAngleZ = transform.rotation.eulerAngles.z;
            float halfSpread = (wayNum - 1) * betweenDistance * 0.5f;

            for (int i = 0; i < wayNum; i++)
            {
                GameObject proj = ProjectilePool.GetFromPool();

                // Offset spawn position along local right vector.
                Vector3 right = proj.transform.right;
                Vector3 offset = right * (i * betweenDistance - halfSpread);
                Vector3 spawnPos = transform.position + offset;

                proj.GetComponent<ProjectileController>().Init(this, finalAngleZ, spawnPos);
                proj.GetComponent<Projectile>().Init(creatorShip, this);
                Utils.SetEulerAnglesZ(proj.transform, finalAngleZ);
            }
        }
    }
}
