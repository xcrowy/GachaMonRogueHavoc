using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unit : Character
{
    #region Data Reference
    public UnitData unitData;
    #endregion

    #region Unit UI References
    public TextMeshProUGUI unitName, unitHealthPoint, unitEnergyPoint, unitHealthPointStat, unitAtkStat, unitDefStat, unitSpdStat, unitCritStat;
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
        GetComponent<Image>().sprite = unitData.RightSprite;
        CharacterName = unitData.CharacterName;
        MaxHealthPoint = unitData.HealthPoint;
        CurrentHealthPoint = MaxHealthPoint;
        MaxEnergy = unitData.Energy;
        CurrentEnergy = MaxEnergy;
        Attack = unitData.Attack;
        Defense = unitData.Defense;
        Speed = unitData.Speed;
        CritRate = unitData.CritRate;
        Constant = unitData.Constant;

        unitName.text = CharacterName;
        unitHealthPoint.text = $"{CurrentHealthPoint}/{MaxHealthPoint}";
        unitEnergyPoint.text = $"{CurrentEnergy}/{MaxEnergy}";
        unitHealthPointStat.text = $"HP: {MaxHealthPoint}";
        unitAtkStat.text = $"ATK: {Attack}";
        unitDefStat.text = $"DEF: {Defense}";
        unitSpdStat.text = $"SPD: {Speed}";
        unitCritStat.text = $"CRIT: {CritRate}";
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
}
