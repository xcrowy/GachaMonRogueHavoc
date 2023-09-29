using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    #region Data Reference
    public EnemyData enemyData;
    #endregion

    #region Stats References
    public string CharacterName { get; private set; }
    public int HealthPoint { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public float CritRate { get; private set; }
    public int Constant { get; private set; }
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
        HealthPoint = enemyData.HealthPoint;
        Attack = enemyData.Attack;
        Defense = enemyData.Defense;
        Speed = enemyData.Speed;
        CritRate = enemyData.CritRate;
        Constant = enemyData.Constant;
    }

    public override bool IsDead()
    {
        if (HealthPoint <= 0)
            return true;
        return false;
    }

    public override void TakeDamageFrom(int damage)
    {
        int damageTaken = damage * (1 - (Defense / Defense + Constant));
        HealthPoint -= damageTaken;
    }

    public void AttackUnit()
    {
        // Implement enemy attack logic
        print("Enemy does this action");
    }

}
