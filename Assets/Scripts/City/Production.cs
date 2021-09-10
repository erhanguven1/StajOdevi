using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Production
{
    public List<ProductionItem> productionItems = new List<ProductionItem>();
    public ProductionItem currentProductionItem;
    public City myCity;

    public void Initialize()
    {
        foreach (var item in productionItems)
        {
            item.onClick += OnTapProductionItem;
            item.onFinished -= OnProductionHasFinished;
            item.onFinished += OnProductionHasFinished;
        }
    }

    public void Close()
    {
        foreach (var item in productionItems)
        {
            item.onClick -= OnTapProductionItem;
            
        }
        //currentProductionItem = null;
        productionItems.Clear();
    }

    private void OnTapProductionItem(ProductionItem item)
    {
        
        if (currentProductionItem == null)
        {
            Debug.Log("Tapped " + item.type.ToString());
            currentProductionItem = item;
            ProductionManager.Instance.producingItems.Add(item);
        }
    }

    private void OnProductionHasFinished(ProductionItem item)
    {
        ProductionManager.Instance.OnProductionHasFinished(myCity, this);
    }
}
public enum ProductionType { Improvements, Units }
public enum ProductionItemType { Monument, Settler, Builder, Warrior }
[System.Serializable]
public class ProductionItem
{
    public ProductionType productionType;
    public UnityEngine.UI.Button btn;

    public ProductionItemType type;

    public delegate void OnClick(ProductionItem item);
    public event OnClick onClick;

    public delegate void OnFinished(ProductionItem item);
    public event OnFinished onFinished;

    //How many turn it needs to finish?
    public int productionTime;

    public int turnsLeft;

    public void InitializeButton(UnityEngine.UI.Button _btn)
    {
        btn = _btn;
        btn.onClick.AddListener(OnTap);
    }

    public void OnTap()
    {
        if (onClick != null)
        {
            onClick(this);
        }
    }

    public void NewTurn()
    {
        if (turnsLeft == 0)
        {
            turnsLeft = productionTime;
        }
        turnsLeft--;
        if (turnsLeft <= 0)
        {
            if (onFinished != null)
            {
                onFinished(this);
            }
        }
    }
}