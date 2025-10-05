using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("Menu")]
        [SerializeField] private GameObject bgOverlay;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button continueBtn;
        [SerializeField] private Button optionsBtn;
        [SerializeField] private Button quitBtn;

        private InputAction _pauseAction;

        private bool _isPause;

        public bool IsPause => _isPause;

        private void Awake()
        {
            Instance = this;

            _pauseAction = inputActionAsset.FindAction("Pause");
            _pauseAction.performed += OnPressPause;

            continueBtn.onClick.AddListener(() =>
            {
                _isPause = !_isPause;
                UpdatePauseState();
            });

            optionsBtn.onClick.AddListener(() =>
            {
                print("Panels_Handler.ShowPanel(Panel.OPTIONS);");
            });

            quitBtn.onClick.AddListener(() =>
            {
                print("Scene_Handler.SwitchTo(Scenes.MENU);");
            });
        }

        private void OnEnable()
        {
            _pauseAction.Enable();
        }

        private void OnDisable()
        {
            _pauseAction.Disable();
        }

        private void OnPressPause(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _isPause = !_isPause;
                UpdatePauseState();
            }
        }

        private void UpdatePauseState()
        {
            Time.timeScale = IsPause ? 0f : 1f;
            Cursor.lockState = IsPause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsPause;
            menuPanel.SetActive(IsPause);
            bgOverlay.SetActive(IsPause);
        }
    }
}
