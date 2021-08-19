using UnityEngine;

namespace Sneakers
{
    public class BadSortingInfo
    {
        private readonly SneakerController _sneaker;
        private readonly Vector3 _trackPosition;

        public SneakerController Sneaker => _sneaker;
        public Vector3 TrackPosition => _trackPosition;

        public BadSortingInfo(SneakerController sneaker, Vector3 trackPosition)
        {
            _sneaker = sneaker;
            _trackPosition = trackPosition;
        }
    }
}