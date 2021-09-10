using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionManager : Instancable<ProductionManager>
{
    public bool useFake;
    public List<FakeProduct> fakeProducts = new List<FakeProduct>();
    public List<ProductionItem> fakeProductsConverted = new List<ProductionItem>();

    public List<ProductionItem> producingItems = new List<ProductionItem>();

    public Settler settler;
    public Builder builder;
    public Warrior warrior;

    private void Start()
    {
        ConvertFake();
        TurnManager.onNewTurnStarted += OnNewTurn;
    }

    public void ConvertFake()
    {
        foreach (var item in fakeProducts)
        {
            var product = new ProductionItem();

            product.type = item.itemType;
            product.productionTime = item.productionTime;
            product.productionType = item.productionType;

            fakeProductsConverted.Add(product);
        }
    }

    public void OnNewTurn()
    {
        if (producingItems.Count > 0)
        {
            foreach (var item in producingItems)
            {
                item.NewTurn();
            }
        }
    }

    public void OnProductionHasFinished(City city, Production production)
    {
        var item = production.currentProductionItem;
        StartCoroutine(waitAndRemove());
        IEnumerator waitAndRemove()
        {
            yield return new WaitForEndOfFrame();

            switch (item.type)
            {
                case ProductionItemType.Monument:
                    city.cityStats.culture++;
                    break;
                case ProductionItemType.Settler:
                    InitializeUnit(PhotonNetwork.Instantiate("Settler", city.transform.position, Quaternion.identity).GetComponent<Unit>());
                    break;
                case ProductionItemType.Builder:
                    InitializeUnit(PhotonNetwork.Instantiate("Builder", city.transform.position, Quaternion.identity).GetComponent<Unit>());
                    break;
                case ProductionItemType.Warrior:
                    InitializeUnit(PhotonNetwork.Instantiate("Warrior", city.transform.position, Quaternion.identity).GetComponent<Unit>());
                    break;
                default:
                    break;
            }

            producingItems.Remove(item);
            production.currentProductionItem = null;
        }
    }

    private void InitializeUnit(Unit unit)
    {
        unit.country = GameManager.Instance.myCountry;
        GameManager.Instance.myCountry.units.Add(unit);
    }
}
