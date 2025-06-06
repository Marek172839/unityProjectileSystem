using System;
using MD.Gameplay;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector;
#endif

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Projectile that can damage ships and is part of Object Pooling system.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(ProjectileController))]
    public class Projectile : MonoBehaviour
    {
        #region Fields

        [SerializeField, DisableInEditorMode]
        private Spaceship creator;

        [SerializeField, DisableInEditorMode]
        private long damage;

        [SerializeField, DisableInEditorMode]
        private long critDamage;

        [SerializeField, DisableInEditorMode]
        private float critDamageChance;

        [SerializeField, DisableInEditorMode]
        private bool destroyOnShipColl = true;

        private bool hitPlayer;
        private bool hitEnemy;
        private ShotPatternObjectPool projectilePool;
        private ShotPattern shotPattern;

        private Rigidbody rb;
        private Collider col;
        private ProjectileController projectileController;

        #endregion

        #region Properties

        public long Damage
        {
            get => damage;
            protected set => damage = value;
        }

        public Spaceship Creator
        {
            get => creator;
            set => creator = value;
        }

        public bool HitPlayer
        {
            get => hitPlayer;
            set => hitPlayer = value;
        }

        public bool HitEnemy
        {
            get => hitEnemy;
            set => hitEnemy = value;
        }

        public float CritDamageChance
        {
            get => critDamageChance;
            set => critDamageChance = value;
        }

        public long CritDamage
        {
            get => critDamage;
            set => critDamage = value;
        }

        public ShotPatternObjectPool ProjectilePool
        {
            get => projectilePool;
            set => projectilePool = value;
        }

        public ShotPattern ShotPattern
        {
            get => shotPattern;
            set => shotPattern = value;
        }

        #endregion

        #region Events

        public event Action OnPoolEnable;
        public event Action OnPoolDisable;
        public event Action OnDestroy;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            projectileController = GetComponent<ProjectileController>();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Initializes the projectile with data from its creator and shot pattern.
        /// </summary>
        /// <param name="creator">The spaceship that fired the projectile.</param>
        /// <param name="shotPattern">The pattern defining the projectile's behaviour.</param>
        public void Init(Spaceship creator, ShotPattern shotPattern)
        {
            this.shotPattern = shotPattern;
            Creator = creator;
            HitPlayer = shotPattern.hitPlayer;
            HitEnemy = shotPattern.hitEnemy;
            ProjectilePool = shotPattern.ProjectilePool;
            destroyOnShipColl = shotPattern.destroyOnShipColl;

            if (Creator != null)
            {
                Damage = (long)(
                    Creator.ShipStats.GetStatValue("stat_Damage") * shotPattern.damageMultiplier
                );
                CritDamageChance = (float)Creator.ShipStats.GetStatValue("stat_CritChance");
                CritDamage = (long)(Damage * CritDamageChance);
            }
            else
            {
                Debug.LogWarning("Projectile creator is null");
            }

            PoolEnable();
        }

        /// <summary>
        /// Applies ship‐collision rules (destroying the projectile if configured to do so).
        /// </summary>
        public void HandleShipCollision()
        {
            if (destroyOnShipColl)
            {
                Destroy();
            }
        }

        /// <summary>
        /// Destroys or pools the projectile depending on whether it has a valid pool assigned.
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy?.Invoke();

            if (ProjectilePool == null)
                Destroy(gameObject);
            else
                PoolDisable();
        }

        #endregion

        #region Pooling

        /// <summary>
        /// Activates the projectile after being spawned or reused from the pool.
        /// </summary>
        public void PoolEnable()
        {
            rb.isKinematic = false;
            col.enabled = true;
            projectileController.enabled = true;

            OnPoolEnable?.Invoke();
        }

        /// <summary>
        /// Deactivates the projectile and returns it to its pool.
        /// </summary>
        public void PoolDisable()
        {
            ProjectilePool.AddToPool(gameObject);

            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            col.enabled = false;
            projectileController.enabled = false;

            OnPoolDisable?.Invoke();
        }

        #endregion

        #region Debugging && Editor

#if UNITY_EDITOR

        [Title("Debug"), Button]
        private void ToggleRenderer(bool enable)
        {
            if (!TryGetComponent<Renderer>(out var renderer))
            {
                Debug.LogError("Renderer component not found");
                return;
            }

            renderer.enabled = enable;
        }

#endif
        #endregion
    }
}
