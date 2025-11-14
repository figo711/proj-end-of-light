using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement; // Seul 'using' de XRI nécessaire

/// <summary>
/// Gère le sprint en forçant la vitesse à chaque image (Update)
/// sur un ContinuousMoveProvider standard.
/// </summary>
public class SimpleSprint : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("L'action du trigger gauche pour courir (ex: LeftHand/Activate).")]
    public InputActionReference sprintAction = null;

    [Tooltip("Le composant qui gère votre déplacement (Continuous Move Provider).")]
    // MODIFIÉ : Nous utilisons la classe de base
    public ContinuousMoveProvider moveProvider = null; 

    [Header("Vitesses")]
    public float walkSpeed = 3f; 
    public float runSpeed = 8f; 

    private bool isSprinting = false;

    private void Start() 
    {
        if (moveProvider == null)
        {
            Debug.LogError("Le 'Move Provider' (ContinuousMoveProvider) n'est pas assigné !");
            return; 
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
        Debug.LogWarning("SPRINT ACTIVÉ !"); 
    }

    private void HandleSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        Debug.LogWarning("SPRINT DÉSACTIVÉ.");
    }

    private void Update()
    {
        if (moveProvider == null) return;

        if (isSprinting)
        {
            moveProvider.moveSpeed = runSpeed;
        }
        else
        {
            moveProvider.moveSpeed = walkSpeed;
        }
    }
}