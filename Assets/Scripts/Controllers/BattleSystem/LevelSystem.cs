using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    #region UI References
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
    public Color upgradeColor;
    public Color originalColor;
    #endregion

    public void SetUnitInformation(Unit unit)
    {
        unitLevel.text = $"LVL. {unit.Level}";
        unitName.text = unit.CharacterName;
        unitSprite.sprite = unit.unitData.DownSprite;
        unitHealth.text = $"HP: {unit.MaxHealthPoint}";
        unitAttack.text = $"ATK: {unit.Attack}";
        unitDefense.text = $"DEF: {unit.Defense}";
        unitSpeed.text = $"SPD: {unit.Speed}";
        unitCrit.text = $"CRIT: {unit.CritRate}";

        LevelUp(unit);
    }

    public void LevelUp(Unit unit)
    {
        unitLevel.text = $"LVL. {unit.Level} + 1";
        unitLevel.color = upgradeColor;
        unit.SetLevel(unit.Level + 1);
    }

    public void UpgradeRandomizeStats(Unit unit)
    {
        int maxStatsToUpgrade = Random.Range(1, 6);
        List<int> statsIndex = new List<int> { 0, 1, 2, 3, 4 };
        int statsUpgraded = 0;

        while(statsUpgraded < maxStatsToUpgrade && statsIndex.Count > 0)
        {
            int randomIndex = Random.Range(0, statsIndex.Count);
            int stat = statsIndex[randomIndex];

            int statIncrease = Random.Range(1, 6);

            //TODO: Save Stats
            switch (stat)
            {
                case 0: 
                    unitHealth.text = $"HP: {unit.MaxHealthPoint} + {statIncrease}";
                    unitHealth.color = upgradeColor;
                    unit.SetMaxHealthPoint(unit.MaxHealthPoint + statIncrease);
                    break;
                case 1:
                    unitAttack.text = $"ATK: {unit.Attack} + {statIncrease}";
                    unitAttack.color = upgradeColor;
                    unit.SetAttack(unit.Attack + statIncrease);
                    break;
                case 2:
                    unitDefense.text = $"DEF: {unit.Defense} + {statIncrease}";
                    unitDefense.color = upgradeColor;
                    unit.SetDefense(unit.Defense + statIncrease);
                    break;
                case 3:
                    unitSpeed.text = $"SPD: {unit.Speed} + {statIncrease}";
                    unitSpeed.color = upgradeColor;
                    unit.SetSpeed(unit.Speed + statIncrease);
                    break;
                case 4:
                    unitCrit.text = $"CRIT: {unit.CritRate:F2} + {statIncrease / 100f}";
                    unitCrit.color = upgradeColor;
                    unit.SetCritRate(unit.CritRate + (statIncrease / 100f));
                    break;
            }

            statsIndex.RemoveAt(randomIndex);
            statsUpgraded++;
        }
    }
}
