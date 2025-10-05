using UnityEngine;
using DG.Tweening;

namespace Menu
{
    public class MenuCameraAnim : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        private void LateUpdate()
        {
            cameraTransform.Rotate(new Vector3(5, 10, 15) * Time.deltaTime);
        }
    }
}