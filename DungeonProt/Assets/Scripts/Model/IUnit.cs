using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    public int Health {get; set; }
    public int MaxHealth { get; set; }
    public void TakeDamage(int damage);
    public void Die();
    ///
}
