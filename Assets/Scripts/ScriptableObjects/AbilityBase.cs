using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Data")]
public class AbilityBase : ScriptableObject
{
    public string abilityName;
    [TextArea]
    public string description;

    public int damage;
    public int energy;
}
