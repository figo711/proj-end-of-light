using UnityEngine;

namespace Shared
{
    public class AssetManager : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _bonusPrefab;
        [SerializeField] private GameObject _keyPrefab;

        public static AssetManager Instance { get; private set; }

        public static T SpawnTile<T>(Vector3 position, Transform parent) where T : MonoBehaviour
        {
            var pf = Instance._tilePrefab;
            GameObject tileObj = Instantiate(pf, position, Quaternion.identity, parent);
            return tileObj.GetComponent<T>();
        }

        public static GameObject SpawnTile(Vector3 position, Transform parent)
        {
            var pf = Instance._tilePrefab;
            GameObject tileObj = Instantiate(pf, position, Quaternion.identity, parent);
            return tileObj;
        }

        public static GameObject SpawnBonus(Vector3 position, Transform parent)
        {
            var pf = Instance._bonusPrefab;
            GameObject bonusObj = Instantiate(pf, position, Quaternion.identity, parent);
            return bonusObj;
        }

        public static GameObject SpawnKey(Vector3 position, Transform parent)
        {
            var pf = Instance._keyPrefab;
            GameObject bonusObj = Instantiate(pf, position, Quaternion.identity, parent);
            return bonusObj;
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}