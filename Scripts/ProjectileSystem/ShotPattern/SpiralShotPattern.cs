using UnityEngine;

namespace MD.ProjectileSystem
{
    public class SpiralShotPattern : NwayShotPattern
    {
        [Header("===== SpiralShot Settings =====")]
        [Tooltip("Maximum rotation angle")]
        [SerializeField]
        private float amplitude = 30f;

        [Tooltip("How fast it rotates")]
        [SerializeField]
        private float frequency = 1f;
        private float originalRotationZ = 0f;

        protected override void Awake()
        {
            base.Awake();
            originalRotationZ = transform.rotation.eulerAngles.z; // set the initial rotation angle
        }

        protected override void FixedUpdate()
        {
            // Calculate the new rotation angle based on a sinusoidal wave
            float angle = originalRotationZ + amplitude * Mathf.Sin(Time.time * frequency);
            transform.rotation = Quaternion.Euler(0, 0, angle); // Apply the rotation around the Z-axis
            base.FixedUpdate();
        }
    }
}
