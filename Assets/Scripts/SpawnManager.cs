using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private string[] _spawnPoolObjectNames;

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
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            string objName = _spawnPoolObjectNames[Random.Range(0, _spawnPoolObjectNames.Length)];

            GameObject obj = Singleton<ObjectPoolManager>.Instance().Get(objName);
            obj.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
}
