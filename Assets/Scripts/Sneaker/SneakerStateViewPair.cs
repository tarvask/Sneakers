using System;
using UnityEngine;

namespace Sneakers
{
    [Serializable]
    public struct SneakerStateViewPair
    {
        [SerializeField] private SneakerState state;
        [SerializeField] private GameObject view;

        public SneakerState State => state;
        public GameObject View => view;
    }
}