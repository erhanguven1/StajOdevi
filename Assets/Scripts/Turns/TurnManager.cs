using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    private void Awake()
    {
        instance = this;
    }

    public delegate void NewTurnStarted();
    public static event NewTurnStarted onNewTurnStarted;

    public void StartNewTurn()
    {
        if (onNewTurnStarted != null)
        {
            onNewTurnStarted();
        }
    }

    public List<Country> readyCountries = new List<Country>();

    [PunRPC]
    public void IncrementReadyCountries(int id)
    {
        Country country = GameManager.Instance.countries[id];
        readyCountries.Add(country);
        if (readyCountries.Count == GameManager.Instance.countries.Count)
        {
            readyCountries.Clear();
            StartNewTurn();
        }
    }
}
