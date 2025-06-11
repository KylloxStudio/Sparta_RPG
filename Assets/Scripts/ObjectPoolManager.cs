using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    private class ObjectInfo
    {
        public string Name;
        public GameObject Prefab;
        public int Count;
    }

    public bool IsReady { get; private set; }

    [SerializeField] private ObjectInfo[] _objectInfos;
    private string _objName;

    private Dictionary<string, IObjectPool<GameObject>> _ojbectPoolDict = new Dictionary<string, IObjectPool<GameObject>>();
    private Dictionary<string, GameObject> _objectDict = new Dictionary<string, GameObject>();

    protected override bool CheckDontDestroyOnLoad()
    {
        return false;
    }

    protected override void OnAwake()
    {
        IsReady = false;
        for (int i = 0; i < _objectInfos.Length; i++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, _objectInfos[i].Count, _objectInfos[i].Count);

            if (_objectDict.ContainsKey(_objectInfos[i].Name))
            {
                Debug.LogWarningFormat("이미 등록된 오브젝트입니다: {0}", _objectInfos[i].Name);
                return;
            }

            _objectDict.Add(_objectInfos[i].Name, _objectInfos[i].Prefab);
            _ojbectPoolDict.Add(_objectInfos[i].Name, pool);

            for (int j = 0; j < _objectInfos[i].Count; j++)
            {
                _objName = _objectInfos[i].Name;
                PoolObject poolAbleObj = CreatePooledItem().GetComponent<PoolObject>();
                poolAbleObj.ReleaseObject();
            }
        }

        IsReady = true;
    }

    private GameObject CreatePooledItem()
    {
        GameObject poolObj = Instantiate(_objectDict[_objName]);
        poolObj.GetComponent<PoolObject>().Pool = _ojbectPoolDict[_objName];
        return poolObj;
    }

    private void OnTakeFromPool(GameObject poolObj)
    {
        poolObj.SetActive(true);
    }

    private void OnReturnedToPool(GameObject poolObj)
    {
        poolObj.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolObj)
    {
        Destroy(poolObj);
    }

    public GameObject Get(string poolName)
    {
        _objName = poolName;
        if (_objectDict.ContainsKey(poolName) == false)
        {
            Debug.LogWarningFormat("오브젝트풀에 등록되지 않은 오브젝트입니다: {0}", poolName);
            return null;
        }

        return _ojbectPoolDict[poolName].Get();
    }
}