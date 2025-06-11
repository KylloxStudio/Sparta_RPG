using UnityEngine;

public static class DamageCalculator
{
    public static int CalcDamage(GameObject victim, float attackDamage, bool ignoreDefense)
    {
        float stability = Random.Range(0.8f, 1f);
        float defensed = 1f;

        if (victim.TryGetComponent(out Player player))
        {
            defensed = 1f / (player.Stats.DefensePower + 111f) * 111f;
        }
        else if (victim.TryGetComponent(out Enemy enemy))
        {
            defensed = 1f / (enemy.Stats.DefensePower + 111f) * 111f;
        }

        float damagePercent = ignoreDefense ? stability : stability * defensed;
        float damage = attackDamage * (Mathf.Round(damagePercent * 100f) / 100f);

        return Mathf.RoundToInt(damage);
    }
}