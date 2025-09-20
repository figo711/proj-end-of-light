using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Space(5)]
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Transform orientation;

        private float _xRot;
        private float _yRot;

        private InputAction _lookAction;

        private void Awake()
        {
            _lookAction = inputActionAsset.FindAction("Look");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            var mouseDelta = _lookAction.ReadValue<Vector2>();

            float mouseX = mouseDelta.x * Time.deltaTime * sensX;
            float mouseY = mouseDelta.y * Time.deltaTime * sensY;

            _yRot += mouseX;
            _xRot -= mouseY;
            _xRot = Mathf.Clamp(_xRot, -90f, 90f);

            transform.rotation = Quaternion.Euler(_xRot, _yRot, 0);
            orientation.rotation = Quaternion.Euler(0, _yRot, 0);
        }
    }
}