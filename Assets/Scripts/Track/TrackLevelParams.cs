using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public struct TrackLevelParams
    {
        [SerializeField] private int level;
        [SerializeField] private int price;
        [SerializeField] private float processDuration;
        [SerializeField] private float trackSpeed;

        public int Level => level;
        public int Price => price;
        public float ProcessDuration => processDuration;
        public float TrackSpeed => trackSpeed;
    }
}