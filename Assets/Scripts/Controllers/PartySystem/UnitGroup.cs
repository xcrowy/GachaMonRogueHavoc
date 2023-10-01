using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitGroup : MonoBehaviour
{
    #region Button Interactions
    [Header("Button Interactions")]
    public List<UnitButton> unitButtons;
    public Color unitIdle;
    public Color unitHover;
    public Color unitActive;
    #endregion

    #region Collection to Party Interactions 
    public UnitButton selectedUnit { get; private set; }
    public GameObject unitInfoInterface { get; private set; }
    public Button addToPartyButton { get; private set; }
    public Button removeFromPartyButton { get; private set; }
    private PartyController partyController;
    #endregion

    private void Start()
    {
        partyController = FindObjectOfType<PartyController>();
    }

    public void Subscribe(UnitButton unit)
    {
        if (unitButtons == null)
            unitButtons = new List<UnitButton>();

        unitButtons.Add(unit);
    }

    public void OnUnitEnter(UnitButton button)
    {
        ResetUnitButtons();
        if (selectedUnit == null || button != selectedUnit)
            button.background.color = unitHover;

        button.background.color = unitHover;
    }

    public void OnUnitExit(UnitButton button)
    {
        ResetUnitButtons();
    }

    public void OnUnitSelected(UnitButton button)
    {
        selectedUnit = button;
        ResetUnitButtons();
        button.background.color = unitActive;

        if(selectedUnit.transform.childCount > 0)
        {
            Unit unit = selectedUnit.transform.GetChild(0).GetComponent<Unit>();
            unitInfoInterface = partyController.unitInfoDict[unit];
            unitInfoInterface.SetActive(true);

            InitializeUnitInfo();
        }
    }

    public void ResetUnitButtons()
    {
        foreach (UnitButton button in unitButtons)
        {
            if (selectedUnit != null && button == selectedUnit)
                continue;

            button.background.color = unitIdle;
        }
    }

    private void InitializeUnitInfo()
    {
        UnitData unitData = selectedUnit.transform.GetChild(0).GetComponent<Unit>().unitData;
        unitInfoInterface.transform.GetChild(0).GetComponent<Image>().sprite = unitData.DownSprite;

        Transform unitInfo = unitInfoInterface.transform.GetChild(1);
        SetUnitInfo(unitData, selectedUnit.transform.GetChild(0).GetComponent<Unit>());
        SetUnitStatsInText(unitData, unitInfo);
        InitializePartyButtons();
    }

    private void SetUnitInfo(UnitData unitData, Unit unit)
    {
        unit.CharacterName = unitData.CharacterName;
        unit.MaxHealthPoint = unitData.HealthPoint;
        unit.CurrentHealthPoint = unit.MaxHealthPoint;
        unit.MaxEnergy = unitData.Energy;
        unit.CurrentEnergy = unit.MaxEnergy;
        unit.Attack = unitData.Attack;
        unit.Defense = unitData.Defense;
        unit.Speed = unitData.Speed;
        unit.CritRate = unitData.CritRate;
        unit.Constant = unitData.Constant;
    }

    private void SetUnitStatsInText(UnitData unitData, Transform unitInfo)
    {
        unitInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unitData.CharacterName;
        unitInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"HP: {unitData.HealthPoint}";
        unitInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"NRG: {unitData.Energy}";
        unitInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"ATK: {unitData.Attack}";
        unitInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"DEF: {unitData.Defense}";
        unitInfo.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = $"SPD: {unitData.Speed}";
        unitInfo.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = $"CRIT: {unitData.CritRate}";
    }

    private void InitializePartyButtons()
    {
        addToPartyButton = unitInfoInterface.transform.GetChild(3).GetComponent<Button>();
        addToPartyButton.onClick.AddListener(AddToParty);

        removeFromPartyButton = unitInfoInterface.transform.GetChild(4).GetComponent<Button>();
        removeFromPartyButton.onClick.AddListener(RemoveFromParty);

        if (!CheckIfUnitInParty(selectedUnit))
            removeFromPartyButton.gameObject.SetActive(false);
        else
            removeFromPartyButton.gameObject.SetActive(true);
    }

    public void AddToParty() //TODO: Add different color to the button for those units that are in party
    {
        Unit unit = selectedUnit.transform.GetChild(0).GetComponent<Unit>();
        if(partyController != null && !CheckIfUnitInParty(unit))
        {
            bool success = partyController.AddUnitToParty(unit);
            if (success && addToPartyButton != null)
            {
                unit.inParty = true;
                removeFromPartyButton.gameObject.SetActive(true);
            }
        }
    }

    public void RemoveFromParty()
    {
        Unit unit = selectedUnit.transform.GetChild(0).GetComponent<Unit>();
        if (partyController != null && CheckIfUnitInParty(unit))
        {
            bool success = partyController.RemoveUnitFromParty(unit);
            if(success && removeFromPartyButton != null)
            {
                unit.inParty = false;
                removeFromPartyButton.gameObject.SetActive(false);
            }
        } 
    }

    public bool CheckIfUnitInParty(Unit unit) => unit.inParty;
    public bool CheckIfUnitInParty(UnitButton selected) => selected.transform.GetChild(0).GetComponent<Unit>().inParty;
}
