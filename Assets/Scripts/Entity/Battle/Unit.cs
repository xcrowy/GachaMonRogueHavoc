using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : Character
{
    #region Data Reference
    public UnitData unitData;
    #endregion

    #region Collection/Party References
    public bool inCollection = false;
    public bool inParty = false;
    #endregion

    #region Getters/Setters
    public bool ActionSelected { get; set; }
    public BattleSystem BattleSystem { get; private set; }
    public List<Ability> AbilitySet { get; private set; }
    public Transform GetAbilities { get; private set; }
    #endregion

    public override void Initialize()
    {
        SetLevel(unitData.Level);
        SetCharacterName(unitData.CharacterName);
        SetMaxHealthPoint(unitData.HealthPoint);
        SetCurrentHealthPoint(MaxHealthPoint);
        SetMaxEnergy(unitData.Energy);
        SetCurrentEnergy(unitData.Energy);
        SetAttack(unitData.Attack);
        SetDefense(unitData.Defense);
        SetSpeed(unitData.Speed);
        SetCritRate(unitData.CritRate);
        SetConstant(unitData.Constant);

        BattleSystem = FindObjectOfType<BattleSystem>();

        // Set Attack and End Turn to non-interactable at the start of combat
        BattleSystem.TogglePlayerChoiceButtonInteractability(false);
        ActionSelected = false;

        // Disable Event Trigger
        BattleSystem.ToggleEventTrigger(false);

        // Generate Abilities for Unit
        AbilitySet = new();
        foreach (LearnableAbility ability in unitData.LearnableAbilities)
        {
            if (ability.Level <= Level)
                AbilitySet.Add(new Ability(ability.Base));

            if (AbilitySet.Count >= 4)
                break;
        }
        InitializeAbility();
    }

    private void InitializeAbility()
    {
        Transform abilityRef = BattleSystem.abilityPanel.transform.GetChild(0);

        for (int i = 1; i < AbilitySet.Count; i++)
            Instantiate(abilityRef, BattleSystem.abilityPanel.transform);

        for (int i = 0; i < BattleSystem.abilityPanel.transform.childCount; i++)
        {
            GetAbilities = BattleSystem.abilityPanel.transform.GetChild(i);
            GetAbilities.GetChild(0).GetComponent<TextMeshProUGUI>().text = AbilitySet[i].AbilityBase.abilityName;
        }

        //TODO: On Mouse Hover over ability button -> Display energy cost + description
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

    public override IEnumerator Action()
    {
        BattleSystem.TogglePlayerChoiceButtonInteractability(true);

        yield return WaitForPlayerAction();

        BattleSystem.TogglePlayerChoiceButtonInteractability(false);

        yield return new WaitForSeconds(2f);

        ActionSelected = false;
    }

    private IEnumerator WaitForPlayerAction()
    {
        yield return new WaitForSeconds(1f);

        while (!ActionSelected)
            yield return null;
    }
}
