using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable : IUnit
{
    public void Heal(int hp);
}
