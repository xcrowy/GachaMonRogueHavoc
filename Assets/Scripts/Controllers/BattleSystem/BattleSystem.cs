using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class BattleSystem : MonoBehaviour
{
    #region Unit References
    [Header("Unit")]
    public VerticalLayoutGroup unitPosition;
    #endregion

    #region Unit UI References
    public TextMeshProUGUI unitName, unitHealthPoint, unitEnergyPoint, unitHealthPointStat, unitAtkStat, unitDefStat, unitSpdStat, unitCritStat;
    #endregion

    #region Enemy References
    [Header("Enemy")]
    public VerticalLayoutGroup enemyPanel;
    public VerticalLayoutGroup enemyPosition;
    #endregion

    #region Turn-Based References
    //TODO: Consider Speed when adding it to the queue
    private Queue<Character> turnQueue = new();
    private Character currentCharacter;
    #endregion

    #region Getters/Setters
    public List<Character> characters { get; private set; }
    public PartyController partyController { get; private set; }
    public EnemyHost enemyPartyController { get; private set; }
    #endregion

    private void Start()
    {
        characters = new();
        partyController = FindObjectOfType<PartyController>();
        enemyPartyController = FindObjectOfType<EnemyHost>();
        BattleSetup();
        InitializeCharacters();
        StartCoroutine(StartBattle());
    }

    private void BattleSetup()
    {
        characters.AddRange(partyController.partyMembers);
        characters.AddRange(enemyPartyController.enemyPartyMembers);

        CheckForSingleUnitBattles();
        CheckForMultipleUnitInBattles();
        CheckForMultipleEnemyInBattles();
    }

    private void CheckForSingleUnitBattles()
    {
        if (partyController.partyMembers.Count == 1)
        {
            Unit getUnitRef = unitPosition.transform.GetChild(0).GetComponent<Unit>();
            Image unitSpriteRef = unitPosition.transform.GetChild(0).GetComponent<Image>();
            Unit findUnit = (Unit)characters.Find(x => x.GetComponent<Unit>());
            getUnitRef.unitData = findUnit.GetComponent<Unit>().unitData;

            InitializeUnitSprite(findUnit, unitSpriteRef);
        }

        if (enemyPartyController.enemyPartyMembers.Count == 1)
        {
            Enemy getEnemyRef = enemyPosition.transform.GetChild(0).GetComponent<Enemy>();
            Image enemySpriteRef = enemyPosition.transform.GetChild(0).GetComponent<Image>();
            Enemy findEnemy = (Enemy)characters.Find(x => x.GetComponent<Enemy>());
            getEnemyRef.enemyData = findEnemy.GetComponent<Enemy>().enemyData;

            InitializeEnemySprite(findEnemy, enemySpriteRef);
        }
    }

    private void CheckForMultipleUnitInBattles()
    {
        if (partyController.partyMembers.Count > 1)
        {
            switch (partyController.partyMembers.Count)
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
            for (int i = 1; i < partyController.partyMembers.Count; i++)
                Instantiate(getUnitRef, unitPosition.transform);

            List<Character> findUnit = characters.FindAll(x => x.GetComponent<Unit>());

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
        }
    }

    private void CheckForMultipleEnemyInBattles()
    {
        if (enemyPartyController.enemyPartyMembers.Count > 1)
        {
            switch (enemyPartyController.enemyPartyMembers.Count)
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
            
            for (int i = 1; i < enemyPartyController.enemyPartyMembers.Count; i++)
            {
                Instantiate(getEnemyRef, enemyPosition.transform);
                Instantiate(getEnemyTextData, enemyPanel.transform);
            }

            List<Character> findEnemy = characters.FindAll(x => x.GetComponent<Enemy>());
            for (int i = 0; i < enemyPosition.transform.childCount; i++)
            {
                enemyPosition.transform.GetChild(i).GetComponent<Enemy>().enemyData = findEnemy[i].GetComponent<Enemy>().enemyData;
                for (int j = 0; j < findEnemy.Count; j++)
                {
                    Image enemySpriteRef = enemyPosition.transform.GetChild(j).GetComponent<Image>();
                    Enemy enemy = findEnemy[j].GetComponent<Enemy>();
                    InitializeEnemySprite(enemy, enemySpriteRef);
                }
                InitializeEnemyStatText(i, findEnemy[i].GetComponent<Enemy>());
            }
        }
    }

    private void InitializeEnemyStatText(int index, Enemy enemy)
    {
        TextMeshProUGUI enemyName = enemyPanel.transform.GetChild(index).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI enemyHp = enemyPanel.transform.GetChild(index).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

        enemyName.text = enemy.CharacterName;
        enemyHp.text = $"{enemy.CurrentHealthPoint}/{enemy.MaxHealthPoint}";
    }

    private void InitializeUnitSprite(Unit unit, Image unitSprite) => unitSprite.sprite = unit.unitData.RightSprite;

    private void InitializeEnemySprite(Enemy enemy, Image enemySprite) => enemySprite.sprite = enemy.enemyData.LeftSprite;

    private void InitializeCharacters()
    {
        //TODO: check to see which unit's turn it is

        foreach (Character character in characters)
            character.Initialize();

        RearrangeTurnBasedOnSpeed();

        foreach (Character character in characters)
            turnQueue.Enqueue(character);

        currentCharacter = turnQueue.Peek();
        Character checkFirstUnit = turnQueue.FirstOrDefault(x => x.GetComponent<Unit>());

        if (checkFirstUnit is Unit)
            InitializeUnitStatText((Unit)checkFirstUnit);
    }

    private void InitializeUnitStatText(Unit unit)
    {
        unitName.text = unit.CharacterName;
        unitHealthPoint.text = $"{unit.CurrentHealthPoint}/{unit.MaxHealthPoint}";
        unitEnergyPoint.text = $"{unit.CurrentEnergy}/{unit.MaxEnergy}";
        unitHealthPointStat.text = $"HP: {unit.MaxHealthPoint}";
        unitAtkStat.text = $"ATK: {unit.Attack}";
        unitDefStat.text = $"DEF: {unit.Defense}";
        unitSpdStat.text = $"SPD: {unit.Speed}";
        unitCritStat.text = $"CRIT: {unit.CritRate}";
    }

    private void RearrangeTurnBasedOnSpeed() => characters.Sort((x, y) => y.Speed.CompareTo(x.Speed));


    private IEnumerator StartBattle()
    {
        while (!IsBattleOver())
        {
            currentCharacter = turnQueue.Dequeue();
            Debug.Log($"{currentCharacter.CharacterName}'s turn.");
            if (currentCharacter is Unit)
                InitializeUnitStatText((Unit)currentCharacter);
            yield return currentCharacter.Action();
            turnQueue.Enqueue(currentCharacter);
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("Battle's Over!");
        BattleOutcome();
    }

    private bool IsBattleOver()
    {
        bool allEnemiesDefeated = true;
        bool allUnitsDefeated = true;

        foreach (var character in characters)
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

    private bool IsAllPlayerUnitDead()
    {
        foreach (var character in characters)
        {
            if (character is Unit && !character.IsDead())
                return false;
            else
                return true;
        }
        return false;
    }
}

