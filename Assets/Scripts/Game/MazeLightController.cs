using UnityEngine;

namespace Game
{
    public class MazeLightController : MonoBehaviour
    {
        [SerializeField] private float _speed; // 1f
        [SerializeField] private float _scale; // 1f
        [SerializeField] private float _intensityMin; // 0f
        [SerializeField] private float _intensityMax; // 1f
        [SerializeField] private float _transitionSpeed; // 2f

        private Light[] _lights;
        private float[] _noiseOffsets;

        public void InitializeLights(Light[] lights)
        {
            _lights = lights;


            _noiseOffsets = new float[lights.Length];
            for (int i = 0; i < lights.Length; i++)
                _noiseOffsets[i] = Random.Range(0f, 100f);
        }

        private void Update()
        {
            if (_lights == null) return;

            float time = Time.time * _speed;

            for (int i = 0; i < _lights.Length; i++)
            {
                Light l = _lights[i];
                if (l == null) continue;
                float noiseValue = Mathf.PerlinNoise(_noiseOffsets[i], time * _scale);
                float targetIntensity = Mathf.Lerp(_intensityMin, _intensityMax, noiseValue);
                l.intensity = Mathf.Lerp(l.intensity, targetIntensity, Time.deltaTime * _transitionSpeed);
            }
        }
    }
}
