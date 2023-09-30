using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region Stats References
    public string CharacterName { get; set; }
    public int MaxHealthPoint { get; set; }
    public int CurrentHealthPoint { get; set; }
    public int MaxEnergy { get; set; }
    public int CurrentEnergy { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public float CritRate { get; set; }
    public int Constant { get; set; }
    #endregion

    public abstract IEnumerator Action();
    public abstract void Initialize();
    public abstract void TakeDamageFrom(int damage);
    public abstract bool IsDead();
}
