using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BulletTrail : PoolObject
{
    private TrailRenderer _trail;

    private Bullet _bullet;
    private Vector3 _dir;

    private bool _isStopped;
    [SerializeField] private float _speed;

    private void Awake()
    {
        _trail = GetComponent<TrailRenderer>();
    }

    public void Initialize(Bullet bullet, Vector3 dir)
    {
        _bullet = bullet;
        _dir = dir;

        _isStopped = false;

        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        transform.position = _bullet.transform.position;
        transform.forward = _dir;
        _trail.material.color = new Color(_trail.material.color.r, _trail.material.color.g, _trail.material.color.b, 0.75f);
        _trail.emitting = true;

        yield return new WaitForEndOfFrame();

        gameObject.SetActive(true);

        float timer = 0.5f;
        while (timer > 0f && !_isStopped)
        {
            transform.position += _speed * Time.deltaTime * transform.forward;
            timer -= Time.deltaTime;

            yield return null;
        }

        if (!_isStopped)
        {
            DestroyTrail(fadeTime: 0.2f);
        }

        yield break;
    }

    public void DestroyTrail(float delay = 0f, float fadeTime = 0f)
    {
        StartCoroutine(DestroyCoroutine(delay, fadeTime));
    }

    private IEnumerator DestroyCoroutine(float delay, float fadeTime)
    {
        if (delay != 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        _isStopped = true;
        _trail.emitting = false;

        if (fadeTime != 0f)
        {
            Color color = _trail.material.color;
            float alpha = _trail.material.color.a;
            float percent = 0f;

            while (percent < 1f)
            {
                percent += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(alpha, 0f, percent);
                _trail.material.color = color;

                yield return null;
            }
        }

        transform.position = _bullet.transform.position;
        transform.forward = _dir;

        yield return new WaitForEndOfFrame();
        
        ReleaseObject();

        yield break;
    }
}
