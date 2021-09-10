using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : Instancable<ResearchManager>
{
    public Research research;

    // Start is called before the first frame update. Really?
    void Start()
    {
        research = new Research();
        research.InitializeTechnologies();

        TurnManager.onNewTurnStarted += NewTurn;
    }

    public void OnTapTechnologyButton(TechnologyType type)
    {
        research.currentTech = research.technologies[type];
    }

    private void NewTurn()
    {
        if (research.currentTech != null)
        {
            research.currentTech.turnsLeft--;
            if (research.currentTech.turnsLeft == 0)
            {
                ApplyEffects(research.currentTech.effects, research.currentTech.infrastructure);
                research.currentTech = null;
            }
        }
    }

    public void ApplyEffects(List<TechEffect> effects, TechInfrastructure infrastructure)
    {
        string msg = "";
        foreach (var item in effects)
        {
            msg += item.ToString() + ", ";
            switch (item)
            {
                case TechEffect.Harvesting:
                    break;
                case TechEffect.Maize:
                    break;
                case TechEffect.Rice:
                    break;
                case TechEffect.Wheat:
                    break;
                case TechEffect.Horses:
                    break;
                case TechEffect.Sheep:
                    break;
                case TechEffect.Deer:
                    break;
                case TechEffect.Woods:
                    break;
                case TechEffect.Copper:
                    break;
                default:
                    break;
            }
        }
        print("Unlocked things: " + msg);

        switch (infrastructure)
        {
            case TechInfrastructure.Granary:
                break;
            case TechInfrastructure.Pasture:
                break;
            case TechInfrastructure.Mine:
                break;
            default:
                break;
        }
    }
}
