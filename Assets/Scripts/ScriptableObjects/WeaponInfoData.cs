using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfoData", menuName = "Scriptable Objects/Weapon Info Data", order = 1)]
public class WeaponInfoData : ScriptableObject
{
    public string Name;
    public WeaponType Type;
    public Color32 MainColor;
    public Sprite Icon;
    public Sprite SpecialSkillIcon;
    public Sprite ExSkillIcon;

    public float BaseAttackDistance;
    public float BaseAttackDamage;
    public int BaseMaxAmmo;
    public float BaseCostResilience;
}