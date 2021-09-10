using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TechnologyType 
{
    Pottery,
    AnimalHusbandary,
    Mining
}
public enum TechInfrastructure 
{
    Granary,
    Pasture,
    Mine
}
public enum TechEffect 
{ 
    Harvesting, Maize, Rice, Wheat,
    Horses, Sheep, Deer,
    Woods, Copper
}

public class Research
{
    public Dictionary<TechnologyType, Technology> technologies = new Dictionary<TechnologyType, Technology>();
    public Technology currentTech;

    public void InitializeTechnologies()
    {
        technologies.Add(TechnologyType.Pottery, new Technology(2, TechInfrastructure.Granary, new List<TechEffect> { TechEffect.Harvesting, TechEffect.Maize, TechEffect.Rice, TechEffect.Wheat }));
        technologies.Add(TechnologyType.AnimalHusbandary, new Technology(2, TechInfrastructure.Pasture, new List<TechEffect> { TechEffect.Horses, TechEffect.Sheep, TechEffect.Deer }));
        technologies.Add(TechnologyType.Mining, new Technology(2, TechInfrastructure.Mine, new List<TechEffect> { TechEffect.Woods, TechEffect.Copper }));
    }
}

public class Technology
{
    public int turnsToComplete;
    public int turnsLeft;
    public TechInfrastructure infrastructure;
    public List<TechEffect> effects = new List<TechEffect>();

    public Technology(int _turnsToComplete, TechInfrastructure _infrastructure, List<TechEffect> _effects)
    {
        this.turnsToComplete = _turnsToComplete;
        this.infrastructure = _infrastructure;
        this.effects = _effects;
        turnsLeft = _turnsToComplete;
    }
}