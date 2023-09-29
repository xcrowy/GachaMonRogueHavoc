using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Character
{
    #region Data Reference
    public UnitData unitData;
    #endregion

    #region Stats References
    public string CharacterName { get; private set; }
    public int HealthPoint { get; private set; }
    public int Energy { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public float CritRate { get; private set; }
    public int Constant { get; private set; }
    #endregion

    public override IEnumerator Action()
    {
        //Implement Unit actions -> Use UI for Options like Attack/End Turn
        //End with yield return new WaitForSeconds
        print("Unit does this kind of action.");
        yield return new WaitForSeconds(2f);
    }

    public override void Initialize()
    {
        CharacterName = unitData.CharacterName;
        HealthPoint = unitData.HealthPoint;
        Energy = unitData.Energy;
        Attack = unitData.Attack;
        Defense = unitData.Defense;
        Speed = unitData.Speed;
        CritRate = unitData.CritRate;
        Constant = unitData.Constant;
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
}
