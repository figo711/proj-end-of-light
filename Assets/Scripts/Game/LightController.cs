using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Light tileLight;
        [SerializeField] private float lightOnDuration; // 4f
        [SerializeField] private float lightOffDuration; // 2f

        private float _timer;
        private bool _isTurningOff;
        private bool _isLit;

        public bool IsLit => _isLit;
        public float LightOnDuration
        {
            get => lightOnDuration;
            set => lightOnDuration = value;
        }
        public float LightOffDuration
        {
            get => lightOffDuration;
            set => lightOffDuration = value;
        }

        private void Start()
        {
            _timer = Random.Range(0f, lightOnDuration);
            _isLit = true;
            _isTurningOff = false;
        }

        private void Update()
        {
            if (_isTurningOff) return;

            _timer -= Time.deltaTime;

            if (_isLit && _timer <= 0)
            {
                // s'éteint
                SetLight(false);
                _timer = lightOffDuration + Random.Range(-0.5f, 0.5f);
            }
            else if (!_isLit && _timer <= 0)
            {
                // se rallume
                SetLight(true);
                _timer = lightOnDuration + Random.Range(-0.5f, 0.5f);
            }
        }

        private void SetLight(bool state)
        {
            _isTurningOff = true;
            var finalValue = state ? 40f : 5f;
            tileLight.DOIntensity(finalValue, 3f).OnComplete(() =>
            {
                _isTurningOff = false;
                _isLit = state;
            });
        }
    }
}