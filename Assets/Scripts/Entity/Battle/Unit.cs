using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public override void Initialize(BattleSystem BattleSystem)
    {
        this.BattleSystem = BattleSystem;

        SetLevel(unitData.Level);
        SetCharacterName(unitData.CharacterName);
        SetMaxHealthPoint(unitData.HealthPoint);
        SetCurrentHealthPoint(unitData.HealthPoint);
        SetMaxEnergy(unitData.Energy);
        SetCurrentEnergy(unitData.Energy);
        SetAttack(unitData.Attack);
        SetDefense(unitData.Defense);
        SetSpeed(unitData.Speed);
        SetCritRate(unitData.CritRate);

        // Set Attack and End Turn to non-interactable at the start of combat
        BattleSystem.TogglePlayerChoiceButtonInteractability(false);
        ActionSelected = false;

        // Disable Event Trigger
        BattleSystem.ToggleEventTriggerForTargetSelection(false);

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
        if(this.CharacterName == BattleSystem.PartyController.partyMembers[0].CharacterName)
        {
            var presetAbilityPanel = BattleSystem.abilityPanelParent.GetChild(0);
            var presetAbility = presetAbilityPanel.GetChild(0);

            for (int i = 1; i < AbilitySet.Count; i++)
            {
                Transform ability = Instantiate(presetAbility, presetAbilityPanel);
                ability.name = $"{presetAbility}_{i}";
            }


            for (int i = 0; i < presetAbilityPanel.childCount; i++)
            {
                GetAbilities = presetAbilityPanel.GetChild(i);
                GetAbilities.GetComponent<Button>().onClick.AddListener(delegate { BattleSystem.OnSelectAbility(); });
                GetAbilities.GetChild(0).GetComponent<TextMeshProUGUI>().text = AbilitySet[i].AbilityBase.abilityName;
                GetAbilities.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{AbilitySet[i].AbilityBase.description} \nCost: {AbilitySet[i].EnergyUsage}";
            }
        }
        else
        {
            GameObject otherAbilityPanel = Instantiate(BattleSystem.abilityPanelParent.GetChild(0).gameObject, BattleSystem.abilityPanelParent);

            for (int i = otherAbilityPanel.transform.childCount; i > AbilitySet.Count; i--)
            {
                Destroy(otherAbilityPanel.transform.GetChild(i - 1).gameObject);
                otherAbilityPanel.transform.GetChild(i - 1).SetParent(null);
            }

            var ability = otherAbilityPanel.transform.GetChild(0);

            for (int i = otherAbilityPanel.transform.childCount; i < AbilitySet.Count; i++)
            {
                Transform getAbility = Instantiate(ability, otherAbilityPanel.transform);
                ability.name = $"{getAbility}_{i}";
            }


            for (int i = 0; i < otherAbilityPanel.transform.childCount; i++)
            {
                GetAbilities = otherAbilityPanel.transform.GetChild(i);
                GetAbilities.GetComponent<Button>().onClick.AddListener(delegate {  BattleSystem.OnSelectAbility(); });
                GetAbilities.GetChild(0).GetComponent<TextMeshProUGUI>().text = AbilitySet[i].AbilityBase.abilityName;
                GetAbilities.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{AbilitySet[i].AbilityBase.description} \nCost: {AbilitySet[i].EnergyUsage}";
            }
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

    public override float CalculateDamage(float baseDamage, float abilityDamage, float defense, float critRate)
    {
        float calculatedDefense = (float)defense / 100f;

        float totalDamage = (baseDamage + abilityDamage) * calculatedDefense;

        totalDamage *= Random.Range(0f, 1f) < critRate ? 2f : 1f;

        totalDamage = Mathf.Max(totalDamage, 0);

        return totalDamage;
    }
}
