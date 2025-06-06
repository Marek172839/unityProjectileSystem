using UnityEngine;

namespace MD.ProjectileSystem
{
    public class TargetPlayerPattern : NwayShotPattern
    {
        [Header("===== Target Player Settings =====")]
        [SerializeField]
        private float rotationSpeed = 1f;
        private Transform playerTransform;
        private Vector3 targetDirection;
        private float angle;
        private Quaternion targetRotation;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            FindPlayer();
        }

        private void FindPlayer()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null)
            {
                Debug.LogError("Player not found!");
                return;
            }
            playerTransform = playerGo.transform;
        }

        private void Update()
        {
            RotateToPlayer();
        }

        private void RotateToPlayer()
        {
            if (playerTransform != null)
            {
                targetDirection = playerTransform.position - transform.position; // Calculate the direction to the player
                angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f; // Calculate the angle in degrees
                targetRotation = Quaternion.AngleAxis(angle, Vector3.forward); // Rotate around the z-axis
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }
}
