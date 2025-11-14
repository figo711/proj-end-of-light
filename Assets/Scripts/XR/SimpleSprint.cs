using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

/// <summary>
/// Gère le sprint en multipliant la vitesse de marche de base.
/// Gère le FOV et affiche la vitesse réelle dans la console.
/// </summary>
public class SimpleSprint : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("L'action du trigger gauche pour courir (ex: LeftHand/Activate).")]
    public InputActionReference sprintAction = null;

    [Tooltip("Le composant qui gère votre déplacement (Continuous Move Provider).")]
    public ContinuousMoveProvider moveProvider = null; 

    [Tooltip("La caméra principale du XR Origin (celle qui a le tag 'MainCamera').")]
    public Camera mainCamera = null;

    [Header("Vitesses")]
    [Tooltip("La vitesse de marche normale.")]
    public float walkSpeed = 3f; 
    
    // MODIFIÉ : 'runSpeed' a été remplacé par un multiplicateur
    [Tooltip("Multiplicateur de sprint. (ex: 2 = deux fois la vitesse de marche).")]
    public float sprintMultiplier = 2f; 

    [Header("Effet de Vitesse (FOV)")]
    [Tooltip("Le FOV (champ de vision) appliqué pendant le sprint.")]
    public float sprintFOV = 70f;
    [Tooltip("La vitesse à laquelle le FOV change pour un effet fluide.")]
    public float fovChangeSpeed = 5f;

    private bool isSprinting = false;
    private float originalFOV; 
    private float m_LastLoggedSpeed = -1f; 

    
    // La fonction OnValidate() a été supprimée car elle n'est plus nécessaire,
    // le calcul se fait en temps réel dans Update().


    private void Start() 
    {
        if (moveProvider == null)
        {
            Debug.LogError("Le 'Move Provider' (ContinuousMoveProvider) n'est pas assigné ! VÉRIFIEZ L'INSPECTEUR !");
            return; 
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("La 'Main Camera' n'est pas assignée. L'effet de FOV sera désactivé. VÉRIFIEZ L'INSPECTEUR !");
        }
        else
        {
            originalFOV = mainCamera.fieldOfView;
            Debug.Log("SimpleSprint : FOV original sauvegardé : " + originalFOV);
        }
        
        moveProvider.moveSpeed = walkSpeed;
        Debug.Log("SimpleSprint : Vitesse initiale réglée sur " + walkSpeed);

        if (sprintAction != null && sprintAction.action != null)
        {
            sprintAction.action.started += HandleSprintStarted;
            sprintAction.action.canceled += HandleSprintCanceled;
            sprintAction.action.Enable();
        }
        else
        {
            Debug.LogError("L'action de Sprint n'est pas configurée.");
        }
    }

    private void OnDestroy()
    {
        if (sprintAction != null && sprintAction.action != null)
        {
            sprintAction.action.started -= HandleSprintStarted;
            sprintAction.action.canceled -= HandleSprintCanceled;
        }
    }

    private void HandleSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void HandleSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    private void Update()
    {
        if (moveProvider == null) return;

        float targetFOV;
        float targetSpeed;

        if (isSprinting)
        {
            // MODIFIÉ : Calcule la vitesse de sprint à la volée
            targetSpeed = walkSpeed * sprintMultiplier;
            targetFOV = sprintFOV;
        }
        else
        {
            targetSpeed = walkSpeed;
            targetFOV = originalFOV;
        }

        // Appliquer la vitesse au provider
        moveProvider.moveSpeed = targetSpeed;

        // Log de la vitesse "réelle"
        if (moveProvider.moveSpeed != m_LastLoggedSpeed)
        {
            Debug.Log("Vitesse actuelle réelle : " + moveProvider.moveSpeed);
            m_LastLoggedSpeed = moveProvider.moveSpeed;
        }


        // Appliquer le changement de FOV en douceur
        if (mainCamera != null && !Mathf.Approximately(mainCamera.fieldOfView, targetFOV))
        {
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView, 
                targetFOV, 
                Time.deltaTime * fovChangeSpeed
            );
        }
    }
}