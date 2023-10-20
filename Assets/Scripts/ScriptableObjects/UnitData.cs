using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Character/Unit Data")]
public class UnitData : CharacterData
{
    public int Energy;
    public int EnergyRegen;

    [SerializeField] List<LearnableAbility> learnableAbilities;
    public List<LearnableAbility> LearnableAbilities
    {
        get => learnableAbilities;
        set => learnableAbilities = value;
    }
}

[System.Serializable]
public class LearnableAbility
{
    [SerializeField] AbilityBase abilityBase;
    [SerializeField] int level;

    public AbilityBase Base => abilityBase;
    public int Level => level;
}
