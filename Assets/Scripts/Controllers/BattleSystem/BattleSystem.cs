using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public List<Character> characters = new();
    private Queue<Character> turnQueue = new();
    private Character currentCharacter;

    private void Start()
    {
        InitializeCharacters();
        StartCoroutine(StartBattle());
    }

    private void InitializeCharacters()
    {
        foreach (var character in characters)
        {
            character.Initialize();
            turnQueue.Enqueue(character);
        }

        currentCharacter = turnQueue.Peek();
    }

    private IEnumerator StartBattle()
    {
        while (!IsBattleOver())
        {
            currentCharacter = turnQueue.Dequeue();
            Debug.Log($"{currentCharacter.name}'s turn.");
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

