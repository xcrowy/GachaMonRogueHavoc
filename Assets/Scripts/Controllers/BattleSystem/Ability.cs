using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public AbilityBase AbilityBase { get; set; }
    public int EnergyUsage { get; set; }

    public Ability(AbilityBase abilityBase)
    {
        AbilityBase = abilityBase;
        EnergyUsage = abilityBase.energy;
    }
}
