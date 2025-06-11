using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Scriptable Objects/Enemy Stats Data", order = 1)]
public class EnemyStatsData : ScriptableObject
{
    public int BaseMaxHealth;
    public float BaseDefensePower;
    public float BaseAttackDistance;
    public float BaseAttackDamage;
    public float BaseMoveSpeed;
}