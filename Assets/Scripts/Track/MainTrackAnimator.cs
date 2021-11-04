using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class MainTrackAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private Image animationTarget;

        [SerializeField] private float MagicSpeedTranslationCoefficient;

        private bool _isRunning;
        private float _singleFrameDuration;
        private float _currentFrameTimer;
        private int _currentFrame;

        private float _speed;

        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
            _isRunning = _speed > 0;
            float animationDuration = 1f / 60 * animationFrames.Length;
            _singleFrameDuration = animationDuration * MagicSpeedTranslationCoefficient / (animationFrames.Length * _speed);
        }

        public void Update()
        {
            if (!_isRunning)
                return;
            
            _currentFrameTimer += Time.deltaTime;
            float animationDuration = 1f / 60 * animationFrames.Length;
            _singleFrameDuration = animationDuration * MagicSpeedTranslationCoefficient / (animationFrames.Length * _speed);

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
