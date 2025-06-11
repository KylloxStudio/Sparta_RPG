using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Scriptable Objects/Player Stats Data", order = 1)]
public class PlayerStatsData : ScriptableObject
{
    public int BaseMaxHealth;
    public float BaseDefensePower;
    public float BaseMoveSpeed;
    public float SpecialSkillCooltime;
    public float ExSkillCost;
}