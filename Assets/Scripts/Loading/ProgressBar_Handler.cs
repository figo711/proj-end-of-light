using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
    public class ProgressBar_Handler : MonoBehaviour
    {
        [SerializeField] private Image barFG;
        [SerializeField] private TMP_Text percentTxt;

        private void Start()
        {
        }

        public void UpdateBar(float value)
        {
            barFG.fillAmount = value;
            percentTxt.text = $"{Mathf.RoundToInt(barFG.fillAmount * 100)}%";
        }
    }
}
