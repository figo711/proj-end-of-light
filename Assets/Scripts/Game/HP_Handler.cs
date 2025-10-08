using UnityEngine;

namespace Game
{
    public class HP_Handler : MonoBehaviour
    {
        public const int MAX_HP = 100;
        private int _health;

        private void Awake()
        {
            _health = MAX_HP / 2;
        }

        private void Start()
        {
            HUD_Handler.Instance.UpdateHP(_health);
        }

        public void Heal(int value)
        {
            _health += value;
            if (_health > MAX_HP)
                _health = MAX_HP;
            HUD_Handler.Instance.UpdateHP(_health);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                _health -= MAX_HP / 4;
                HUD_Handler.Instance.UpdateHP(_health);
                // TODO: Add bounce from BOSS

                if (_health <= 0)
                {
                    _health = 0;
                    HUD_Handler.Instance.UpdateHP(_health);
                    PauseMenu.Instance.EndGame(false);
                }
            }
        }
    }
}
