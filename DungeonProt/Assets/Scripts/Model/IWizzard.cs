using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWizzard
{ 
    public int ManaPoints { get; set; }
    public int MaxMana { get; set; }
    public void RegenMana(int mp);
    public ISpell spell { get; set; }
}
