using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Fires projectiles with randomised angle, speed and delay.
    /// Can optionally distribute angles evenly across quadrants for more uniform spreads.
    /// </summary>
    public class RandomShotPattern : ShotPattern
    {
        #region Fields

        [Header("===== Random Shot Settings =====")]
        [Tooltip("Central direction of the random range (deg).")]
        [Range(-360f, 360f)]
        public float m_randomCenterAngle = 180f;

        [Tooltip("Total size of the random angle range (deg).")]
        [Range(0f, 360f)]
        public float m_randomRangeSize = 360f;

        [Tooltip("Minimum projectile speed.")]
        public float m_randomSpeedMin = 1f;

        [Tooltip("Maximum projectile speed.")]
        public float m_randomSpeedMax = 3f;

        [Tooltip("Minimum delay between bullets (sec). — NOT handled by this script directly.")]
        public float m_randomDelayMin = 0.01f;

        [Tooltip("Maximum delay between bullets (sec). — NOT handled by this script directly.")]
        public float m_randomDelayMax = 0.1f;

        [Tooltip("If true, angles are evenly distributed across quarters of the range.")]
        public bool m_evenlyDistribute = true;

        [Tooltip("Number of bullets considered when distributing angles.")]
        public int m_bulletNum = 10;

        private readonly List<int> m_numList = new();

        private float MinAngle => m_randomCenterAngle - m_randomRangeSize * 0.5f;
        private float MaxAngle => m_randomCenterAngle + m_randomRangeSize * 0.5f;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Pre-populate an index list used for even distribution.
            m_numList.Capacity = m_bulletNum;
            for (int i = 0; i < m_bulletNum; i++)
            {
                m_numList.Add(i);
            }
        }

        #endregion

        protected override void CreateProjectile()
        {
            // Select a random index for distribution calculations.
            int index = Random.Range(0, m_numList.Count);

            // Speed.
            float speed = Random.Range(m_randomSpeedMin, m_randomSpeedMax);

            // Angle.
            float angleDeg = m_evenlyDistribute
                ? GetEvenlyDistributedAngle(index)
                : Random.Range(MinAngle, MaxAngle);

            // Spawn & initialise projectile.
            GameObject proj = ProjectilePool.GetFromPool();
            proj.GetComponent<ProjectileController>().Init(this, speed, angleDeg);
            proj.GetComponent<Projectile>().Init(creatorShip, this);
            Utils.SetEulerAnglesZ(proj.transform, angleDeg);
        }

        private float GetEvenlyDistributedAngle(int listIndex)
        {
            float oneDirCount = m_bulletNum >= 4 ? Mathf.Floor(m_bulletNum / 4f) : 1f;
            float quarterIndex = Mathf.Floor(listIndex / oneDirCount);
            float quarterSize = Mathf.Abs(MaxAngle - MinAngle) * 0.25f;

            return Random.Range(
                MinAngle + quarterSize * quarterIndex,
                MinAngle + quarterSize * (quarterIndex + 1f)
            );
        }
    }
}
