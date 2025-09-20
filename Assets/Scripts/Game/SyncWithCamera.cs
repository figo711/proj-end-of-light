using UnityEngine;

namespace Game
{
    public class SyncWithCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraPos;

        private void Update()
        {
            transform.position = cameraPos.position;
        }
    }
}
