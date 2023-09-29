using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract IEnumerator Action();
    public abstract void Initialize();
    public abstract void TakeDamageFrom(int damage);
    public abstract bool IsDead();
}
