using Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class ObjectHandler : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Transform objectHolder;
        [SerializeField] private float throwForce; // 500f

        private int _objectCount;

        private InputAction _attackAction;

        private void Awake()
        {
            _objectCount = 5;
            _attackAction = inputActionAsset.FindAction("Attack");

            _attackAction.performed += OnThrow;
        }

        private void OnDestroy()
        {
            _attackAction.performed -= OnThrow;
        }

        private void OnEnable()
        {
            _attackAction.Enable();
        }

        private void OnDisable()
        {
            _attackAction.Disable();
        }

        private void OnThrow(InputAction.CallbackContext ctx)
        {
            if (_objectCount <= 0)
            {
                return;
            }
            _objectCount -= 1;
            if (_objectCount <= 0)
            {
                objectHolder.gameObject.SetActive(false);
            }

            HUD_Handler.Instance.UpdateStones(_objectCount);

            var stone = AssetManager.SpawnStone(objectHolder.position);

            stone.GetComponent<Rigidbody>().AddForce(
                Camera.main.transform.forward * throwForce);
        }

        public void AddStone()
        {
            objectHolder.gameObject.SetActive(true);
            _objectCount += 1;
            HUD_Handler.Instance.UpdateStones(_objectCount);
        }
    }
}
