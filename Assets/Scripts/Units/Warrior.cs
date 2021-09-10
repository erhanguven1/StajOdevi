using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Unit
{
    public override void SetConfigs()
    {
        unitType = UnitType.Warrior;
        movement = 3;
        health = 5;
        damage = 2;
    }
}
