using UnityEngine;

namespace MD.ProjectileSystem
{
    public class VFXProjectile : MonoBehaviour
    {
        public ParticleSystem projectile;
        public ParticleSystem hit;
        public ParticleSystem flash;
        public ParticleSystem sparks;
        public TrailRenderer trail;

        private void Awake()
        {
            GetComponentInParent<Projectile>().OnPoolEnable += OnPrjEnable;
            GetComponentInParent<Projectile>().OnPoolDisable += OnPrjDisable;
        }

        private void OnPrjEnable()
        {
            ToggleProjectileVfx(true);
            ToggleVfx(flash, true);
            ToggleVfx(sparks, true);
            ToggleVfx(hit, false);
            ToggleTrailVfx(true);
        }

        private void OnPrjDisable()
        {
            ToggleProjectileVfx(false);
            ToggleVfx(flash, false);
            ToggleVfx(sparks, false);
            ToggleVfx(hit, true);
            ToggleTrailVfx(false);
        }

        private void ToggleVfx(ParticleSystem ps, bool value)
        {
            if (ps == null)
                return;

            if (value)
                ps.Play();
            else
                ps.Stop();
        }

        private void ToggleProjectileVfx(bool value)
        {
            if (projectile == null)
                return;
            if (value)
            {
                projectile.Play();
            }
            else
            {
                projectile.Clear();
                projectile.Stop();
            }
        }

        private void ToggleTrailVfx(bool value)
        {
            if (trail == null)
                return;

            if (value)
            {
                trail.emitting = true;
                trail.enabled = true;
            }
            else
            {
                trail.Clear();
                trail.emitting = false;
                trail.enabled = false;
            }
        }
    }
}
