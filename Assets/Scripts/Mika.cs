using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Mika : MonoBehaviour
{
    private Player _player;

    [SerializeField] private int _meteorDamage;
    [SerializeField] private int _explosionDamage;

    private int _exShootCount;
    private Coroutine _exSkillCoroutine;

    [SerializeField] private AudioSource _specialMeteorSound;
    [SerializeField] private AudioSource _exSmokeSound;
    [SerializeField] private AudioSource _exExplosionSound;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void SpecialStart()
    {
        _specialMeteorSound.Play();
    }

    public void SpecialShoot()
    {
        _player.Weapon.Shoot();
    }

    public void SpecialMeteor()
    {
        StartCoroutine(SpecialMeteorCoroutine());
    }

    private IEnumerator SpecialMeteorCoroutine()
    {
        _player.Weapon.Shoot();
        yield return new WaitForSeconds(0.1f);

        if (_player.Weapon.Victim == null)
        {
            yield break;
        }

        //GameObject particle = Singleton<ObjectPoolManager>.Instance().Get("HolyHit");
        //particle.transform.SetPositionAndRotation(_player.Weapon.Victim.transform.position, _player.Weapon.Victim.transform.rotation);

        if (_player.Weapon.Victim.CompareTag("Enemy"))
        {
            Enemy enemy = _player.Weapon.Victim.GetComponent<Enemy>();
            int damage = DamageCalculator.CalcDamage(_player.Weapon.Victim, _meteorDamage, true);
            enemy.Controller.OnDamaged(_player.gameObject, damage);
        }

        yield break;
    }

    public void ExStart()
    {
        _exShootCount = 0;
        _exSkillCoroutine = null;

        _player.Controller.RotateTo(Vector3.Scale(Singleton<CameraManager>.Instance().MainCamera.transform.forward, new Vector3(1f, 0f, 1f)));
    }

    public void ExShoot()
    {
        if (!_player.Controller.IsUsingExSkill)
        {
            return;
        }

        _player.Weapon.Shoot();

        _exShootCount++;
        ExExplode();
    }

    public void ExExplode()
    {
        if (_exSkillCoroutine != null)
        {
            StopCoroutine(_exSkillCoroutine);
            _exSkillCoroutine = null;
        }

        _exSkillCoroutine = StartCoroutine(ExExplodeCoroutine());
    }

    public IEnumerator ExExplodeCoroutine()
    {
        yield return new WaitForSeconds((float)(11 - _exShootCount) * 0.5f);
        if (_player.Weapon.Victim == null)
        {
            yield break;
        }

        _exSmokeSound.Play();
        yield return new WaitForSeconds(1.3f);

        if (_player.Weapon.Victim == null)
        {
            yield break;
        }

        _exExplosionSound.Play();
        //GameObject particle = Singleton<ObjectPoolManager>.Instance().Get("HolyHit");
        //particle.transform.SetPositionAndRotation(_player.Weapon.Victim.transform.position, _player.Weapon.Victim.transform.rotation);

        if (_player.Weapon.Victim.CompareTag("Enemy"))
        {
            Enemy enemy = _player.Weapon.Victim.GetComponent<Enemy>();

            int damage = Mathf.Min(_explosionDamage * enemy.Stats.MaxHealth / Mathf.Abs(enemy.Stats.Health - enemy.Stats.MaxHealth + 1), _explosionDamage * 2);

            enemy.Controller.OnDamaged(_player.gameObject, damage);
        }

        yield break;
    }

    public void ExEnd()
    {
        
    }
}