using System.Collections;
using System.Linq;
using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    public bool IsReadyToStart { get; private set; }

    protected override bool CheckDontDestroyOnLoad()
    {
        return false;
    }

    protected override void OnAwake()
    {
        Singleton<GameManager>.Instance().IsInGame = true;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => LocalPlayer != null);
        IsReadyToStart = true;

        yield break;
	}

    public Player LocalPlayer { get; private set; }

    public void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
    }

    public GameObject GetNearestGameObject(GameObject[] objects)
    {
        if (!IsReadyToStart)
        {
            return null;
        }

        GameObject obj = null;

        float nearestDistance = Mathf.Infinity;
        Vector3 currentPosition = LocalPlayer.transform.position;

        for (int i = 0; i < objects.Length; i++)
        {
            float distance = (objects[i].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                obj = objects[i];
            }
        }

        return obj;
    }

    public Enemy GetNearestEnemy(Enemy[] enemies = null)
    {
		if (!IsReadyToStart)
		{
			return null;
		}

		if (enemies == null)
        {
            enemies = GameObject.FindGameObjectsWithTag("Entity").Select(o => o.GetComponent<Enemy>()).ToArray();
        }

        Enemy enemy = null;

        float nearestDistance = Mathf.Infinity;
        Vector3 currentPosition = LocalPlayer.transform.position;

        for (int i = 0; i < enemies.Length; i++)
        {
            float distance = (enemies[i].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance && !enemies[i].Controller.IsDead)
            {
                nearestDistance = distance;
                enemy = enemies[i];
            }
        }

        return enemy;
    }
}