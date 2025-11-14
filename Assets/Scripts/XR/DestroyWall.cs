using UnityEngine;
using UnityEngine.InputSystem; 
using Unity.AI.Navigation;

/// <summary>
/// Ce script écoute l'état du bouton Trigger (Gâchette) droit
/// et détruit les objets "Mur" (par Tag ou Layer) lorsque le bouton est pressé.
/// </summary>
public class DestroyWall : MonoBehaviour
{
    [Tooltip("Référence à l'Action que nous voulons écouter (ex: RightHand/Trigger).")]
    public InputActionReference triggerAction = null;

    // --- NOUVELLES VARIABLES ---
    [Header("Paramètres de Destruction")]
    [Tooltip("Le Transform de la manette (d'où part le rayon).")]
    public Transform controllerTransform; // À glisser dans l'Inspecteur

    [Tooltip("La distance maximale du rayon.")]
    public float laserRange = 10f;

    [Tooltip("Le Layer (Calque) des murs à détruire.")]
    public LayerMask wallLayer; // À sélectionner dans l'Inspecteur
    // --- FIN DES NOUVELLES VARIABLES ---
    [Tooltip("Référence au NavMeshSurface à reconstruire après destruction.")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        // Vérifie si l'action est définie
        if (triggerAction != null && triggerAction.action != null)
        {
            triggerAction.action.performed += HandleTrigger;
            triggerAction.action.Enable();
        }
        else
        {
            Debug.LogError("L'action du Trigger n'est pas configurée dans l'Inspecteur.");
        }

        // Vérification de sécurité pour la nouvelle variable
        if (controllerTransform == null)
        {
            Debug.LogError("Le 'Controller Transform' n'est pas assigné. Le rayon ne partira de nulle part.");
        }
    }

    private void OnDestroy()
    {
        // Désabonner la méthode
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
        // Sécurité : si le transform n'est pas assigné, on ne fait rien.
        if (controllerTransform == null) return;

        // On lance un rayon (Raycast) depuis la position de la manette, vers l'avant
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, laserRange))
        {
            // Le rayon a touché quelque chose. On vérifie si c'est un mur.
            
            // Condition 1 : L'objet a le tag "Wall"
            bool hasWallTag = hit.transform.CompareTag("Wall");

            // Condition 2 : L'objet est sur le Layer "wall" (que vous avez défini dans wallLayer)
            // (1 << hit.collider.gameObject.layer) crée un masque binaire pour le layer de l'objet touché
            // On vérifie s'il correspond au masque que vous avez réglé dans l'inspecteur
            bool isOnWallLayer = (wallLayer.value & (1 << hit.collider.gameObject.layer)) > 0;

            if (hasWallTag || isOnWallLayer)
            {
                // C'est un mur ! On le détruit.
                Destroy(hit.transform.gameObject);
                navMeshSurface.BuildNavMesh();

                
                // Message de succès dans la console
                Debug.Log($"================ MUR DÉTRUIT ================ \nNom de l'objet : {hit.transform.name}");
            }
            else
            {
                // On a touché quelque chose, mais ce n'est pas un mur.
                Debug.Log($"Action déclenchée : Touché {hit.transform.name}, mais ce n'est pas un mur.");
            }
        }
        else
        {
            // Le rayon n'a rien touché.
            Debug.Log("Action déclenchée : Rien n'a été touché.");
        }
    }
}