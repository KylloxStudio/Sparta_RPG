using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Player _player;

    [SerializeField] private Slider _hpBar;
    [SerializeField] private TextMeshProUGUI _hpText;

    [SerializeField] private Slider _damagedEffectBar;

    [SerializeField] private Slider _costBar;
    [SerializeField] private Image _costBarFill;
    [SerializeField] private Color _defaultCostBarColor = new Color32(40, 150, 255, 255);
    [SerializeField] private Color _filledCostBarColor = new Color32(241, 26, 26, 255);

    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private TextMeshProUGUI _weaponAmmoText;

    [SerializeField] private Image _specialSkillIcon;
    [SerializeField] private Image _specialSkillIconMask;
    [SerializeField] private TextMeshProUGUI _specialSkillKeyText;

    [SerializeField] private Image _exSkillIcon;
    [SerializeField] private TextMeshProUGUI _exSkillKeyText;

    [SerializeField] private Color _enableColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color _disableColor = new Color32(188, 188, 188, 128);

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Singleton<InGameManager>.Instance().IsReadyToStart);

        _player = Singleton<InGameManager>.Instance().LocalPlayer;

        _hpBar.value = 1f;
        _damagedEffectBar.value = 1f;
        _costBar.value = 0f;

        _player.Weapon.OnAmmoEvent.AddListener(UpdateAmmo);
        _weaponNameText.color = _player.Weapon.Info.MainColor;
        _weaponNameText.text = _player.Weapon.Info.Name;
        _weaponIcon.sprite = _player.Weapon.Info.Icon;
        _weaponAmmoText.text = $"{_player.Weapon.Info.CurAmmo} / {_player.Weapon.Info.MaxAmmo}";

        _specialSkillIcon.sprite = _player.Weapon.Info.SpecialSkillIcon;
        _specialSkillIconMask.sprite = _player.Weapon.Info.SpecialSkillIcon;
        _exSkillIcon.sprite = _player.Weapon.Info.ExSkillIcon;

        StartCoroutine(HandleHpBar());
        StartCoroutine(HandleCostBar());
        StartCoroutine(HandleSpecialSkill());
        StartCoroutine(HandleExSkill());

        yield break;
    }

    private void UpdateAmmo(int cur, int max)
    {
        _weaponAmmoText.text = $"{cur} / {max}";
    }

    private IEnumerator HandleHpBar()
    {
        while (Singleton<GameManager>.Instance().IsInGame)
        {
            _hpText.text = _player.Stats.Health + " / " + _player.Stats.MaxHealth;
            _hpBar.value = Mathf.Lerp(_hpBar.value, (float)_player.Stats.Health / (float)_player.Stats.MaxHealth, Time.deltaTime * 10f);
            UpdateDamagedEffectBar();

            yield return null;
        }

        yield break;
    }

    private void UpdateDamagedEffectBar()
    {
        if (_player.Controller.IsDamaged)
        {
            return;
        }

        if (Mathf.Abs(_damagedEffectBar.value - _hpBar.value) > 0.0001f)
        {
            _damagedEffectBar.value = Mathf.Lerp(_damagedEffectBar.value, _hpBar.value, Time.deltaTime * 10f);
        }
    }

    private IEnumerator HandleCostBar()
    {
        while (Singleton<GameManager>.Instance().IsInGame)
        {
            while (_player.Stats.Cost >= _player.Stats.ExSkillCost)
            {
                _costBar.value = 1f;
                _costBarFill.color = _filledCostBarColor;
                yield return new WaitForSeconds(0.5f);
                _costBarFill.color = _defaultCostBarColor;
                yield return new WaitForSeconds(0.5f);
            }

            _costBar.value = Mathf.Lerp(_costBar.value, _player.Stats.Cost / _player.Stats.ExSkillCost, Time.deltaTime * 10f);
            _costBarFill.color = _defaultCostBarColor;

            yield return null;
        }

        yield break;
    }

    private IEnumerator HandleSpecialSkill()
    {
        while (Singleton<GameManager>.Instance().IsInGame)
        {
            if (_player.Controller.IsSpecialSkillCooldown)
            {
                _specialSkillKeyText.color = _disableColor;
            }
            else
            {
                _specialSkillKeyText.color = _enableColor;
            }

            _specialSkillIconMask.fillAmount = Mathf.Lerp(_specialSkillIconMask.fillAmount, _player.Controller.SpecialSkillTimer / _player.Stats.SpecialSkillCooltime, Time.deltaTime * 10f);

            yield return null;
        }

        yield break;
    }

    private IEnumerator HandleExSkill()
    {
        while (Singleton<GameManager>.Instance().IsInGame)
        {
            if (_player.Stats.Cost >= _player.Stats.ExSkillCost)
            {
                _exSkillIcon.color = _enableColor;
                _exSkillKeyText.color = _enableColor;
            }
            else
            {
                _exSkillIcon.color = _disableColor;
                _exSkillKeyText.color = _disableColor;
            }

            yield return null;
        }

        yield break;
    }
}
