using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{
    public int Damage;
    public float Distance;
    public float AttackLock;

    public void Attack(IUnit unit)
    {
        unit.TakeDamage(1111);
    }
}
