using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    #region UI References
    [Header("Unit Stat References")]
    public TextMeshProUGUI unitLevel;
    public TextMeshProUGUI unitName;
    public Image unitSprite;
    public TextMeshProUGUI unitHealth;
    public TextMeshProUGUI unitAttack;
    public TextMeshProUGUI unitDefense;
    public TextMeshProUGUI unitSpeed;
    public TextMeshProUGUI unitCrit;
    #endregion

    #region Color References
    [Header("Color References")]
    public Color upgradeColor;
    public Color originalColor;
    #endregion

    #region New Ability System
    [Header("Prefab References")]
    public GameObject learnNewAbilityPrefab;
    #endregion

    public void SetUnitInformation(Unit unit, List<Ability> abilities = null)
    {
        unitLevel.text = $"LVL. {PlayerPrefs.GetInt($"{unit.CharacterName} Level", unit.Level)}";
        unitName.text = unit.CharacterName;
        unitSprite.sprite = unit.unitData.DownSprite;
        unitHealth.text = $"HP: {PlayerPrefs.GetInt($"{unit.CharacterName} Max HP", unit.MaxHealthPoint)}";
        unitAttack.text = $"ATK: {PlayerPrefs.GetInt($"{unit.CharacterName} Atk", unit.Attack)}";
        unitDefense.text = $"DEF: {PlayerPrefs.GetInt($"{unit.CharacterName} Def", unit.Defense)}";
        unitSpeed.text = $"SPD: {PlayerPrefs.GetInt($"{unit.CharacterName} Spd", unit.Speed)}";
        unitCrit.text = $"CRIT: {PlayerPrefs.GetFloat($"{unit.CharacterName} Crit", unit.CritRate)}";

        LevelUp(unit, abilities);
    }

    public void LevelUp(Unit unit, List<Ability> abilities = null)
    {
        unitLevel.text = $"LV. {unit.Level} + 1";
        unitLevel.color = upgradeColor;
        unit.SetLevel(unit.Level + 1);
        PlayerPrefs.SetInt($"{unit.CharacterName} Level", unit.Level);

        foreach (LearnableAbility ability in unit.unitData.LearnableAbilities)
        {
            if (ability.Level == unit.Level)
            {
                GameObject learnNewAbilityObject = Instantiate(learnNewAbilityPrefab, transform);
                NewAbilitySystem newAbilitySystem = learnNewAbilityObject.GetComponent<NewAbilitySystem>();
                newAbilitySystem.SetUnitInfo(unit);

                if (abilities.Count < 4)
                {
                    newAbilitySystem.LearnableAbilityForUnit(unit, ability);
                }
                else
                {
                    newAbilitySystem.SetExistingAbilityInfo(abilities);
                }
            }            
        }
    }

    public void UpgradeRandomizeStats(Unit unit)
    {
        ResetUpgradeTextColor();
        int maxStatsToUpgrade = Random.Range(1, 6);
        List<int> statsIndex = new List<int> { 0, 1, 2, 3, 4 };
        int statsUpgraded = 0;

        while(statsUpgraded < maxStatsToUpgrade && statsIndex.Count > 0)
        {
            int randomIndex = Random.Range(0, statsIndex.Count);
            int stat = statsIndex[randomIndex];

            int statIncrease = Random.Range(1, 6);

            switch (stat)
            {
                case 0: 
                    unitHealth.text = $"HP: {unit.MaxHealthPoint} + {statIncrease}";
                    unitHealth.color = upgradeColor;
                    PlayerPrefs.SetInt($"{unit.CharacterName} Old Max HP", unit.MaxHealthPoint);
                    unit.SetMaxHealthPoint(unit.MaxHealthPoint + statIncrease);
                    PlayerPrefs.SetInt($"{unit.CharacterName} Max HP", unit.MaxHealthPoint);
                    break;
                case 1:
                    unitAttack.text = $"ATK: {unit.Attack} + {statIncrease}";
                    unitAttack.color = upgradeColor;
                    unit.SetAttack(unit.Attack + statIncrease);
                    PlayerPrefs.SetInt($"{unit.CharacterName} Atk", unit.Attack);
                    break;
                case 2:
                    unitDefense.text = $"DEF: {unit.Defense} + {statIncrease}";
                    unitDefense.color = upgradeColor;
                    unit.SetDefense(unit.Defense + statIncrease);
                    PlayerPrefs.SetInt($"{unit.CharacterName} Def", unit.Defense);
                    break;
                case 3:
                    unitSpeed.text = $"SPD: {unit.Speed} + {statIncrease}";
                    unitSpeed.color = upgradeColor;
                    unit.SetSpeed(unit.Speed + statIncrease);
                    PlayerPrefs.SetInt($"{unit.CharacterName} Spd", unit.Speed);
                    break;
                case 4:
                    unitCrit.text = $"CRIT: {unit.CritRate:F2} + {statIncrease / 100f}";
                    unitCrit.color = upgradeColor;
                    unit.SetCritRate(unit.CritRate + (statIncrease / 100f));
                    PlayerPrefs.SetFloat($"{unit.CharacterName} Crit", unit.CritRate);
                    break;
            }

            statsIndex.RemoveAt(randomIndex);
            statsUpgraded++;
        }
    }

    public void ResetUpgradeTextColor()
    {
        unitHealth.color = originalColor;
        unitAttack.color = originalColor;
        unitDefense.color = originalColor;
        unitSpeed.color = originalColor;
        unitCrit.color = originalColor;
    }
}
