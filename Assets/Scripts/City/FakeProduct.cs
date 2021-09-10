using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Product/Fake Product", order = 1)]
public class FakeProduct : ScriptableObject
{
    public ProductionType productionType;
    public ProductionItemType itemType;

    //How many turn it needs to finish?
    public int productionTime;
}
