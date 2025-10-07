using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("KeyCollector")]
        [SerializeField] private KeyCollector keyCollector;

        [SerializeField] private float interactRange; // 5f

        private InputAction _interactAction;

        private GameObject _exitDoor;

        private void Awake()
        {
            _interactAction = inputActionAsset.FindAction("Interact");

            _interactAction.performed += OnInteract;
        }

        private void OnDestroy()
        {
            _interactAction.performed -= OnInteract;
        }

        private void OnEnable()
        {
            _interactAction.Enable();
        }

        private void OnDisable()
        {
            _interactAction.Disable();
        }

        private void LateUpdate()
        {
            var _dist = Vector3.Distance(
                _exitDoor.transform.position, transform.position);

            if (_dist < 5)
            {
                if (keyCollector.KeyCount >= 3)
                {
                    HUD_Handler.Instance.ShowMsg("Use 'F' to open the door.");
                }
                else
                {
                    HUD_Handler.Instance.ShowMsg("You need 3 keys to open this door.");
                }
            }
            else
            {
                HUD_Handler.Instance.HideMsg();
            }
        }

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            if (Physics.Raycast(
                transform.position, transform.forward, out RaycastHit hit, interactRange))
            {
                if (hit.transform.gameObject.CompareTag("ExitDoor"))
                {
                    if (keyCollector.KeyCount >= 3)
                    {
                        hit.transform.DOMoveY(9, 2f);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ExitTrigger"))
            {
                print("WIN !!!");
            }
        }

        public void Setup()
        {
            _exitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        }
    }
}