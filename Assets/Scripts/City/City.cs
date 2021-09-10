using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviourPun, ISelectable, IPunObservable
{
    public Country ownerCountry;
    public CityStats cityStats = new CityStats();

    public Production production;

    public bool isSelected;

    private readonly List<Hexagon> hexagons = new List<Hexagon>();
    public int hexagonCount;

    private void Start()
    {
        production = new Production();
        production.myCity = this;
        cityStats.onLeveledUp += CityLeveledUp;
    }

    public void AddHexagon(Hexagon hex)
    {
        hex.OwnerCountry = ownerCountry;
        hexagons.Add(hex);
        if (photonView.IsMine)
        {
            hexagonCount++;
        }
        UpdateCityStatsByHexagon(hex);
        //HexagonDataManager.Instance.GetComponent<PhotonView>().RPC("UpdateData", RpcTarget.AllBuffered, hex.hexPosition);
    }

    private void UpdateCityStatsByHexagon(Hexagon hex)
    {
        switch (hex.resourceType)
        {
            case ResourceType.Empty:
                break;
            case ResourceType.Food:
                cityStats.food++;
                break;
            case ResourceType.Rock:
                cityStats.mine++;
                break;
            case ResourceType.Forest:
                cityStats.wood++;
                break;
            default:
                break;
        }
    }

    public void InitializeCity(Country _ownerCountry)
    {
        ownerCountry = _ownerCountry;
        ownerCountry.cities.Add(this);
    }

    public void CityLeveledUp()
    {
        GrowCity();
        cityStats.population++;
    }

    private void GrowCity()
    {
        var shuffledHexagons = hexagons.OrderBy(x => Random.Range(0, hexagons.Count)).ToArray();
        foreach (var item in shuffledHexagons)
        {
            var l = HexManager.instance.GetNeighborHexagons(item.hexPosition, true);
            if (l.Length > 0)
            {
                AddHexagon(l[Random.Range(0, l.Length)]);
                break;
            }
        }
    }

    public void Select()
    {
        if (!isSelected && ownerCountry == GameManager.Instance.myCountry)
        {
            CityUiManager.Instance.InitializeUI(this);
            UISelectionManager.Instance.Open(CityUiManager.Instance.GetComponent<ISelectableUI>());
        }
    }

    public void Deselect()
    {
        UISelectionManager.Instance.CloseCurrent();
    }

    private void OnDestroy()
    {
        if (SelectionManager.selected == GetComponent<ISelectable>())
        {
            SelectionManager.selected = null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hexagons.Count);
            foreach (var item in hexagons)
            {
                stream.SendNext(item.hexPosition);
            }
        }
        else
        {
            hexagonCount = (int)stream.ReceiveNext();

            for (int i = 0; i < hexagonCount; i++)
            {
                Vector3 hexPos = (Vector3)stream.ReceiveNext();
                AddHexagon(HexManager.instance.allHexagons[hexPos]);
            }
        }
    }
}

[System.Serializable]
public class CityStats
{
    public delegate void LeveledUp();
    public event LeveledUp onLeveledUp;

    public void OnLeveledUp()
    {
        if (onLeveledUp != null)
        {
            onLeveledUp();
        }
    }

    public int growthLevel = 0;

    //Start with 1/5 (+1 every 5 turn)
    private int turnsLeftToGrow = 5;
    public int TurnsLeftToGrow
    {
        get => turnsLeftToGrow;
        set
        {
            turnsLeftToGrow = value;
            if (turnsLeftToGrow == 0)
            {
                turnsLeftToGrow = 5;
                growthLevel++;
                OnLeveledUp();
            }
            growthRate = 1.0f / turnsLeftToGrow;
        }
    }
    public float growthRate = 1 / 5f;

    public int population = 1;

    public float happiness = 1;

    //Resources
    public int food = 1;
    public int mine = 1;
    public int wood = 1;

    //War
    public int attack = 1;
    public int health = 10;
    public int maxHealth = 10;
    public int defense = 1;
    public int maxDefense = 1;

    public int healtIncreaseAmount = 1;
    public int culture;

    public CityStats()
    {
        TurnsLeftToGrow = turnsLeftToGrow;
    }

    public void IncreaseHealth()
    {

        if (health + healtIncreaseAmount <= maxHealth)
        {
            health += healtIncreaseAmount;
        }
        else
        {
            health = maxHealth;
        }
    }

    public void CalculateHappiness()
    {
        happiness = (food + mine + wood) / (population * 1.0f);
    }
}