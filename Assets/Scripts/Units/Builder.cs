    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Mine, Farm }
public class Builder : Unit
{
    public int actionsLeft;

    public override void SetConfigs()
    {
        unitType = UnitType.Builder;
        movement = 3;
    }

    public void Build(BuildingType type)
    {

    }
}
