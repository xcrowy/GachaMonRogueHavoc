using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Enemy : Character
{
    #region Data Reference
    public EnemyData enemyData;
    #endregion

    #region Getters/Setters
    public bool ActionSelected { get; private set; }
    public BattleSystem BattleSystem { get; private set; }
    public List<Ability> AbilitySet { get; private set; }
    #endregion

    public override IEnumerator Action()
    {
        yield return AttackUnit();

        yield return new WaitForSeconds(2f);

        ActionSelected = false;
    }

    public override void Initialize()
    {
        SetCharacterName(enemyData.CharacterName);
        SetLevel(enemyData.Level);
        SetMaxHealthPoint(enemyData.HealthPoint);
        SetCurrentHealthPoint(enemyData.HealthPoint);
        SetAttack(enemyData.Attack);
        SetDefense(enemyData.Defense);
        SetSpeed(enemyData.Speed);
        SetCritRate(enemyData.CritRate);

        BattleSystem = FindObjectOfType<BattleSystem>();

        AbilitySet = new();
        foreach (LearnableAbility ability in enemyData.LearnableAbilities)
        {
            // TODO: Randomize abilities to add into list if more than 4 abilities are the same learnable level
            if (ability.Level <= Level)
                AbilitySet.Add(new Ability(ability.Base));

            if (AbilitySet.Count >= 4)
                break;
        }
    }

    public override bool IsDead()
    {
        if (CurrentHealthPoint <= 0)
        {
            return true;
        }
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
        float calculatedDefense = (float)defense / 100f;
        float totalDamage = (baseDamage + abilityDamage) * calculatedDefense;

        totalDamage *= Random.Range(0f, 1f) < critRate ? 2f : 1f;

        totalDamage = Mathf.Max(totalDamage, 0);

        return totalDamage;
    }

    public IEnumerator AttackUnit()
    {
        yield return new WaitForSeconds(2f);

        Unit target = FindWeakestUnit();
        Ability ability = FindStrongestAbility();

        ApplyDamageToUnit(target, ability);
        BattleSystem.UpdateUnitStats(target);
        ActionSelected = true;

        while (!ActionSelected)
            yield return null;
    }

    private Unit FindWeakestUnit()
    {
        List<Unit> aliveUnits = new();

        for (int i = 0; i < BattleSystem.unitPosition.transform.childCount; i++)
        {
            Unit currentUnit = BattleSystem.unitPosition.transform.GetChild(i).GetComponent<Unit>();
            if (currentUnit.IsDead())
            {
                continue;
            }
            else
            {
                aliveUnits.Add(currentUnit);
            }
        }

        aliveUnits.Sort((x, y) => y.Defense.CompareTo(x.Defense));
        aliveUnits.Reverse();

        return aliveUnits.FirstOrDefault();
    }

    private Ability FindStrongestAbility()
    {
        if (AbilitySet.Count == 1)
        {
            return AbilitySet.First();
        }

        AbilitySet.Sort((x, y) => y.AbilityBase.damage.CompareTo(x.AbilityBase.damage));
        return AbilitySet.FirstOrDefault();
    }

    private void ApplyDamageToUnit(Unit unit, Ability ability) => unit.TakeDamageFrom(this, ability.AbilityBase.damage);

}
