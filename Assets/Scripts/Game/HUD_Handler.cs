using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HUD_Handler : MonoBehaviour
    {
        public static HUD_Handler Instance { get; private set; }

        [Header("Stamina")]
        [SerializeField] private Image staminaBar;

        [Header("Keys")]
        [SerializeField] private Image keysBar;
        [SerializeField] private TextMeshProUGUI keysText;

        [Header("Stones")]
        [SerializeField] private Image stonesBar;
        [SerializeField] private TextMeshProUGUI stonesText;

        [Header("HP")]
        [SerializeField] private Image hpBar;

        [Header("Indication")]
        [SerializeField] private TextMeshProUGUI indicationText;

        private void Awake()
        {
            Instance = this;

            staminaBar.fillAmount = 1f;
            keysBar.fillAmount = 0f;
            keysText.text = "0 / 3";
            stonesBar.fillAmount = 5f;
            stonesText.text = "5 / 5";
        }

        public void UpdateHP(int value)
        {
            hpBar.fillAmount = (float)value / HP_Handler.MAX_HP;
        }

        public void UpdateStamina(float value)
        {
            staminaBar.fillAmount = value / PlayerMovement.MAX_STAMINA;
        }

        public void UpdateKeys(int value)
        {
            keysBar.fillAmount = value / 3.0f;
            keysText.text = $"{value} / 3";
        }

        public void UpdateStones(int value)
        {
            stonesBar.fillAmount = value / 5.0f;
            stonesText.text = $"{value} / 5";
        }

        public void ShowMsg(string value)
        {
            indicationText.gameObject.SetActive(true);
            indicationText.text = value;
        }

        public void HideMsg()
        {
            indicationText.gameObject.SetActive(false);
        }
    }
}
