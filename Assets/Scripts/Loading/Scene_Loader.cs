using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Shared;

namespace Loading
{
    public class Scene_Loader : MonoBehaviour
    {
        [SerializeField] private ProgressBar_Handler pgbar;

        private string _sceneName;

        private void Start()
        {
            var scene = Scene_Handler.Instance.SceneToLoad;

            _sceneName = scene.ToString() + "_Scene";

            StartCoroutine(LoadProc());
        }

        private IEnumerator LoadProc()
        {
            yield return new WaitForSecondsRealtime(1.0f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

            while (!asyncLoad.isDone)
            {
                pgbar.UpdateBar(asyncLoad.progress);
                yield return null;
            }
        }
    }
}
