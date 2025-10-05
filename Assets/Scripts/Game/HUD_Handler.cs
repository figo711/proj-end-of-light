using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HUD_Handler : MonoBehaviour
    {
        public static HUD_Handler Instance { get; private set; }

        [Header("Stamina")]
        [SerializeField] private Image staminaBar;

        private void Awake()
        {
            Instance = this;

            staminaBar.fillAmount = 1f;
        }

        public void UpdateStamina(float value)
        {
            staminaBar.fillAmount = value / PlayerMovement.MAX_STAMINA;
        }
    }
}