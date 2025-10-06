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

        public void InitializeLights()
        {
            // Récupérer toutes les lumières dans le maze
            /*lights = mazeGenerator.GetAllTiles()
                                 .Select(t => t.GetComponentInChildren<Light>())
                                 .Where(l => l != null)
                                 .ToArray();

            noiseOffsets = new float[lights.Length];
            for (int i = 0; i < lights.Length; i++)
                noiseOffsets[i] = Random.Range(0f, 100f);*/
        }
    }
}