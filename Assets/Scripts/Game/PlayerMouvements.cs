using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
// Classe gérant le mouvement du joueur (déplacement, saut, accroupissement, rotation caméra)
public class PlayerMovement : MonoBehaviour
{

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    // Référence à la caméra du joueur (utilisée pour la rotation en pitch)
    public Camera playerCamera;

    // Vitesses de déplacement (paramètres exposés dans l'inspecteur)
    public float walkSpeed = 6f;     // vitesse en marchant
    public float runSpeed = 12f;     // vitesse en courant

    // Paramètres de saut et gravité
    public float jumpPower = 7f;     // force initiale du saut
    public float gravity = 10f;      // force de gravité appliquée lorsque l'on est en l'air

    // Contrôle de la caméra (souris)
    public float lookSpeed = 2f;     // sensibilité souris
    public float lookXLimit = 45f;   // limite d'inclinaison verticale (en degrés)

    // Paramètres d'accroupissement / taille du CharacterController
    public float defaultHeight = 2f; // hauteur par défaut du CharacterController
    public float crouchHeight = 1f;  // hauteur quand accroupi
    public float crouchSpeed = 3f;   // vitesse de déplacement quand accroupi


    // État interne
    private Vector3 moveDirection = Vector3.zero; // vecteur de déplacement courant (inclut y pour saut/gravite)
    private float rotationX = 0;                  // rotation verticale accumulée pour la caméra
    private CharacterController characterController; // référence au CharacterController attaché
    private InputAction _moveAction;
    private InputAction _crouchAction;
    private InputAction _lookAction;
    private InputAction _sprintAction;

    // Permet de désactiver le mouvement (utile pour cutscenes, menus, etc.)
    private bool canMove = true;


    void Awake()
    {
        _moveAction = inputActionAsset.FindAction("Move");
        _crouchAction = inputActionAsset.FindAction("Crouch");
        _lookAction = inputActionAsset.FindAction("Look");
        _sprintAction = inputActionAsset.FindAction("Sprint");
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _crouchAction.Enable();
        _lookAction.Enable();
        _sprintAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _crouchAction.Disable();
        _lookAction.Disable();
        _sprintAction.Disable();
    }
    
    void Start()
    {
        // Récupère le CharacterController sur le GameObject et verrouille le curseur
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // verrouille le curseur au centre de l'écran
        Cursor.visible = false;                   // cache le curseur
    }


    void Update()
    {
        // Calcul des directions locales avant/droite en fonction de l'orientation du transform
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Vérifie si le joueur maintient la touche shift pour courir
        bool isRunning = _sprintAction.IsPressed();

        var moveInput = _moveAction.ReadValue<Vector2>();
        // Calcul de la vitesse sur les axes X et Y locaux (Vertical -> forward/back, Horizontal -> right/left)
        // Si canMove est false, les vitesses sont forcées à 0
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * moveInput.y : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * moveInput.x : 0;

        // Sauvegarde de la composante Y du mouvement (pour conserver la gravité/saut entre les frames)
        float movementDirectionY = moveDirection.y;

        // Construction du vecteur de mouvement horizontal (X/Z)
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Rotation de la caméra et du corps si le mouvement est autorisé
        if (canMove)
        {
            var lookInput = _lookAction.ReadValue<Vector2>();
            // Pitch (regard haut/bas) : inversé par convention (souris vers le haut -> regard vers le bas)
            rotationX += - lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // clamp pour éviter de se retourner
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Yaw (rotation autour de l'axe Y) : applique la rotation horizontale au transform du joueur
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
    }

    void FixedUpdate()
    {
        // Application de la gravité lorsque le joueur n'est pas au sol
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Accroupissement temporaire : quand la touche C est enfoncée
        // Attention : modifier la hauteur du CharacterController peut nécessiter d'ajuster aussi le center
        if (_crouchAction.IsPressed() && canMove)
        {
            characterController.height = crouchHeight; // réduit la hauteur du CharacterController
            walkSpeed = crouchSpeed;                   // réduit la vitesse de marche
            runSpeed = crouchSpeed;                    // empêche de courir (cours = vitesse d'accroupissement)
        }
        else
        {
            // Réinitialise la hauteur et les vitesses aux valeurs par défaut
            characterController.height = defaultHeight;
            walkSpeed = 5f;
            runSpeed = 10f;
        }

        // Applique le déplacement final (Time.deltaTime pour rendre indépendant du framerate)
        characterController.Move(moveDirection * Time.deltaTime);
    }
}