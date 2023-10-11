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

    public override void Initialize(BattleSystem BattleSystem)
    {
        SetCharacterName(enemyData.CharacterName);
        SetLevel(enemyData.Level);
        SetMaxHealthPoint(enemyData.HealthPoint);
        SetCurrentHealthPoint(enemyData.HealthPoint);
        SetAttack(enemyData.Attack);
        SetDefense(enemyData.Defense);
        SetSpeed(enemyData.Speed);
        SetCritRate(enemyData.CritRate);

        this.BattleSystem = BattleSystem;

        AbilitySet = new();
        List<Ability> abilityCollection = new();

        foreach (LearnableAbility ability in enemyData.LearnableAbilities)
        {
            if (ability.Level <= Level)
            {
                abilityCollection.Add(new Ability(ability.Base));
            }
        }

        while (AbilitySet.Count < 4 && abilityCollection.Count > 0)
        {
            int randomAbility = Random.Range(0, abilityCollection.Count);
            AbilitySet.Add(abilityCollection[randomAbility]);
            abilityCollection.RemoveAt(randomAbility);
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

        float randomChance = Random.Range(0f, 1f);

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

        if (randomChance < 0.5f) // 50% chance of hitting a random unit
        {
            aliveUnits.Sort((x, y) => y.Defense.CompareTo(x.Defense));
            aliveUnits.Reverse();

            return aliveUnits.FirstOrDefault();
        }
        else
        {
            int count = aliveUnits.Count;
            int last = count - 1;

            for (int i = 0; i < last; ++i)
            {
                int rng = Random.Range(i, count);
                var tmp = aliveUnits[i];
                aliveUnits[i] = aliveUnits[rng];
                aliveUnits[rng] = tmp;
            }

            return aliveUnits.FirstOrDefault();
        }
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
