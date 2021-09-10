using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using DG.Tweening;

public class Country : MonoBehaviourPun, IPunObservable
{
    public string countryName;
    public Color countryColor;
    public Button endTurnButton;

    public List<City> cities = new List<City>();
    public List<Unit> units = new List<Unit>();

    public bool turnHasEnded = false;

    public int id;

    public int totalAttack, totalHexagonCount;
    public int domination;

    private void Awake()
    {
        id = -1;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            GameManager.Instance.myCountry = this;

            countryColor = new Color(Random.Range(0, 255f) / 255, Random.Range(0, 255f) / 255, Random.Range(0, 255f) / 255);

            GameObject.Find("End Turn Button").GetComponent<Button>().onClick.AddListener(OnTapEndTurn);

            TurnManager.onNewTurnStarted += NewTurnStarted;

            AddMyCountryToList();
        }
    }

    public void AddMyCountryToList()
    {
        StartCoroutine(waitAndAdd());
        IEnumerator waitAndAdd()
        {
            yield return new WaitForSeconds(.1f);

            GameManager.Instance.AddCountry(this, true);

            var settler = PhotonNetwork.Instantiate("Settler", HexManager.instance.allHexagons.Values.ToList().Rand().hexPosition + Vector3.up, Quaternion.identity);
            settler.GetComponent<Unit>().country = this;
            units.Add(settler.GetComponent<Unit>());

            CameraController.Instance.MoveTo(settler);
        }
    }
    [PunRPC]
    public void SetID(int _id, bool first = true)
    {
        id = _id;
        if (first)
        {
            if (photonView.IsMine)
            {
                GameManager.Instance.myCountry = this;
            }
            else
            {
                AddOtherCountryToList();
            }
        }
    }

    private void OnGUI()
    {
        if (photonView.IsMine)
        {
            GUILayout.Label(id.ToString());
        }
    }

    void AddOtherCountryToList()
    {
        GameManager.Instance.AddCountry(this);
    }

    private void Update()
    {
        name = id.ToString();
    }

    public void OnTapEndTurn()
    {
        turnHasEnded = true;
        GameObject.Find("End Turn Button").GetComponent<Button>().interactable = false;

        TurnManager.instance.GetComponent<PhotonView>().RPC("IncrementReadyCountries", RpcTarget.AllBuffered, id);
    }

    private void NewTurnStarted()
    {
        foreach (var city in cities)
        {
            city.cityStats.IncreaseHealth();
            city.cityStats.CalculateHappiness();
            city.cityStats.TurnsLeftToGrow--;
        }

        GameObject.Find("End Turn Button").GetComponent<Button>().interactable = true;
        turnHasEnded = false;

        if (photonView.IsMine)
        {
            CalculateDomination();
        }
    }

    private void CalculateDomination()
    {
        int hexCount = 0;
        int totalCityDamage = 0;

        foreach (var item in cities)
        {
            hexCount += item.hexagonCount;
            totalCityDamage += item.cityStats.attack;
        }

        int totalUnitStrength = 0;

        foreach (var item in units)
        {
            totalUnitStrength += item.damage;
        }

        domination = hexCount * 10 + totalCityDamage + totalUnitStrength;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(countryName);
            stream.SendNext(new Vector3(countryColor.r, countryColor.g, countryColor.b));
            stream.SendNext(turnHasEnded);
            stream.SendNext(domination);
        }
        else
        {
            countryName = (string)stream.ReceiveNext();

            var c = (Vector3)stream.ReceiveNext();
            countryColor = new Color(c.x, c.y, c.z);

            turnHasEnded = (bool)stream.ReceiveNext();
            domination = (int)stream.ReceiveNext();
        }
    }
}
