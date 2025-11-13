using UnityEngine;

namespace Figo.Mazes
{
    public class TilePlaceholder : MonoBehaviour
    {
        [SerializeField] private bool _hasLeft;
        [SerializeField] private bool _hasBottom;

        [SerializeField] private GameObject _wallLeft;
        [SerializeField] private GameObject _wallBottom;

        public bool HasLeft => _hasLeft;
        public bool HasBottom => _hasBottom;

        public void SetLeftVisibility(bool value)
        {
            _wallLeft.SetActive(value);
            _hasLeft = value;
        }

        public void SetBottomVisibility(bool value)
        {
            _wallBottom.SetActive(value);
            _hasBottom = value;
        }
    }
}