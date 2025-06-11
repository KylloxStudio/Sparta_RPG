using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class Bullet : PoolObject
{
    private GameObject _owner;

    private Ray _ray;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _layerMask;

    private float _attackDamage;
    private bool _ignoreDefense;

    public void Initialize(GameObject owner, Ray ray, float attackDamage, bool ignoreDefense = false)
    {
        _owner = owner;

        _ray = ray;

        _attackDamage = attackDamage;
        _ignoreDefense = ignoreDefense;

        StartCoroutine(ShootBulletRayHit());
    }

    public IEnumerator ShootBulletRayHit()
    {
        if (_owner == null)
        {
            yield break;
        }

        BulletTrail bulletTrail = Singleton<ObjectPoolManager>.Instance().Get("BulletTrail").GetComponent<BulletTrail>();
        bulletTrail.Initialize(this, _ray.direction);

        float timer = 0.5f;
        while (gameObject != null && gameObject.activeSelf)
        {
            if (timer <= 0f)
            {
                break;
            }

            if (Physics.Raycast(_ray, out RaycastHit hit, _distance, _layerMask.value))
            {
                CheckBulletRayHit(hit);
                break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        bulletTrail.DestroyTrail(delay: 0.05f, fadeTime: 0.1f);
        ReleaseObject();

        yield break;
    }

    public bool CheckBulletRayHit(RaycastHit hit)
    {
        if (hit.collider.gameObject == _owner)
        {
            return false;
        }

        if (hit.collider.CompareTag("Structure"))
        {
            //GameObject bulletMark = Singleton<ObjectPoolManager>.Instance().Get("BulletMark");
            //bulletMark.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
            return true;
        }

        if (hit.collider.CompareTag("Enemy"))
        {
            if (_owner.TryGetComponent(out Player player))
            {
                player.Weapon.Victim = hit.collider.gameObject;
            }

            GameObject particle = Singleton<ObjectPoolManager>.Instance().Get("NormalHit");
            particle.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));

            int damage = DamageCalculator.CalcDamage(hit.collider.gameObject, _attackDamage, _ignoreDefense);

            hit.collider.GetComponent<Enemy>().Controller.OnDamaged(_owner, damage);
            return true;
        }

        return false;
    }
}
