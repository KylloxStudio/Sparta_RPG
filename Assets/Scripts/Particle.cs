using System;
using System.Collections;
using UnityEngine;

public class Particle : PoolObject
{
    [SerializeField] private float _destroyTime;

    private void OnEnable()
    {
        StartCoroutine(OnEnabled());
    }

    private IEnumerator OnEnabled()
    {
        yield return new WaitForSeconds(_destroyTime);

        ReleaseObject();

        yield break;
    }
}