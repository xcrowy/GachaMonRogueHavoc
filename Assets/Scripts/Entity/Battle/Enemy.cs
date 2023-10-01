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
        CharacterName = enemyData.CharacterName;
        MaxHealthPoint = enemyData.HealthPoint;
        CurrentHealthPoint = MaxHealthPoint;
        Attack = enemyData.Attack;
        Defense = enemyData.Defense;
        Speed = enemyData.Speed;
        CritRate = enemyData.CritRate;
        Constant = enemyData.Constant;
    }

    public override bool IsDead()
    {
        if (CurrentHealthPoint <= 0)
            return true;
        return false;
    }

    public override void TakeDamageFrom(int damage)
    {
        int damageTaken = damage * (1 - (Defense / Defense + Constant));
        CurrentHealthPoint -= damageTaken;
    }

    public void AttackUnit()
    {
        // Implement enemy attack logic
        print("Enemy does this action");
    }

}
