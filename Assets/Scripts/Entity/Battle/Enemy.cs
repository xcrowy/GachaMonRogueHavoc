using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : Character
{
    #region Data Reference
    public EnemyData enemyData;
    #endregion

    public override IEnumerator Action()
    {
        // Implement enemy AI actions
        AttackUnit();
        yield return new WaitForSeconds(2f);
    }

    public override void Initialize()
    {
        SetCharacterName(enemyData.CharacterName);
        SetMaxHealthPoint(enemyData.HealthPoint);
        SetCurrentHealthPoint(enemyData.HealthPoint);
        SetAttack(enemyData.Attack);
        SetDefense(enemyData.Defense);
        SetSpeed(enemyData.Speed);
        SetCritRate(enemyData.CritRate);
    }

    public override bool IsDead()
    {
        if (CurrentHealthPoint <= 0)
            return true;
        return false;
    }

    public override void TakeDamageFrom(Character character, int abilityDamage)
    {
        float totalDamage = CalculateDamage(character.Attack, abilityDamage, Defense, character.CritRate);
        print($"Total Damage: {totalDamage}");

        ModifyCurrentHealthPoint(-Mathf.RoundToInt(totalDamage));
    
    }

    public override float CalculateDamage(float baseDamage, float abilityDamage, float defense, float critRate)
    {
        float calculatedDefense = defense / 100f >= 1 ? ((defense / 100f) - 1f) : (1f - (defense / 100f));
        float totalDamage = (baseDamage + abilityDamage) * calculatedDefense;

        totalDamage *= Random.Range(0f, 1f) < critRate ? 2f : 1f;

        totalDamage = Mathf.Max(totalDamage, 0);

        return totalDamage;
    }

    public void AttackUnit()
    {
        // Implement enemy attack logic
        print("Enemy does this action");
    }


}
