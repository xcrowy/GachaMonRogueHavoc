using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    #region Stats References
    public int Level { get; private set; }
    public string CharacterName { get; private set; }
    public int MaxHealthPoint { get; private set; }
    public int CurrentHealthPoint { get; private set; }
    public int MaxEnergy { get; private set; }
    public int CurrentEnergy { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public float CritRate { get; private set; }
    #endregion

    #region Abstract
    public abstract IEnumerator Action();
    public abstract void Initialize();
    public abstract void TakeDamageFrom(Character character, int damage);
    public abstract float CalculateDamage(float baseDamage, float abilityDamage, float defense, float critMultiplier);
    public abstract bool IsDead();
    #endregion

    #region Setters
    public void SetLevel(int val) => Level = val;
    public void SetCharacterName(string val) => CharacterName = val;
    public void SetMaxHealthPoint(int val) => MaxHealthPoint = val;
    public void SetCurrentHealthPoint(int val) => CurrentHealthPoint = val;
    public void SetMaxEnergy(int val) => MaxEnergy = val;
    public void SetCurrentEnergy(int val) => CurrentEnergy = val;
    public void SetAttack(int val) => Attack = val;
    public void SetDefense(int val) => Defense = val;
    public void SetSpeed(int val) => Speed = val;
    public void SetCritRate(float val) => CritRate = val;
    #endregion

    public void ModifyMaxHealthPoint(int val) => MaxHealthPoint += val;
    public void ModifyCurrentHealthPoint(int val)
    {
        CurrentHealthPoint += val;
        
        if (CurrentHealthPoint <= 0)
            CurrentHealthPoint = 0;

        if (CurrentHealthPoint > MaxHealthPoint)
            CurrentHealthPoint = MaxHealthPoint;
    }
    public void ModifyEnergy(int val)
    {
        CurrentEnergy += val;

        if (CurrentEnergy > MaxEnergy)
            CurrentEnergy = MaxEnergy;
    }
    public bool HasEnoughEnergy(Unit unit, int index) => unit.CurrentEnergy >= unit.AbilitySet[index].EnergyUsage;
    public bool HasMaxEnergy() => CurrentEnergy >= MaxEnergy;
}
