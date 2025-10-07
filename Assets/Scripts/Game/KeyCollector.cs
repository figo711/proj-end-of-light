using UnityEngine;

namespace Game
{
    public class KeyCollector : MonoBehaviour
    {
        private int keyCount;

        public int KeyCount => keyCount;

        private void Start()
        {
            keyCount = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Key"))
            {
                other.enabled = false;
                other.gameObject.SetActive(false);
                keyCount += 1;
                HUD_Handler.Instance.UpdateKeys(keyCount);
            }
        }
    }
}
