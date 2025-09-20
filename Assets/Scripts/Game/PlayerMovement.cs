using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("Movement")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float groundDrag;

        [Header("Ground Check")]
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;

        [Space(5)]
        [SerializeField] private Transform orientation;

        private float hInput;
        private float vInput;

        private Vector3 moveDirection;
        private Rigidbody rb;
        private InputAction _moveAction;

        private bool _grounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            _moveAction = inputActionAsset.FindAction("Move");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
        }

        private void Update()
        {
            _grounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                playerHeight * 0.5f + 0.2f,
                whatIsGround
            );

            HandleInput();
            SpeedControl();

            rb.linearDamping = _grounded ? groundDrag : 0;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void HandleInput()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();

            hInput = moveInput.x;
            vInput = moveInput.y;
        }

        private void MovePlayer()
        {
            moveDirection = orientation.forward * vInput + orientation.right * hInput;

            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(
                    limitedVel.x,
                    rb.linearVelocity.y,
                    limitedVel.z
                );
            }
        }
    }
}