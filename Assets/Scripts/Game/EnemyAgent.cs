using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyAgent : MonoBehaviour
    {
        [SerializeField] private PlayerMovement pMovement;

        public static EnemyAgent Instance;

        private NavMeshAgent _agent;

        private Vector3 _destination;
        private Vector3 _startPosition;
        private GameObject[] _keys;

        private bool _distraction;

        private void Awake()
        {
            Instance = this;

            _agent = GetComponent<NavMeshAgent>();
            _startPosition = transform.position;
            _distraction = false;
        }

        public void Setup()
        {
            _keys = GameObject.FindGameObjectsWithTag("Key");
            var newPlace = _keys[Random.Range(0, _keys.Length)].transform.position;
            _destination = newPlace;
            _agent.SetDestination(_destination);
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, pMovement.transform.position) > 30f)
            {
                return;
            }

            if (_distraction) return;

            if (pMovement.IsRunning)
            {
                _agent.SetDestination(pMovement.transform.position);
            }
        }

        private void LateUpdate()
        {
            if (_agent.hasPath || _agent.pathPending || _distraction)
                return;

            var newPlace = _keys[Random.Range(0, _keys.Length)].transform.position;
            if (newPlace != _destination)
            {
                _destination = newPlace;
                _agent.SetDestination(_destination);
            }
            else
                _agent.SetDestination(_startPosition);
        }

        public void SetDistraction(Vector3 position)
        {
            if (Vector3.Distance(transform.position, position) > 30f)
            {
                return;
            }
            print("BOOM!!");
            _distraction = true;
            _agent.SetDestination(position);
            StartCoroutine(WalkUntilDone());
        }

        private IEnumerator WalkUntilDone()
        {
            while (_agent.remainingDistance > 1f)
            {
                yield return null;
            }

            _distraction = false;
        }
    }
}
