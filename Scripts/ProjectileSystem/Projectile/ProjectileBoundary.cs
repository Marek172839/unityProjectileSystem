using UnityEngine;

namespace MD.ProjectileSystem
{
    /// <summary>
    /// When a projectile exits this trigger collider, it is destroyed or pooled.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ProjectileBoundary : MonoBehaviour
    {
        private const string PROJECTILE_TAG = "Projectile";

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(PROJECTILE_TAG))
                return;

            if (other.TryGetComponent(out Projectile projectile))
                projectile.Destroy();
        }
    }
}
