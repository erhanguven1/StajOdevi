using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityUiManager : Instancable<CityUiManager>, ISelectableUI
{
    private City city;

    public GameObject cityUI;
    public CityStatsPanel cityStatsPanel; //Left panel
    public ProductionPanel productionPanel; //Right panel

    public void InitializeUI(City _city)
    {
        if (city != null)
        {
            if (city == _city)
            {
                return;
            }
        }

        city = _city;
    }

    public void Open()
    {
        city.isSelected = true;
        cityUI.SetActive(true);
        cityStatsPanel.Initialize(city);
        productionPanel.Initialize(ref city.production);
    }

    public void Close()
    {
        city.isSelected = false;
        city.production.Close();
        cityUI.SetActive(false);
        city = null;
    }
}

public enum CityStatsPanelTextType { Population, Happiness, TurnsLeftToGrow, Food, Mine, Wood }
[System.Serializable]
public class CityStatsPanel
{
    public Text txtPopulation;
    public Text txtHappiness;
    public Text txtTurnsLeftToGrow;
    public Text txtFood;
    public Text txtMine;
    public Text txtWood;

    public void Initialize(City city)
    {
        SetText(CityStatsPanelTextType.Population, city.cityStats.population.ToString());
        SetText(CityStatsPanelTextType.Happiness, city.cityStats.happiness.ToString());
        SetText(CityStatsPanelTextType.TurnsLeftToGrow, city.cityStats.TurnsLeftToGrow.ToString());

        SetText(CityStatsPanelTextType.Food, city.cityStats.food.ToString());
        SetText(CityStatsPanelTextType.Mine, city.cityStats.mine.ToString());
        SetText(CityStatsPanelTextType.Wood, city.cityStats.wood.ToString());
    }

    public void SetText(CityStatsPanelTextType type, string text)
    {
        switch (type)
        {
            case CityStatsPanelTextType.Population:
                txtPopulation.text = "Population: " + text;
                break;
            case CityStatsPanelTextType.Happiness:
                txtHappiness.text = "Happiness: " + text;
                break;
            case CityStatsPanelTextType.TurnsLeftToGrow:
                txtTurnsLeftToGrow.text = "Turns Left To Grow: " + text;
                break;
            case CityStatsPanelTextType.Food:
                txtFood.text = "Food: " + text;
                break;
            case CityStatsPanelTextType.Mine:
                txtMine.text = "Mine: " + text;
                break;
            case CityStatsPanelTextType.Wood:
                txtWood.text = "Wood: " + text;
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class ProductionPanel
{
    public Button buttonPrefab;

    public RectTransform improvementsContentArea, unitsContentArea;
    public List<Button> productionButtons = new List<Button>();

    public void Initialize(ref Production production)
    {
        var a = productionButtons.ToArray();
        for (int i = 0; i < a.Length; i++)
        {
            GameObject.Destroy(a[i].gameObject);
        }
        productionButtons.Clear();
        if (ProductionManager.Instance.useFake)
        {
            var k = ProductionManager.Instance.fakeProductsConverted;
            production.productionItems.Clear();
            foreach (var item in k)
            {
                production.productionItems.Add(item);
            }
        }

        foreach (var item in production.productionItems)
        {
            var o = GameObject.Instantiate(buttonPrefab);
            o.transform.GetChild(0).GetComponent<Text>().text = item.type.ToString();
            o.transform.GetChild(1).GetComponent<Text>().text = item.productionTime.ToString();

            if (item.productionType == ProductionType.Improvements)
            {
                o.transform.SetParent(improvementsContentArea, false);
            }
            else
            {
                o.transform.SetParent(unitsContentArea, false);
            }

            productionButtons.Add(o.GetComponent<Button>());
            item.InitializeButton(o.GetComponent<Button>());
        }

        production.Initialize();
    }
}