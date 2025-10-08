using UnityEngine;

namespace Game
{
    public class StoneEffect : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Lights"))
            {
                EnemyAgent.Instance.SetDistraction(collider.transform.position);
            }
        }
    }
}