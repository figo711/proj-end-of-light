using UnityEngine;
using UnityEngine.UI;
using Shared;

namespace Menu
{
    public class Menu_Handler : MonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button optionsBtn;
        [SerializeField] private Button quitBtn;

        private void Awake()
        {
            startBtn.onClick.AddListener(() =>
            {
                Scene_Handler.Instance.SwitchTo(Scenes.Game);
                print("DBG: START GAME");
            });

            optionsBtn.onClick.AddListener(() =>
            {
                // Menu_Handler.ShowPanel(Panel.OPTIONS);
                print("DBG: SHOW OPTIONS");
            });

            quitBtn.onClick.AddListener(() =>
            {
                print("DBG: CLOSE GAME");
                Application.Quit();
            });
        }
    }
}
