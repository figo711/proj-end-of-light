using Figo.Timer;
using UnityEngine;

namespace Game
{
    public class HP_Handler : MonoBehaviour
    {
        [SerializeField] private PlayerMovement pMovement;

        public const int MAX_HP = 200;
        private int _health;
        private LightController _currentTile;
        private FloatTimer _timer;

        private float _defaultRunSpeed;
        private float _defaultWalkSpeed;

        private void Awake()
        {
            _health = MAX_HP;
            _defaultRunSpeed = pMovement.runSpeed;
            _defaultWalkSpeed = pMovement.walkSpeed;
        }

        private void Start()
        {
            HUD_Handler.Instance.UpdateHP(_health);

            _timer = new FloatTimer(0.75f, () =>
            {
                _health -= 1;
                HUD_Handler.Instance.UpdateHP(_health);

                if (_health <= 0)
                {
                    _health = 0;
                    HUD_Handler.Instance.UpdateHP(_health);
                    PauseMenu.Instance.EndGame(false);
                }
            }, true);
            _timer.Active = true;
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

                if (_health <= 0)
                {
                    _health = 0;
                    HUD_Handler.Instance.UpdateHP(_health);
                    PauseMenu.Instance.EndGame(false);
                }
            }
            else if (collider.gameObject.CompareTag("Lights"))
            {
                var tile = collider.GetComponentInParent<LightController>();
                if (tile) _currentTile = tile;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            var tile = collider.GetComponent<LightController>();
            if (tile && tile == _currentTile)
                _currentTile = null;
        }

        private void Update()
        {
            if (_currentTile != null && !_currentTile.IsLit)
            {
                pMovement.runSpeed = _defaultRunSpeed / 1.75f;
                pMovement.walkSpeed = _defaultWalkSpeed / 1.75f;
                _timer.Update(Time.deltaTime);
            }
            else
            {
                pMovement.runSpeed = _defaultRunSpeed;
                pMovement.walkSpeed = _defaultWalkSpeed;
                _timer.Reset();
            }
        }
    }
}
