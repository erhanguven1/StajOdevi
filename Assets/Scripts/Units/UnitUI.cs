using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : Instancable<UnitUI>, ISelectableUI
{
    public Unit unit;

    public Text unitName, damage, health, movement;

    public void SetUnit(Unit _unit)
    {
        unit = _unit;
    }

    public void UpdateUnitUI()
    {
        unitName.text = unit.unitType.ToString();
        damage.text = "Damage: "+unit.damage.ToString();
        health.text = "Health: "+unit.healthLeft.ToString() + "/" + unit.health.ToString();
        movement.text = "Movement: "+unit.movementLeft.ToString() + "/" + unit.movement.ToString();
    }

    public void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        UpdateUnitUI();
    }

    public void Close()
    {
        unit = null;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}