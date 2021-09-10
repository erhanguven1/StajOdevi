using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Empty, Food, Rock, Forest }

public class ResourcesGenerator : Instancable<ResourcesGenerator>
{
    public Sprite[] textures;

    public void GenerateResource(Hexagon hex)
    {
        int r = Random.Range(-3, 10);

        if (r >= -3 && r < 5)
        {
            hex.SetResourceType(ResourceType.Empty);
        }
        else if (r < 7)
        {
            hex.SetResourceType(ResourceType.Food);
        }
        else if (r < 9)
        {
            hex.SetResourceType(ResourceType.Forest);
        }
        else
        {
            hex.SetResourceType(ResourceType.Rock);
        }
    }
}
