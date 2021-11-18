using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class WashTrackAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private Image animationTarget;

        private bool _isRunning;
        private float _singleFrameDuration;
        private float _currentFrameTimer;
        private int _currentFrame;

        public void OuterStart()
        {
            _isRunning = true;
            _singleFrameDuration = 1f / 60;
        }

        public void OuterStop()
        {
            _isRunning = false;
        }

        public void Update()
        {
            if (!_isRunning)
                return;
            
            _currentFrameTimer += Time.deltaTime;

            if (_currentFrameTimer > _singleFrameDuration)
            {
                int frameDelta = Mathf.FloorToInt(_currentFrameTimer / _singleFrameDuration);
                _currentFrame += frameDelta;
                _currentFrame %= animationFrames.Length;
                _currentFrameTimer = 0;
                
                animationTarget.sprite = animationFrames[_currentFrame];
            }
        }
    }
}