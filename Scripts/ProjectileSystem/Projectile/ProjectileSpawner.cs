using UnityEngine;

namespace MD.ProjectileSystem
{
    [RequireComponent(typeof(Projectile))]
    [RequireComponent(typeof(ShotPattern))]
    public class ProjectileSpawner : MonoBehaviour
    {
        private GameObject spawnPrefab;
        private ShotPattern shotPattern;
        private Projectile projectile;

        private void Awake()
        {
            shotPattern = GetComponent<ShotPattern>();
            projectile = GetComponent<Projectile>();
            spawnPrefab = shotPattern.bulletPrefab;
            projectile.OnDestroy += Spawn;
        }

        private void Spawn()
        {
            // Get parent of the spawned projectile
            var parent = GameObject.Find("ProjectileContent");
            if (parent == null)
                parent = new GameObject("ProjectileContent");

            // Spawn the projectile
            var spawned = Instantiate(
                spawnPrefab,
                transform.position,
                transform.rotation,
                parent.transform
            );

            // Set the creator of the spawned projectile
            if (spawned.TryGetComponent(out Projectile prj))
                prj.Init(projectile.Creator, shotPattern);
        }

        private void OnDestroy()
        {
            projectile.OnDestroy -= Spawn;
        }
    }
}
