using UnityEngine;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Spawns projectiles with homing behaviour.
    /// Target selection can be first-found, random, or nearest by distance.
    /// </summary>
    public class HomingShotPattern : ShotPattern
    {
        #region Fields

        [Header("===== HomingShot Settings =====")]
        [Tooltip("Speed at which the projectile rotates towards its target (deg/s).")]
        public float m_homingAngleSpeed = 20f;

        [Tooltip("Tag of the objects this projectile can home onto.")]
        public string targetTag = "Enemy";

        [Tooltip("Select a random target among all objects with the tag.")]
        public bool m_randomSelectTagTarget;

        [Tooltip("Select the nearest target among all objects with the tag.")]
        public bool m_nearestSelectTagTarget;

        [HideInInspector]
        public Transform m_targetTransform;

        #endregion

        protected override void CreateProjectile()
        {
            float finalAngle = transform.rotation.eulerAngles.z;

            // Get homing target
            m_targetTransform = Utils.GetTransformFromTagName(
                targetTag,
                m_randomSelectTagTarget,
                m_nearestSelectTagTarget,
                transform
            );

            GameObject projectile = ProjectilePool.GetFromPool();

            // Initialise behaviour
            projectile.GetComponent<ProjectileController>().InitHoming(this, finalAngle);
            projectile.GetComponent<Projectile>().Init(creatorShip, this);

            // Align the projectile's visual rotation.
            projectile.transform.eulerAngles = finalAngle * Vector3.forward;
        }
    }
}
