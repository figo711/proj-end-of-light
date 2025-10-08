using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyAgent : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private Vector3 _destination;
        private Vector3 _startPosition;
        private GameObject[] _keys;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startPosition = transform.position;
        }

        public void Setup()
        {
            _keys = GameObject.FindGameObjectsWithTag("Key");
            var newPlace = _keys[Random.Range(0, _keys.Length)].transform.position;
            _destination = newPlace;
            _agent.SetDestination(_destination);
        }

        private void LateUpdate()
        {
            if (_agent.hasPath || _agent.pathPending)
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
    }
}
