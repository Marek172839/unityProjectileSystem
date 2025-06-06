using System;
using UnityEngine;

// !TODO Stump class
namespace MD.Gameplay
{
    public class ShipStats : MonoBehaviour
    {
        public event Action OnStatsChanged;

        public float GetStatValue(string v)
        {
            return 1f;
        }
    }
}
