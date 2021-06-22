using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    public int ManaCost { get; set; }
    public void Use();
}
