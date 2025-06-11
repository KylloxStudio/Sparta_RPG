using UnityEngine;
using UnityEngine.UI;

public class Enemy : PoolObject
{
    [SerializeField] private EnemyStatsData _statsData;

    public EnemyStats Stats { get; private set; }
    public EnemyAIController Controller { get; private set; }

    [SerializeField] private Slider _floatingHpBar;
    [SerializeField] private Slider _damagedEffectBar;

    private void Awake()
    {
        Controller = GetComponent<EnemyAIController>();
    }

    private void OnEnable()
    {
        if (_statsData == null)
        {
            Debug.LogError("enemy stats data is null.");
            return;
        }

        Stats = new EnemyStats(_statsData);
    }

    private void Update()
    {
        _floatingHpBar.value = Mathf.Lerp(_floatingHpBar.value, (float)Stats.Health / (float)Stats.MaxHealth, Time.deltaTime * 10f);
        UpdateDamagedEffect();
    }

    private void UpdateDamagedEffect()
    {
        if (Controller.IsDamaged)
        {
            return;
        }

        if (Mathf.Abs(_damagedEffectBar.value - _floatingHpBar.value) > 0.0001f)
        {
            _damagedEffectBar.value = Mathf.Lerp(_damagedEffectBar.value, _floatingHpBar.value, Time.deltaTime * 10f);
        }
    }

    private void LateUpdate()
    {
        _floatingHpBar.transform.parent.LookAt(_floatingHpBar.transform.parent.position + Singleton<CameraManager>.Instance().MainCamera.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
        _floatingHpBar.transform.parent.Rotate(0f, 180f, 0f);
        _floatingHpBar.transform.parent.rotation = Quaternion.Euler(Vector3.Scale(_floatingHpBar.transform.parent.rotation.eulerAngles, new Vector3(0f, 1f, 0f)));
    }
}