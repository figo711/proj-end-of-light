using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR; // Nécessaire pour les Haptiques

/// <summary>
/// Gère un Grab/Collect via Raycast (Laser) qui s'active à la pression de la gâchette.
/// </summary>
public class GrabInputTest : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence à l'Action de Grab (ex: RightHand/Select).")]
    public InputActionReference grabAction = null;

    [Tooltip("Le Transform de la manette (d'où part le laser).")]
    public Transform controllerTransform = null;

    [Tooltip("Référence au composant LineRenderer qui dessinera le laser.")]
    public LineRenderer laserLine = null; // <-- RESTAURÉ

    [Header("Paramètres du Laser/Grab")]
    [Tooltip("Distance maximale du laser.")]
    public float laserRange = 10f; // <-- RESTAURÉ
    
    [Tooltip("Définit quels objets peuvent être détectés (Layer 'Grabbable').")]
    public LayerMask grabbableLayer;

    // Variables privées pour le grab
    private Transform grabbedObject = null;
    private Rigidbody grabbedRigidbody = null;
    private bool originalRigidbodyState;
    
    // Suit l'état du trigger pour le dessin et l'action
    private bool isTriggerDown = false; // <-- RESTAURÉ

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
            laserLine.enabled = false; // Le laser est caché par défaut
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
    /// Appelée à chaque frame pour dessiner le laser.
    /// </summary>
    private void Update()
    {
        // Ne dessine que si le trigger est pressé ET qu'on ne tient rien
        if (controllerTransform == null || laserLine == null || !isTriggerDown || grabbedObject != null) 
        {
            if(laserLine != null) laserLine.enabled = false;
            return;
        }
        
        if (!laserLine.enabled)
        {
            laserLine.enabled = true;
        }
        
        // 1. Lance le Raycast pour le visuel
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

        // 2. Dessine le rayon
        laserLine.SetPosition(0, startPosition); 
        laserLine.SetPosition(1, endPosition); 
    }

    /// <summary>
    /// Méthode appelée lorsque l'action de Grab COMMENCE (bouton pressé).
    /// </summary>
    private void HandleGrabStarted(InputAction.CallbackContext context)
    {
        // Active le dessin du laser
        isTriggerDown = true;

        if (controllerTransform == null) return; 
        if (grabbedObject != null) return; 

        // --- LOGIQUE DE RAYCAST POUR LE GRAB ---
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, laserRange, grabbableLayer))
        {
            Transform hitTransform = hit.transform;
            
            // Haptique au moment du grab/collect

            // Logique de Collecte (Destruction)
            if (hitTransform.CompareTag("Key") || hitTransform.CompareTag("Stone") || hitTransform.CompareTag("Bonus"))
            {
                Debug.Log($"Objet collecté (Laser) : {hitTransform.name}");
                Destroy(hitTransform.gameObject);
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
        // Désactive le dessin du laser
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

}