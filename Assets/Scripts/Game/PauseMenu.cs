using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Shared;
using TMPro;

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

        [Header("End Game")]
        [SerializeField] private GameObject endPanel;
        [SerializeField] private TextMeshProUGUI endGameText;
        [SerializeField] private Button endQuitBtn;

        private InputAction _pauseAction;

        private bool _isPause;
        private bool _isEnd;

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

            quitBtn.onClick.AddListener(OnClickQuit);
            endQuitBtn.onClick.AddListener(OnClickQuit);
        }

        private void OnClickQuit()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Scene_Handler.Instance.SwitchTo(Scenes.Menu);
        }

        private void OnDestroy()
        {
            _pauseAction.performed -= OnPressPause;
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
            if (_isEnd) return;
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

        public void EndGame(bool win)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _isPause = true;
            _isEnd = true;

            endPanel.SetActive(true);
            bgOverlay.SetActive(true);
            endGameText.text = win ? "WIN !" : "LOSE";
        }
    }
}
