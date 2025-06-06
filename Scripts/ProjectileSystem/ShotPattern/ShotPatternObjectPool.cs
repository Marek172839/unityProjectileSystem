using System;
using System.Collections.Generic;
using UnityEngine;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// Object pool design pattern for projectiles
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(ShotPattern))]
    public class ShotPatternObjectPool : MonoBehaviour
    {
        #region Fields

        public GameObject PrjPrefab { get; set; }
        private readonly Stack<GameObject> prjPool = new();
        private Transform prjParent;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            PrjPrefab = GetComponent<ShotPattern>().bulletPrefab;

            // Ensure a common parent GameObject exists for organisational clarity.
            GameObject container = GameObject.FindWithTag("ProjectileParent");
            if (container == null)
                container = new GameObject("ProjectileParent") { tag = "ProjectileParent" };
            prjParent = container.transform;
        }

        private void OnDestroy()
        {
            // Clean up pooled projectiles to avoid orphaned objects.
            foreach (GameObject go in prjPool)
            {
                if (!go)
                    continue;

                if (!go.activeInHierarchy)
                {
                    Destroy(go);
                }
                else
                {
                    // Detach from pool ownership to avoid double-destroy via projectile logic.
                    go.GetComponent<Projectile>().ProjectilePool = null;
                }
            }
            prjPool.Clear();
        }

        #endregion

        /// <summary>
        /// Adds a projectile back into the pool.
        /// </summary>
        public void AddToPool(GameObject projectileGo)
        {
            prjPool.Push(projectileGo);
        }

        /// <summary>
        /// Returns a projectile from the pool or instantiates a new one if empty.
        /// </summary>
        public GameObject GetFromPool()
        {
            if (prjPool.Count > 0)
                return prjPool.Pop();

            return Instantiate(PrjPrefab, transform.position, transform.rotation, prjParent);
        }
    }
}
