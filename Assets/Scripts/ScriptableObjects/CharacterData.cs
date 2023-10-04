using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Entity/Character")]
public class CharacterData : EntityData
{
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;
    public int Level;
    public string CharacterName;
    public int HealthPoint;
    public int Attack;
    public int Defense;
    public int Speed;
    public float CritRate;
    public int Constant;
}
