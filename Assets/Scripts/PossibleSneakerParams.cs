using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public struct PossibleSneakerParams
    {
        [SerializeField] private SneakerConfig config;
        [SerializeField] private SneakerState state;
        [SerializeField] private int count;

        public SneakerConfig Config => config;
        public SneakerState State => state;
        public int Count => count;
    }
}