using DG.Tweening;
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
            barFG.DOFillAmount(0.99f, 3f).OnUpdate(() =>
            {
                percentTxt.text = $"{Mathf.RoundToInt(barFG.fillAmount * 100)}%";
            });
        }
    }
}