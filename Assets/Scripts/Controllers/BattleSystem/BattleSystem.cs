using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleSystem : MonoBehaviour
{
    #region Unit References
    [Header("Unit")]
    public GameObject unitPosition;
    #endregion

    #region Unit UI References
    public Transform unitPanel;
    public TextMeshProUGUI unitLevel, unitName, unitHealthPoint, unitEnergyPoint, unitHealthPointStat, unitAtkStat, unitDefStat, unitSpdStat, unitCritStat;
    #endregion

    #region Enemy References
    [Header("Enemy")]
    public GameObject enemyPosition;
    #endregion

    #region Turn-Based References
    private Queue<Character> turnQueue = new();
    private Character currentCharacter;
    #endregion

    #region Getters/Setters
    public Player Player { get; private set; }
    public EnemyHost EnemyHost { get; private set; }
    public List<Character> Characters { get; private set; }
    public PartyController PartyController { get; private set; }
    public EnemyHost EnemyPartyController { get; private set; }
    public Transform SelectedTarget { get; private set; }
    public Transform SelectedAbility { get; private set; }
    public List<Button> SelectTargetButtons { get; private set; }
    public Slider UnitHealthPointSlider { get; private set; }
    public Slider UnitEnergySlider { get; private set; }
    public Slider EnemyHealthPointSlider { get; private set; }
    public GameObject AbilityPanel { get; private set; }
    #endregion

    #region Unit Stats (Getters/Setters)
    public int Level { get; private set; }
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public float CritRate { get; private set; }
    #endregion

    #region Battle UI References
    [Header("Battle Options")]
    public Button attackButton;
    public Button endButton;
    public Transform abilityPanelParent;
    public GameObject levelPanelPrefab;
    #endregion

    public void Initialize(EnemyHost enemyHost) => this.EnemyHost = enemyHost;

    private void Start()
    {
        Characters = new();
        PartyController = FindObjectOfType<PartyController>();
        EnemyPartyController = EnemyHost;
        Player = FindObjectOfType<Player>();

        SelectTargetButtons = new();
        attackButton.onClick.AddListener(OnAttack);
        endButton.onClick.AddListener(OnEndTurn);

        BattleSetup();
        InitializeCharacters();

        SetupUnitsInBattle();
        SetupEnemiesInBattle();

        StartCoroutine(StartBattle());
    }

    private void BattleSetup()
    {
        Characters.AddRange(PartyController.partyMembers);
        Characters.AddRange(EnemyPartyController.enemyPartyMembers);
    }

    private void InitializeCharacters()
    {
        foreach (Character character in Characters)
            character.Initialize();

        RearrangeTurnBasedOnSpeed();

        foreach (Character character in Characters)
            turnQueue.Enqueue(character);

        currentCharacter = turnQueue.Peek();
    }

    public void UpdateUnitHealth(Unit unit)
    {
        TextMeshProUGUI unitHp = unit.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        unitHp.text = $"{unit.CurrentHealthPoint}/{unit.MaxHealthPoint}";

        UnitHealthPointSlider = unit.transform.GetChild(0).GetComponent<Slider>();
        UnitHealthPointSlider.value = (float)unit.CurrentHealthPoint / (float)unit.MaxHealthPoint;
    }

    public void UpdateUnitEnergy(Unit unit)
    {
        TextMeshProUGUI unitEnergy = unit.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        unitEnergy.text = $"{unit.CurrentEnergy}/{unit.MaxEnergy}";

        UnitEnergySlider = unit.transform.GetChild(1).GetComponent<Slider>();
        UnitEnergySlider.value = (float)unit.CurrentEnergy / (float)unit.MaxEnergy;
    }

    public void SetUnitStatsInUI(Unit unit)
    {
        SetUnitStatsIfExist(unit);

        unitLevel.text = $"Lv. {Level}";
        unitName.text = unit.CharacterName;
        unitHealthPointStat.text = $"HP: {MaxHp}";
        unitAtkStat.text = $"ATK: {Attack}";
        unitDefStat.text = $"DEF: {Defense}";
        unitSpdStat.text = $"SPD: {Speed}";
        unitCritStat.text = $"CRIT: {CritRate}";
    }

    public void UpdateEnemyStats(Enemy enemy)
    {
        TextMeshProUGUI enemyName = enemy.transform.Find("Enemy Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI enemyHp = enemy.transform.Find("Enemy Name").GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

        enemyName.text = enemy.CharacterName;
        enemyHp.text = $"{enemy.CurrentHealthPoint}/{enemy.MaxHealthPoint}";

        EnemyHealthPointSlider = enemy.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        EnemyHealthPointSlider.value = (float)enemy.CurrentHealthPoint / (float)enemy.MaxHealthPoint;
    }

    private void InitializeUnitSprite(Unit unit, Image unitSprite) => unitSprite.sprite = unit.unitData.RightSprite;

    private void InitializeEnemySprite(Enemy enemy, Image enemySprite) => enemySprite.sprite = enemy.enemyData.LeftSprite;

    private void SetUnitStats(Unit unit)
    {
        unit.SetCharacterName(unit.unitData.CharacterName);
        
        SetUnitStatsIfExist(unit, unit.unitData);

        unit.SetLevel(Level);
        unit.SetMaxHealthPoint(MaxHp);

        if (PlayerPrefs.GetInt($"{unit.CharacterName} Old Max HP") == PlayerPrefs.GetInt($"{unit.CharacterName} HP"))
        {
            CurrentHp = PlayerPrefs.GetInt($"{unit.CharacterName} Max HP", unit.MaxHealthPoint);
        }
        else
        {
            CurrentHp = PlayerPrefs.GetInt($"{unit.CharacterName} HP", unit.MaxHealthPoint);
        }

        unit.SetCurrentHealthPoint(CurrentHp);
        
        unit.SetMaxEnergy(unit.unitData.Energy);
        unit.SetCurrentEnergy(PlayerPrefs.GetInt($"{unit.CharacterName} Energy", unit.MaxEnergy));
        
        unit.SetAttack(Attack);
        unit.SetDefense(Defense);
        unit.SetSpeed(Speed);
        unit.SetCritRate(CritRate);
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
    }

    private void SetUnitStatsIfExist(Unit unit, UnitData unitData = null)
    {
        if (unitData)
        {
            Level = PlayerPrefs.GetInt($"{unit.CharacterName} Level", unitData.Level);
            MaxHp = PlayerPrefs.GetInt($"{unit.CharacterName} Max HP", unitData.HealthPoint);
            Attack = PlayerPrefs.GetInt($"{unit.CharacterName} Atk", unitData.Attack);
            Defense = PlayerPrefs.GetInt($"{unit.CharacterName} Def", unitData.Defense);
            Speed = PlayerPrefs.GetInt($"{unit.CharacterName} Spd", unitData.Speed);
            CritRate = PlayerPrefs.GetFloat($"{unit.CharacterName} Crit", unitData.CritRate);
        }
        else
        {
            Level = PlayerPrefs.GetInt($"{unit.CharacterName} Level", unit.Level);
            MaxHp = PlayerPrefs.GetInt($"{unit.CharacterName} Max HP", unit.MaxHealthPoint);
            Attack = PlayerPrefs.GetInt($"{unit.CharacterName} Atk", unit.Attack);
            Defense = PlayerPrefs.GetInt($"{unit.CharacterName} Def", unit.Defense);
            Speed = PlayerPrefs.GetInt($"{unit.CharacterName} Spd", unit.Speed);
            CritRate = PlayerPrefs.GetFloat($"{unit.CharacterName} Crit", unit.CritRate);
        }
    }

    private void SetupUnitsInBattle()
    {
        if (PartyController.partyMembers.Count == 1)
        {
            Unit getUnitRef = unitPosition.transform.GetChild(0).GetComponent<Unit>();
            Image unitSpriteRef = unitPosition.transform.GetChild(0).GetComponent<Image>();
            Unit findUnit = (Unit)Characters.Find(x => x.GetComponent<Unit>());
            getUnitRef.unitData = findUnit.GetComponent<Unit>().unitData;

            SetUnitStats(getUnitRef);
            SetUnitStatsInUI(getUnitRef);
            UpdateUnitHealth(getUnitRef);
            UpdateUnitEnergy(getUnitRef);
            InitializeUnitSprite(findUnit, unitSpriteRef);
        }
        else
        {
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
            {
                Unit unit = unitPosition.transform.GetChild(i).GetComponent<Unit>();
                SetUnitStats(unit);
                SetUnitStatsInUI(unit);
                UpdateUnitHealth(unit);
                UpdateUnitEnergy(unit);
            }

            Character initializeFirstUnit = turnQueue.FirstOrDefault(x => x.GetComponent<Unit>());

            if (initializeFirstUnit is Unit)
            {
                SetUnitStatsInUI((Unit)initializeFirstUnit);
            }
        }
    }

    private void SetupEnemiesInBattle()
    {
        if (EnemyPartyController.enemyPartyMembers.Count == 1)
        {
            Enemy getEnemyRef = enemyPosition.transform.GetChild(0).GetComponent<Enemy>();
            Image enemySpriteRef = enemyPosition.transform.GetChild(0).GetComponent<Image>();
            Enemy findEnemy = (Enemy)Characters.Find(x => x.GetComponent<Enemy>());
            getEnemyRef.enemyData = findEnemy.GetComponent<Enemy>().enemyData;

            SetEnemyStats(getEnemyRef);
            UpdateEnemyStats(getEnemyRef);
            InitializeEnemySprite(findEnemy, enemySpriteRef);
        }
        else
        {
            Enemy getEnemyRef = enemyPosition.transform.GetChild(0).GetComponent<Enemy>();

            for (int i = 1; i < EnemyPartyController.enemyPartyMembers.Count; i++)
            {
                Instantiate(getEnemyRef, enemyPosition.transform);
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
            }

            for (int i = 0; i < enemyPosition.transform.childCount; i++)
            {
                Enemy enemy = enemyPosition.transform.GetChild(i).GetComponent<Enemy>();
                SetEnemyStats(enemy);
                UpdateEnemyStats(enemy);
            }
                
        }

        for (int i = 0; i < enemyPosition.transform.childCount; i++)
            SelectTargetButtons.Add(enemyPosition.transform.GetChild(i).GetComponent<Button>());
    }

    private IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(1f);

        while (!IsBattleOver())
        {
            currentCharacter = turnQueue.Dequeue();

            Debug.Log($"{currentCharacter.CharacterName}'s turn.");

            if (currentCharacter is Unit)
            {
                Unit unitCharacter = SetCurrentUnit();
                Unit unitRef = currentCharacter.GetComponent<Unit>();
                SetUnitStatsInUI(unitCharacter);
                UpdateUnitHealth(unitCharacter);
                UpdateUnitEnergy(unitCharacter);

                if (!unitCharacter.HasMaxEnergy(unitCharacter))
                {
                    unitCharacter.ModifyEnergy(unitCharacter, unitCharacter.unitData.EnergyRegen);
                    UpdateUnitEnergy(unitCharacter);
                }

                for (int i = 0; i < SelectTargetButtons.Count; i++)
                {
                    SelectTargetButtons[i].onClick.AddListener(delegate { OnSelectTarget(unitCharacter, unitRef); });
                }
            }

            yield return currentCharacter.Action();

            turnQueue.Enqueue(currentCharacter);
            
            yield return new WaitForSeconds(2f);

            if (currentCharacter is Unit)
            {
                for (int i = 0; i < SelectTargetButtons.Count; i++)
                    SelectTargetButtons[i].onClick.RemoveAllListeners();
            }
        }

        Debug.Log("Battle's Over!");
        BattleOutcome();
    }

    private bool IsAllPlayerUnitDead()
    {
        if(unitPosition.transform.childCount > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsBattleOver()
    {
        bool allEnemiesDefeated = true;
        bool allUnitsDefeated = true;

        for (int i = 0; i < unitPosition.transform.childCount; i++)
        {
            Unit currentUnit = unitPosition.transform.GetChild(i).GetComponent<Unit>();
            if (!currentUnit.IsDead())
            {
                allUnitsDefeated = false;
            }
            else
            {
                PartyController.partyMembers.RemoveAll(x => x.CharacterName == currentUnit.CharacterName);
                turnQueue = new Queue<Character>(turnQueue.Where(x => x.CharacterName != currentUnit.CharacterName));
                currentUnit.transform.SetParent(null);
                Destroy(currentUnit.gameObject);

                if (unitPosition.transform.childCount > 0)
                {
                    allUnitsDefeated = false;
                }
                else
                {
                    allUnitsDefeated = true;
                }
            }
        }

        for (int i = 0; i < enemyPosition.transform.childCount; i++)
        {
            Enemy currentEnemy = enemyPosition.transform.GetChild(i).GetComponent<Enemy>();

            if (!currentEnemy.IsDead())
            {
                allEnemiesDefeated = false;
            }
            else
            {
                turnQueue = new Queue<Character>(turnQueue.Where(x => x.CharacterName != currentEnemy.CharacterName));
                currentEnemy.transform.SetParent(null);
                Destroy(currentEnemy.gameObject);

                if (enemyPosition.transform.childCount > 0)
                {
                    allEnemiesDefeated = false;
                }
                else
                {
                    allEnemiesDefeated = true;
                }
            }
        }
        return allEnemiesDefeated || allUnitsDefeated;
    }

    private void BattleOutcome()
    {
        if (IsAllPlayerUnitDead())
        {
            //TODO: all units are dead
            print("Man, you kinda suck at this game!");
        }
        else
        {
            print("You are the chosen one.");
            StartCoroutine(DestroyLastEnemyAndChangeState());
            SaveUnitStats();
        }
    }

    private void SaveUnitStats()
    {
        for (int i = 0; i < unitPosition.transform.childCount; i++)
        {
            Unit unit = unitPosition.transform.GetChild(i).GetComponent<Unit>();
            PlayerPrefs.SetInt($"{unit.CharacterName} HP", unit.CurrentHealthPoint);
            PlayerPrefs.SetInt($"{unit.CharacterName} Energy", unit.CurrentEnergy);
        }
    }
    private IEnumerator LevelRandomizeStats()
    {
        GameObject levelSystemObject = Instantiate(levelPanelPrefab, gameObject.transform);
        LevelSystem levelSystem = levelSystemObject.GetComponent<LevelSystem>();

        for (int i = 0; i < unitPosition.transform.childCount; i++)
        {
            Unit unit = unitPosition.transform.GetChild(i).GetComponent<Unit>();
            Unit unitChar = (Unit)Characters.Find(x => x.CharacterName == unit.CharacterName);

            List<Ability> abilitySet = unitChar.AbilitySet;

            levelSystem.SetUnitInformation(unit, abilitySet);

            levelSystem.UpgradeRandomizeStats(unit);

            yield return new WaitUntil(() => Keyboard.current[Key.Space].wasPressedThisFrame);
        }
        yield return new WaitUntil(() => Keyboard.current[Key.Space].wasPressedThisFrame);
        yield return new WaitForSeconds(1f);
        Destroy(levelSystemObject);
    }
    private IEnumerator DestroyLastEnemyAndChangeState()
    {
        yield return new WaitForSeconds(2f);

        Destroy(EnemyHost.gameObject);

        yield return LevelRandomizeStats();
        
        Destroy(this.gameObject);
        Player.PlayerStateMachine.ChangeState(Player.IdleState);
    }
    public void OnSelectedTarget(Transform target) => SelectedTarget = target;
    public void OnSelectedAbility(Transform ability) => SelectedAbility = ability;
    private void RearrangeTurnBasedOnSpeed() => Characters.Sort((x, y) => y.Speed.CompareTo(x.Speed));
    private Unit SetCurrentUnit()
    {
        for (int i = 0; i < unitPosition.transform.childCount; i++)
        {
            if (currentCharacter.GetComponent<Unit>().CharacterName == unitPosition.transform.GetChild(i).GetComponent<Unit>().CharacterName)
            {
                return unitPosition.transform.GetChild(i).GetComponent<Unit>();
            }
        }
        return null;
        
    }

    #region Button Event Listener
    private void OnAttack()
    {
        abilityPanelParent.gameObject.SetActive(true);
        for (int i = 0; i < PartyController.partyMembers.Count; i++)
        {
            if(currentCharacter.GetComponent<Unit>() == PartyController.partyMembers[i])
            {
                abilityPanelParent.GetChild(i).gameObject.SetActive(true);
                AbilityPanel = abilityPanelParent.GetChild(i).gameObject;
            }
            else
            {
                abilityPanelParent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void OnEndTurn()
    {
        if(currentCharacter is Unit)
        {
            ToggleAbilityPanel(false);
            currentCharacter.GetComponent<Unit>().ActionSelected = true;
        }
    }

    private void OnSelectTarget(Unit unit, Unit unitRef)
    {
        int findSelectedTarget = enemyPosition.transform.Find(SelectedTarget.name).GetSiblingIndex();
        ToggleEventTriggerForTargetSelection(false);

        GameObject targetIcon = enemyPosition.transform.GetChild(findSelectedTarget).GetChild(0).gameObject;
        targetIcon.SetActive(!targetIcon);

        Enemy target = enemyPosition.transform.GetChild(findSelectedTarget).GetComponent<Enemy>();

        int findSelectedAbility = AbilityPanel.transform.Find(SelectedAbility.name).GetSiblingIndex();

        target.TakeDamageFrom(unit, unitRef.AbilitySet[findSelectedAbility].AbilityBase.damage);

        EnemyHealthPointSlider = target.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        UpdateEnemyStats(target);

        unit.ModifyEnergy(unit, -unitRef.AbilitySet[findSelectedAbility].EnergyUsage);
        UpdateUnitEnergy(unit);

        unitRef.ActionSelected = true;

        attackButton.interactable = true;
        endButton.interactable = true;
    }

    public void OnSelectAbility()
    {
        Unit unit = SetCurrentUnit();
        Unit unitRef = currentCharacter.GetComponent<Unit>();
        int findSelectedAbility = AbilityPanel.transform.Find(SelectedAbility.name).GetSiblingIndex();
        
        if(unit.HasEnoughEnergy(unit, unitRef, findSelectedAbility))
        {
            ToggleAbilityPanel(false);
            attackButton.interactable = false;
            endButton.interactable = false;
            ToggleEventTriggerForTargetSelection(true);
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

    public void ToggleEventTriggerForTargetSelection(bool isOn)
    {
        for (int i = 0; i < enemyPosition.transform.childCount; i++)
        {
            EventTrigger getEventTrigger = enemyPosition.transform.GetChild(i).GetComponent<EventTrigger>();
            getEventTrigger.enabled = isOn;

            Button getButtons = enemyPosition.transform.GetChild(i).GetComponent<Button>();
            getButtons.interactable = isOn;
        }
    }

    private void ToggleAbilityPanel(bool isOn) => AbilityPanel.SetActive(isOn);

    #endregion
}

