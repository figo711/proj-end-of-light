using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour interagir avec le nouveau système d'entrée (Input System)

/// <summary>
/// Ce script écoute l'état du bouton Trigger (Gâchette) droit 
/// et exécute une action lorsque le bouton est pressé.
/// </summary>
public class TGSave : MonoBehaviour
{
    [Tooltip("Référence à l'Action que nous voulons écouter (ex: RightHand/Trigger).")]
    // L'objet InputActionReference doit être lié dans l'inspecteur Unity.
    public InputActionReference triggerAction = null;

    private void Awake()
    {
        // Vérifie si l'action est définie pour éviter les erreurs.
        if (triggerAction != null && triggerAction.action != null)
        {
            // Abonne la méthode HandleTrigger à l'événement 'performed' (quand l'action est déclenchée)
            triggerAction.action.performed += HandleTrigger;
            // Démarre l'écoute de l'action
            triggerAction.action.Enable();
        }
        else
        {
            Debug.LogError("L'action du Trigger n'est pas configurée dans l'Inspecteur pour TriggerTest sur " + gameObject.name);
        }
    }

    private void OnDestroy()
    {
        // Très important : Désabonner la méthode pour éviter les fuites de mémoire (memory leaks) et les erreurs.
        if (triggerAction != null && triggerAction.action != null)
        {
            triggerAction.action.performed -= HandleTrigger;
        }
    }

    /// <summary>
    /// Méthode appelée lorsque l'action du Trigger est déclenchée (bouton pressé).
    /// </summary>
    private void HandleTrigger(InputAction.CallbackContext context)
    {
        // L'action se déclenche une fois le bouton pressé. 
        // Si vous voulez un événement continu (tant que le bouton est enfoncé), vous utiliseriez 'started' et 'canceled'.

        Debug.Log("===============================================");
        Debug.Log($"Action de la gâchette droite déclenchée ! Valeur: {context.ReadValue<float>()}");
        Debug.Log("Félicitations, votre input VR fonctionne !");
        Debug.Log("===============================================");
    }
}