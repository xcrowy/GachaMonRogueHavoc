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
        SetConstant(enemyData.Constant);
    }

    public override bool IsDead()
    {
        if (CurrentHealthPoint <= 0)
            return true;
        return false;
    }

    public override void TakeDamageFrom(int damage)
    {
        float constantDef = Defense + Constant;
        float calculateDefense = Defense / constantDef;
        
        float damageTaken = damage * (1 - calculateDefense);
        ModifyCurrentHealthPoint(-Mathf.RoundToInt(damageTaken));
    }

    public void AttackUnit()
    {
        // Implement enemy attack logic
        print("Enemy does this action");
    }

}
