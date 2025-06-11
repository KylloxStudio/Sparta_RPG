using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private SpawnPoint[] _spawnPoints;
    [SerializeField] private string[] _spawnPoolObjectNames;
    [SerializeField] private int _maxSpawnCount;

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    public List<GameObject> SpawnedObjects => _spawnedObjects;

    protected override bool CheckDontDestroyOnLoad()
    {
        return false;
    }

    private void Start()
    {
        StartCoroutine(ProcessSpawn());
    }

    private IEnumerator ProcessSpawn()
    {
        while (true)
        {
            if (SpawnedObjects.Count >= _maxSpawnCount)
            {
                continue;
            }

            int attemp = 0;
            int idx = 0;
            bool isValidPosition;
            do
            {
                if (attemp >= _spawnPoints.Length)
                {
                    break;
                }

                idx = Random.Range(0, _spawnPoints.Length);
                isValidPosition = !_spawnPoints[idx].IsUsingPoint;
                attemp++;
            }
            while (!isValidPosition);

            //Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            SpawnPoint spawnPoint = _spawnPoints[idx];
            string objName = _spawnPoolObjectNames[Random.Range(0, _spawnPoolObjectNames.Length)];

            GameObject obj = Singleton<ObjectPoolManager>.Instance().Get(objName);
            obj.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
            SpawnedObjects.Add(obj);

            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
}
