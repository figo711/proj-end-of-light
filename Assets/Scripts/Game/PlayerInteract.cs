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
        [SerializeField] private ObjectHandler objectHandler;
        [SerializeField] private HP_Handler hpHandler;
        [SerializeField] private PlayerMovement pMovement;

        [SerializeField] private float interactRange; // 5f
        [SerializeField] private LayerMask collectLayer;

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

            Camera cam = Camera.main;
            Ray ray = new(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit _, interactRange, collectLayer))
            {
                HUD_Handler.Instance.ShowMsg("Use 'F' to take.");
            }
        }

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            Camera cam = Camera.main;
            Ray ray = new(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                if (hit.transform.gameObject.CompareTag("ExitDoor"))
                {
                    if (keyCollector.KeyCount >= 3)
                    {
                        hit.transform.DOMoveY(9, 2f);
                    }
                }

                if (hit.transform.gameObject.CompareTag("Bonus"))
                {
                    hit.transform.gameObject.SetActive(false);
                    if (Random.value > 0.5f)
                    {
                        hpHandler.Heal(25);
                    }
                    else
                    {
                        pMovement.crouchSpeed += 0.5f;
                    }
                }

                if (hit.transform.gameObject.CompareTag("Stone"))
                {
                    hit.transform.gameObject.SetActive(false);
                    objectHandler.AddStone();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ExitTrigger"))
            {
                PauseMenu.Instance.EndGame(true);
            }
        }

        public void Setup()
        {
            _exitDoor = GameObject.FindGameObjectWithTag("ExitDoor");
        }
    }
}
