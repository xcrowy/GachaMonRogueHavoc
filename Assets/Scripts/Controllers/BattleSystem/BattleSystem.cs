using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class BattleSystem : MonoBehaviour
{
    #region Unit References
    [Header("Unit")]
    public VerticalLayoutGroup unitPosition;
    #endregion

    #region Unit UI References
    public Transform unitPanel;
    public TextMeshProUGUI unitLevel, unitName, unitHealthPoint, unitEnergyPoint, unitHealthPointStat, unitAtkStat, unitDefStat, unitSpdStat, unitCritStat;
    #endregion

    #region Enemy References
    [Header("Enemy")]
    public VerticalLayoutGroup enemyPanel;
    public VerticalLayoutGroup enemyPosition;
    #endregion

    #region Turn-Based References
    private Queue<Character> turnQueue = new();
    private Character currentCharacter;
    #endregion

    #region Getters/Setters
    public List<Character> Characters { get; private set; }
    public PartyController PartyController { get; private set; }
    public EnemyHost EnemyPartyController { get; private set; }
    public Transform SelectedTarget { get; private set; }
    public Transform SelectedAbility { get; private set; }
    public List<Button> SelectTargetButtons { get; private set; }
    public Slider UnitHealthPointSlider { get; private set; }
    public Slider UnitEnergySlider { get; private set; }
    public Slider EnemyHealthPointSlider { get; private set; }
    #endregion

    #region Battle UI References
    [Header("Battle Options")]
    public Button attackButton;
    public Button endButton;
    public GameObject abilityPanel;
    #endregion

    private void Start()
    {
        Characters = new();
        PartyController = FindObjectOfType<PartyController>();
        EnemyPartyController = FindObjectOfType<EnemyHost>();

        SelectTargetButtons = new();
        attackButton.onClick.AddListener(OnAttack);
        endButton.onClick.AddListener(OnEndTurn);

        BattleSetup();
        InitializeCharacters();
        StartCoroutine(StartBattle());
    }

    private void BattleSetup()
    {
        Characters.AddRange(PartyController.partyMembers);
        Characters.AddRange(EnemyPartyController.enemyPartyMembers);

        CheckForSingleUnitBattles();
        CheckForMultipleUnitInBattles();
        CheckForMultipleEnemyInBattles();
    }

    private void InitializeCharacters()
    {
        InitializeHealthPoints();
        InitializeEnergy();

        foreach (Character character in Characters)
            character.Initialize();

        RearrangeTurnBasedOnSpeed();

        foreach (Character character in Characters)
            turnQueue.Enqueue(character);

        currentCharacter = turnQueue.Peek();
        Character checkFirstUnit = turnQueue.FirstOrDefault(x => x.GetComponent<Unit>());

        if (checkFirstUnit is Unit)
            UpdateUnitStats((Unit)checkFirstUnit);

        for (int i = 0; i < enemyPosition.transform.childCount; i++)
            SelectTargetButtons.Add(enemyPosition.transform.GetChild(i).GetComponent<Button>());
    }

    private void InitializeHealthPoints()
    {
        UnitHealthPointSlider = unitPanel.GetChild(2).GetComponent<Slider>();
    }

    private void InitializeEnergy() => UnitEnergySlider = unitPanel.GetChild(3).GetComponent<Slider>();

    private void UpdateUnitStats(Unit unit)
    {
        unitLevel.text = $"Lv. {unit.Level}";
        unitName.text = unit.CharacterName;
        unitHealthPoint.text = $"{unit.CurrentHealthPoint}/{unit.MaxHealthPoint}";
        unitEnergyPoint.text = $"{unit.CurrentEnergy}/{unit.MaxEnergy}";
        unitHealthPointStat.text = $"HP: {unit.MaxHealthPoint}";
        unitAtkStat.text = $"ATK: {unit.Attack}";
        unitDefStat.text = $"DEF: {unit.Defense}";
        unitSpdStat.text = $"SPD: {unit.Speed}";
        unitCritStat.text = $"CRIT: {unit.CritRate}";

        UnitHealthPointSlider.value = (float) unit.CurrentHealthPoint / (float) unit.MaxHealthPoint;
        UnitEnergySlider.value = (float) unit.CurrentEnergy / (float) unit.MaxEnergy;
    }

    private void UpdateEnemyStats(Enemy enemy, int index)
    {
        TextMeshProUGUI enemyName = enemyPanel.transform.GetChild(index).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI enemyHp = enemyPanel.transform.GetChild(index).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

        enemyName.text = enemy.CharacterName;
        enemyHp.text = $"{enemy.CurrentHealthPoint}/{enemy.MaxHealthPoint}";

        EnemyHealthPointSlider = enemyPanel.transform.GetChild(index).GetChild(0).GetComponent<Slider>();
        EnemyHealthPointSlider.value = (float)enemy.CurrentHealthPoint / (float)enemy.MaxHealthPoint;
    }

    private void InitializeUnitSprite(Unit unit, Image unitSprite) => unitSprite.sprite = unit.unitData.RightSprite;

    private void InitializeEnemySprite(Enemy enemy, Image enemySprite) => enemySprite.sprite = enemy.enemyData.LeftSprite;

    private void SetUnitStats(Unit unit)
    {
        unit.SetLevel(unit.unitData.Level);
        unit.SetCharacterName(unit.unitData.CharacterName);
        unit.SetMaxHealthPoint(unit.unitData.HealthPoint);
        unit.SetCurrentHealthPoint(unit.MaxHealthPoint);
        unit.SetMaxEnergy(unit.unitData.Energy);
        unit.SetCurrentEnergy(unit.MaxEnergy);
        unit.SetAttack(unit.unitData.Attack);
        unit.SetDefense(unit.unitData.Defense);
        unit.SetSpeed(unit.unitData.Speed);
        unit.SetCritRate(unit.unitData.CritRate);
        unit.SetConstant(unit.unitData.Constant);
    }

    private void SetEnemyStats(Enemy enemy)
    {
        enemy.SetCharacterName(enemy.enemyData.CharacterName);
        enemy.SetMaxHealthPoint(enemy.enemyData.HealthPoint);
        enemy.SetCurrentHealthPoint(enemy.MaxHealthPoint);
        enemy.SetAttack(enemy.enemyData.Attack);
        enemy.SetDefense(enemy.enemyData.Defense);
        enemy.SetSpeed(enemy.enemyData.Speed);
        enemy.SetCritRate(enemy.enemyData.CritRate);
        enemy.SetConstant(enemy.enemyData.Constant);
    }

    private void CheckForSingleUnitBattles()
    {
        if (PartyController.partyMembers.Count == 1)
        {
            Unit getUnitRef = unitPosition.transform.GetChild(0).GetComponent<Unit>();
            Image unitSpriteRef = unitPosition.transform.GetChild(0).GetComponent<Image>();
            Unit findUnit = (Unit)Characters.Find(x => x.GetComponent<Unit>());
            getUnitRef.unitData = findUnit.GetComponent<Unit>().unitData;

            SetUnitStats(getUnitRef);

            InitializeUnitSprite(findUnit, unitSpriteRef);
        }

        if (EnemyPartyController.enemyPartyMembers.Count == 1)
        {
            Enemy getEnemyRef = enemyPosition.transform.GetChild(0).GetComponent<Enemy>();
            Image enemySpriteRef = enemyPosition.transform.GetChild(0).GetComponent<Image>();
            Enemy findEnemy = (Enemy)Characters.Find(x => x.GetComponent<Enemy>());
            getEnemyRef.enemyData = findEnemy.GetComponent<Enemy>().enemyData;

            SetEnemyStats(getEnemyRef);

            InitializeEnemySprite(findEnemy, enemySpriteRef);
        }
    }

    private void CheckForMultipleUnitInBattles()
    {
        if (PartyController.partyMembers.Count > 1)
        {
            switch (PartyController.partyMembers.Count)
            {
                case 1:
                    unitPosition.padding.top = 300;
                    break;
                case 2:
                    unitPosition.padding.top = 220;
                    break;
                case 3:
                case 4:
                    unitPosition.padding.top = 40;
                    unitPosition.spacing = -350;
                    break;
            }

            Unit getUnitRef = unitPosition.transform.GetChild(0).GetComponent<Unit>();
            for (int i = 1; i < PartyController.partyMembers.Count; i++)
                Instantiate(getUnitRef, unitPosition.transform);

            List<Character> findUnit = Characters.FindAll(x => x.GetComponent<Unit>());

            for (int i = 0; i < unitPosition.transform.childCount; i++)
            {
                unitPosition.transform.GetChild(i).GetComponent<Unit>().unitData = findUnit[i].GetComponent<Unit>().unitData;
                for (int j = 0; j < findUnit.Count; j++)
                {
                    Image unitSpriteRef = unitPosition.transform.GetChild(j).GetComponent<Image>();
                    Unit unit = findUnit[j].GetComponent<Unit>();
                    InitializeUnitSprite(unit, unitSpriteRef);
                }
            }
            for (int i = 0; i < unitPosition.transform.childCount; i++)
                SetUnitStats(unitPosition.transform.GetChild(i).GetComponent<Unit>());
        }
    }

    private void CheckForMultipleEnemyInBattles()
    {
        if (EnemyPartyController.enemyPartyMembers.Count > 1)
        {
            switch (EnemyPartyController.enemyPartyMembers.Count)
            {
                case 1:
                    enemyPosition.padding.top = 300;
                    enemyPanel.padding.top = 120;
                    break;
                case 2:
                    enemyPosition.padding.top = 220;
                    enemyPanel.padding.top = 70;
                    break;
                case 3:
                case 4:
                    enemyPosition.padding.top = 40;
                    enemyPanel.padding.top = 20;
                    break;
            }

            Enemy getEnemyRef = enemyPosition.transform.GetChild(0).GetComponent<Enemy>();
            GameObject getEnemyTextData = enemyPanel.transform.GetChild(0).gameObject;
            
            for (int i = 1; i < EnemyPartyController.enemyPartyMembers.Count; i++)
            {
                Instantiate(getEnemyRef, enemyPosition.transform);
                Instantiate(getEnemyTextData, enemyPanel.transform);
            }

            List<Character> findEnemy = Characters.FindAll(x => x.GetComponent<Enemy>());
            for (int i = 0; i < enemyPosition.transform.childCount; i++)
            {
                enemyPosition.transform.GetChild(i).GetComponent<Enemy>().enemyData = findEnemy[i].GetComponent<Enemy>().enemyData;
                for (int j = 0; j < findEnemy.Count; j++)
                {
                    Image enemySpriteRef = enemyPosition.transform.GetChild(j).GetComponent<Image>();
                    Enemy enemy = findEnemy[j].GetComponent<Enemy>();
                    InitializeEnemySprite(enemy, enemySpriteRef);
                }
                UpdateEnemyStats(findEnemy[i].GetComponent<Enemy>(), i);
            }

            for (int i = 0; i < enemyPosition.transform.childCount; i++)
                SetEnemyStats(enemyPosition.transform.GetChild(i).GetComponent<Enemy>());
        }
    }


    private IEnumerator StartBattle()
    {
        while (!IsBattleOver())
        {
            currentCharacter = turnQueue.Dequeue();
            Debug.Log($"{currentCharacter.CharacterName}'s turn.");

            if (currentCharacter is Unit)
            {
                Unit unitCharacter = currentCharacter.GetComponent<Unit>();

                if (!unitCharacter.HasMaxEnergy())
                    unitCharacter.ModifyEnergy(unitCharacter.unitData.EnergyRegen);

                UpdateUnitStats(unitCharacter);
                for (int i = 0; i < SelectTargetButtons.Count; i++)
                    SelectTargetButtons[i].onClick.AddListener(delegate { OnSelectTarget(unitCharacter); });

                unitCharacter.GetAbilities.GetComponent<Button>().onClick.AddListener(delegate { OnSelectAbility(unitCharacter); });
            }
                
            yield return currentCharacter.Action();
            
            turnQueue.Enqueue(currentCharacter);
            
            yield return new WaitForSeconds(2f);

            if (currentCharacter is Unit)
            {
                for (int i = 0; i < SelectTargetButtons.Count; i++)
                    SelectTargetButtons[i].onClick.RemoveAllListeners();
                currentCharacter.GetComponent<Unit>().GetAbilities.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        Debug.Log("Battle's Over!");
        BattleOutcome();
    }

    private bool IsAllPlayerUnitDead()
    {
        foreach (var character in Characters)
        {
            if (character is Unit && !character.IsDead())
                return false;
            else
                return true;
        }
        return false;
    }

    private bool IsBattleOver()
    {
        bool allEnemiesDefeated = true;
        bool allUnitsDefeated = true;

        foreach (var character in Characters)
        {
            if (character is Enemy && !character.IsDead())
                allEnemiesDefeated = false;

            else if (character is Unit && !character.IsDead())
                allUnitsDefeated = false;
        }
        return allEnemiesDefeated || allUnitsDefeated;
    }

    private void BattleOutcome()
    {
        if (IsAllPlayerUnitDead())
        {
            print("All Units are Dead. You suck.");
        }
        else
        {
            print("You are the chosen one.");
        }
        ///TODO: Disable Battle Interface
    }

    public void OnSelectedTarget(Transform target) => SelectedTarget = target;
    public void OnSelectedAbility(Transform ability) => SelectedAbility = ability;
    private void RearrangeTurnBasedOnSpeed() => Characters.Sort((x, y) => y.Speed.CompareTo(x.Speed));

    #region Button Event Listener
    private void OnAttack()
    {
        print("On Attack");
        abilityPanel.SetActive(true);
    }

    private void OnEndTurn() //TODO
    {
        print("End Turn");
    }

    private void OnSelectTarget(Unit unit)
    {
        print("Pick a target");
        int findSelectedTarget = enemyPosition.transform.Find(SelectedTarget.name).GetSiblingIndex();
        ToggleEventTrigger(false);

        GameObject targetIcon = enemyPosition.transform.GetChild(findSelectedTarget).GetChild(0).gameObject;
        targetIcon.SetActive(!targetIcon);

        Enemy target = enemyPosition.transform.GetChild(findSelectedTarget).GetComponent<Enemy>();

        int findSelectedAbility = abilityPanel.transform.Find(SelectedAbility.name).GetSiblingIndex();

        target.TakeDamageFrom(unit.Attack);

        EnemyHealthPointSlider = enemyPanel.transform.GetChild(findSelectedTarget).GetChild(0).GetComponent<Slider>();
        UpdateEnemyStats(target, findSelectedTarget);

        print($"Ability Used: {unit.AbilitySet[findSelectedAbility].AbilityBase.abilityName}. Energy Cost: {unit.AbilitySet[findSelectedAbility].EnergyUsage}.");
        unit.ModifyEnergy(-unit.AbilitySet[findSelectedAbility].EnergyUsage);

        print($"{unit.CharacterName}'s Energy: {unit.CurrentEnergy}.");

        print($"{unit.CharacterName} attacked {target.CharacterName}. Current Target HP: {target.CurrentHealthPoint}. Damage Taken: {target.MaxHealthPoint - target.CurrentHealthPoint}.");
        unit.ActionSelected = true;

        UpdateUnitStats(unit);
    }

    public void OnSelectAbility(Unit unit)
    {
        int findSelectedAbility = abilityPanel.transform.Find(SelectedAbility.name).GetSiblingIndex();
        print($"Ability Selected");
        
        if(unit.HasEnoughEnergy(unit, findSelectedAbility))
        {
            ToggleAbilityPanel(false);
            ToggleEventTrigger(true);
        }
        else
        {
            print("You do not have enough energy to perform this attack.");
        }

    }
    #endregion

    #region Helper Functions (Toggle)
    public void TogglePlayerChoiceButtonInteractability(bool isOn)
    {
        attackButton.interactable = isOn;
        endButton.interactable = isOn;
    }

    public void ToggleEventTrigger(bool isOn)
    {
        for (int i = 0; i < enemyPosition.transform.childCount; i++)
        {
            EventTrigger getEventTrigger = enemyPosition.transform.GetChild(i).GetComponent<EventTrigger>();
            getEventTrigger.enabled = isOn;

            Button getButtons = enemyPosition.transform.GetChild(i).GetComponent<Button>();
            getButtons.interactable = isOn;
        }
    }

    private void ToggleAbilityPanel(bool isOn) => abilityPanel.SetActive(isOn);

    #endregion
}

