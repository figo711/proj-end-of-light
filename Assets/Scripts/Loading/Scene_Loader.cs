using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using Shared;

namespace Loading
{
    public class Scene_Loader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private ProgressBar_Handler pgbar;

        private string _sceneName;
        private InputAction _interactAction;

        private void Awake()
        {
            _interactAction = inputActionAsset.FindAction("Interact");
        }

        private void Start()
        {
            var scene = Scene_Handler.Instance.SceneToLoad;

            _sceneName = scene.ToString() + "_Scene";

            StartCoroutine(LoadProc());
        }

        private void OnEnable()
        {
            _interactAction.Enable();
        }

        private void OnDisable()
        {
            _interactAction.Disable();
        }


        private IEnumerator LoadProc()
        {
            // yield return new WaitForSecondsRealtime(1.0f);

            pgbar.UpdateBar(0);

            yield return new WaitUntil(() => _interactAction.triggered);

            pgbar.UpdateBar(90);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

            while (!asyncLoad.isDone)
            {
                pgbar.UpdateBar(asyncLoad.progress);
                yield return null;
            }
        }
    }
}
