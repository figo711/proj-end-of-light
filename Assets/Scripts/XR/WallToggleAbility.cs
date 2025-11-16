using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections;
using Figo.Mazes; // NOUVEAU: Nécessaire pour les Coroutines

/// <summary>
/// Gère un Grab/Collect via Raycast (Laser) qui s'active à la pression de la gâchette,
/// et ajoute la fonctionalité de désactivation de murs si un objet est saisi/collecté.
/// </summary>
public class WallToggleAbility : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence à l'Action de Grab (ex: RightHand/Select).")]
    public InputActionReference grabAction = null;

    [Tooltip("Le Transform de la manette (d'où part le laser).")]
    public Transform controllerTransform = null;

    [Tooltip("Référence au composant LineRenderer qui dessinera le laser.")]
    public LineRenderer laserLine = null;

    [Header("Paramètres du Laser/Grab")]
    [Tooltip("Distance maximale du laser.")]
    public float laserRange = 10f;

    [Tooltip("Définit quels objets peuvent être détectés (Layer 'Grabbable').")]
    public LayerMask grabbableLayer;

    [Header("Fonctionnalité Mur")]
    [Tooltip("Durée de la désactivation des murs (en secondes).")]
    public float toggleDuration = 2.0f; // NOUVEAU

    [SerializeField] private MazeGenerator mazeGenerator;

    // NOUVEAU : État pour éviter de relancer l'effet si déjà en cours
    private bool isTogglingWalls = false;
    private bool isRegenerateLevel = false;

    // Variables privées pour le grab
    private Transform grabbedObject = null;
    private Rigidbody grabbedRigidbody = null;
    private bool originalRigidbodyState;
    private bool isTriggerDown = false;

    private void Awake()
    {
        if (grabAction != null && grabAction.action != null)
        {
            grabAction.action.started += HandleGrabStarted;
            grabAction.action.canceled += HandleGrabCanceled;
            grabAction.action.Enable();
        }
        else
        {
            Debug.LogError("L'action du Grab n'est pas configurée.");
        }

        if (controllerTransform == null)
        {
            Debug.LogError("Le 'Controller Transform' n'est pas assigné.");
        }

        // --- Configuration du LineRenderer ---
        if (laserLine == null)
        {
            Debug.LogError("La 'Laser Line' (LineRenderer) n'est pas assignée.");
        }
        else
        {
            laserLine.positionCount = 2;
            laserLine.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (grabAction != null && grabAction.action != null)
        {
            grabAction.action.started -= HandleGrabStarted;
            grabAction.action.canceled -= HandleGrabCanceled;
        }
    }

    /// <summary>
    /// Appelée à chaque frame pour dessiner le laser. (Inchangée)
    /// </summary>
    private void Update()
    {
        if (controllerTransform == null || laserLine == null || !isTriggerDown || grabbedObject != null)
        {
            if (laserLine != null) laserLine.enabled = false;
            return;
        }

        if (!laserLine.enabled)
        {
            laserLine.enabled = true;
        }

        Vector3 startPosition = controllerTransform.position;
        Vector3 endPosition;

        RaycastHit hit;
        if (Physics.Raycast(startPosition, controllerTransform.forward, out hit, laserRange, grabbableLayer))
        {
            endPosition = hit.point;
        }
        else
        {
            endPosition = startPosition + controllerTransform.forward * laserRange;
        }

        laserLine.SetPosition(0, startPosition);
        laserLine.SetPosition(1, endPosition);
    }

    /// <summary>
    /// Méthode appelée lorsque l'action de Grab COMMENCE (bouton pressé).
    /// </summary>
    private void HandleGrabStarted(InputAction.CallbackContext context)
    {
        isTriggerDown = true;

        if (controllerTransform == null) return;
        if (grabbedObject != null) return;

        // --- LOGIQUE DE RAYCAST POUR LE GRAB ---
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, laserRange, grabbableLayer))
        {
            Transform hitTransform = hit.transform;
            // Haptique au moment du grab/collect (Doit être dans une fonction de support)
            // (La fonction TriggerHaptics doit être ajoutée ou déclarée ici pour que ça compile.)
            // TriggerHaptics(context.control.device); 

            // Logique de Collecte (Destruction)
            if (hitTransform.CompareTag("Key") || hitTransform.CompareTag("Stone") || hitTransform.CompareTag("Bonus"))
            {
                Debug.Log($"Objet collecté (Laser) : {hitTransform.name}");
                Destroy(hitTransform.gameObject);

                if (hitTransform.CompareTag("Key") && !isRegenerateLevel)
                {
                    StartCoroutine(RegenerateLevel());
                }

                if (hitTransform.CompareTag("Bonus") && !isTogglingWalls)
                {
                    StartCoroutine(ToggleWalls());
                }
            }
            // Logique de Grab Normal
            else
            {
                Debug.Log($"Objet normal saisi (Laser) : {hitTransform.name}");

                grabbedObject = hitTransform;
                grabbedRigidbody = hitTransform.GetComponent<Rigidbody>();
                grabbedObject.SetParent(controllerTransform);

                if (grabbedRigidbody != null)
                {
                    originalRigidbodyState = grabbedRigidbody.isKinematic;
                    grabbedRigidbody.isKinematic = true;
                }
            }
        }
    }

    /// <summary>
    /// Méthode appelée lorsque l'action de Grab SE TERMINE (bouton relâché).
    /// </summary>
    private void HandleGrabCanceled(InputAction.CallbackContext context)
    {
        isTriggerDown = false;

        if (grabbedObject != null)
        {
            Debug.Log($"Relâchement de : {grabbedObject.name}");
            grabbedObject.SetParent(null);

            if (grabbedRigidbody != null)
            {
                grabbedRigidbody.isKinematic = originalRigidbodyState;
            }
            grabbedObject = null;
            grabbedRigidbody = null;
        }
    }

    // --- NOUVELLE LOGIQUE DE DÉSACTIVATION DES MURS ---

    /// <summary>
    /// Coroutine pour gérer l'activation et la désactivation temporaire des murs.
    /// </summary>
    private IEnumerator ToggleWalls()
    {
        isTogglingWalls = true; // Empêche de relancer l'effet

        // 1. Désactiver tous les murs
        mazeGenerator.ToggleInnerWalls(false);
        Debug.Log($"Désactivation des murs pendant {toggleDuration} secondes.");

        // 2. Attendre
        yield return new WaitForSeconds(toggleDuration);

        // 3. Réactiver tous les murs
        mazeGenerator.ToggleInnerWalls(true);
        Debug.Log("Réactivation des murs.");

        isTogglingWalls = false; // L'effet est terminé
    }

    /// <summary>
    /// Coroutine pour gérer l'activation et la désactivation temporaire des murs.
    /// </summary>
    private IEnumerator RegenerateLevel()
    {
        isRegenerateLevel = true;

        mazeGenerator.ToggleInnerWalls(false);

        yield return new WaitForSeconds(toggleDuration);

        mazeGenerator.Regenerate();
        print("Regenerate Done !");

        isRegenerateLevel = false;
    }
}