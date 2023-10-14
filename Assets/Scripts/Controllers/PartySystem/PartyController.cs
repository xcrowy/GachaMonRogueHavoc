using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PartyController : MonoBehaviour
{
    #region Object References
    public Player player { get; set; }
    public GameObject collectionSystem;
    public Transform collection;
    public GameObject unitInfoInterface;
    #endregion

    #region Unit & Unit Info Container
    public Dictionary<Unit, GameObject> unitInfoDict { get; private set; }
    #endregion

    #region Party List Creation
    public List<Unit> availableUnits = new();
    private List<Unit> inCollection = new();
    public List<Unit> partyMembers { get; private set; }
    #endregion

    // TODO: Eventually there will be a hub for the player to choose their units before starting a run
    private void Start()
    {
        unitInfoDict = new();
        partyMembers = new();
        player = GameObject.Find("Player").GetComponent<Player>();
        GatherAvailableUnits();

    }

    private void Update()
    {
        if (Keyboard.current[Key.Tab].wasPressedThisFrame && player.PlayerStateMachine.CurrentState.Equals(player.IdleState))
        {
            ToggleCollectionPanel();
            player.PlayerStateMachine.ChangeState(player.InCollectionState);
        }
        else if (Keyboard.current[Key.Tab].wasPressedThisFrame && player.PlayerStateMachine.CurrentState.Equals(player.InCollectionState))
        {
            if(partyMembers.Count > 0)
            {
                ToggleCollectionPanel();
                player.PlayerStateMachine.ChangeState(player.IdleState);
            }
            else
            {
                print("You need to have at least 1 unit in your party.");
            }
        }

    }

    public void ToggleCollectionPanel()
    {
        collectionSystem.SetActive(!collectionSystem.activeSelf);
    }

    private void GatherAvailableUnits()
    {
        foreach (Unit unit in availableUnits)
        {
            if (unit.inCollection && !inCollection.Contains(unit))
            {
                for (int i = 0; i < collection.childCount; i++)
                {
                    var child = collection.GetChild(i).gameObject;

                    if (child.transform.childCount == 0)
                    {
                        Unit unitClone = Instantiate(unit, child.transform);
                        inCollection.Add(unitClone);
                        GameObject infoInterface = Instantiate(unitInfoInterface, collection.transform);
                        infoInterface.SetActive(false);
                        unitInfoDict[unitClone] = infoInterface;
                        break;
                    }
                }

            }
        }
    }

    public bool AddUnitToParty(Unit unit)
    {
        if (!partyMembers.Contains(unit) && partyMembers.Count <= 4)
        {
            partyMembers.Add(unit);
            print($"{unit.CharacterName} has been added to the party.");
            print($"Total Member in Party: {partyMembers.Count}");
            return true;
        }

        print("You cannot have more than 4 units in your party.");
        return false;
    }

    public bool RemoveUnitFromParty(Unit unit)
    {
        if (partyMembers.Contains(unit) && partyMembers.Count > 1)
        {
            partyMembers.Remove(unit);
            print($"{unit.CharacterName} has been removed from the party.");
            print($"Total Member in Party: {partyMembers.Count}");
            return true;
        }

        print("You need to have at least 1 unit in your party.");
        return false;
    }
}

