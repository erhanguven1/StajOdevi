using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class GameManager : InstancablePUN<GameManager>
{
    public int countryCount;
    public List<Country> countries = new List<Country>();
    public Country myCountry;

    public delegate void AddedCountry(int countryId, bool isMine);
    public static event AddedCountry onAddedCountry;

    public delegate void OnCountryHasDisconnected(int id);
    public static event OnCountryHasDisconnected onCountryHasDisconnected;

    bool fetched;
    public bool Fetched
    {
        get => fetched;
        set
        {
            fetched = value;
            if (fetched == true)
            {
                StartCoroutine(waitAndAdd());
                IEnumerator waitAndAdd()
                {
                    yield return new WaitForEndOfFrame();
                    var country = PhotonNetwork.Instantiate("Country", new Vector3(0, 0, Random.Range(-2, 2)), Quaternion.identity);
                }

            }
        }
    }

    public void AddCountry(Country country, bool isMine = false)
    {
        countries.Add(country);
        if(isMine)
        {
            country.photonView.RPC("SetID", RpcTarget.AllBuffered, countryCount - 1, true);
        }
        if (onAddedCountry != null)
        {
            onAddedCountry(country.id, isMine);
        }

        //After every other country added, add ours to the list
        if (!Fetched && countries.Count == countryCount - 1)
        {
            Fetched = true;
        }
    }

    [PunRPC]
    public void IncreaseCountryCount()
    {
        countryCount++;
    }
}
