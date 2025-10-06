using UnityEngine.SceneManagement;
using Figo.Singleton;

namespace Shared
{
    public enum Scenes
    {
        Menu,
        Game
    }

    public class Scene_Handler : Singleton<Scene_Handler>
    {
        private Scenes _sceneToLoad;

        public Scenes SceneToLoad => _sceneToLoad;

        public void SwitchTo(Scenes nextScene)
        {
            _sceneToLoad = nextScene;

            SceneManager.LoadScene("Loading_Scene");
        }
    }
}
