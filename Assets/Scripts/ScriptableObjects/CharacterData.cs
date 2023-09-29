using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Character", order = 1)]
public class CharacterData : EntityData
{
    public string CharacterName;
    public int HealthPoint;
    public int Attack;
    public int Defense;
    public int Speed;
    public float CritRate;
    public int Constant;
}
