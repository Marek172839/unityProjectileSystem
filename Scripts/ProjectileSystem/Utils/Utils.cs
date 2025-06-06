using UnityEngine;
using Random = UnityEngine.Random;

namespace MD.ProjectileSystem
{
    public static class Utils
    {
        /// <summary>
        /// Returns a target <see cref="Transform"/> that matches the given tag.
        /// </summary>
        /// <param name="tagName">Tag to search for.</param>
        /// <param name="randomSelect">If true, pick a random target from all matches.</param>
        /// <param name="nearestSelect">If true, pick the nearest target from all matches.</param>
        /// <param name="selfTrans">Transform of the caller – used for distance checks.</param>
        /// <returns>The selected Transform or <c>null</c> if none found.</returns>
        public static Transform GetTransformFromTagName(
            string tagName,
            bool randomSelect,
            bool nearestSelect,
            Transform selfTrans
        )
        {
            if (string.IsNullOrEmpty(tagName))
                return null;

            // Neither random nor nearest – simply return first-found object.
            if (!randomSelect && !nearestSelect)
            {
                GameObject single = GameObject.FindWithTag(tagName);
                return single != null ? single.transform : null;
            }

            // Collect all candidates.
            GameObject[] candidates = GameObject.FindGameObjectsWithTag(tagName);
            if (candidates == null || candidates.Length == 0)
                return null;

            // Random selection.
            if (randomSelect)
            {
                return candidates[Random.Range(0, candidates.Length)].transform;
            }

            // Nearest selection.
            Vector3 selfPos = selfTrans.position;
            float bestDist = float.MaxValue;
            Transform nearest = null;

            foreach (GameObject go in candidates)
            {
                float dist = (go.transform.position - selfPos).sqrMagnitude;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = go.transform;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Get angle from two transforms position.
        /// </summary>
        public static float GetAngleFromTwoPosition(Transform fromTrans, Transform toTrans)
        {
            if (fromTrans == null || toTrans == null)
                return 0f;

            float xDistance = toTrans.position.x - fromTrans.position.x;
            float yDistance = toTrans.position.y - fromTrans.position.y;
            float angle = (Mathf.Atan2(yDistance, xDistance) * Mathf.Rad2Deg) - 90f;
            angle = NormalizeAngle(angle);

            return angle;
        }

        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0)
                angle += 360f;
            return angle;
        }

        public static void SetEulerAnglesZ(Transform self, float z)
        {
            Vector3 selfAngles = self.eulerAngles;
            self.rotation = Quaternion.Euler(selfAngles.x, selfAngles.y, z);
        }

        public static float GetShiftedAngle(int wayIndex, float baseAngle, float betweenAngle)
        {
            float angle =
                wayIndex % 2 == 0
                    ? baseAngle - (betweenAngle * wayIndex / 2f)
                    : baseAngle + (betweenAngle * Mathf.Ceil(wayIndex / 2f));
            return angle;
        }
    }
}
