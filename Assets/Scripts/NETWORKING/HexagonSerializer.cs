using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class HexagonSerializer : InstancablePUN<HexagonSerializer>, IPunObservable
{
    public Dictionary<Vector3, Hexagon> hexagons => HexManager.instance.allHexagons;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            foreach (var item in hexagons)
            {
                stream.SendNext(item.Value.id);
                stream.SendNext(item.Value.resourceType);
                stream.SendNext(item.Value.OwnerCountry ? item.Value.OwnerCountry.id : -1);
            }
        }
        else
        {
            foreach (var item in hexagons)
            {
                item.Value.id = (int)stream.ReceiveNext();
                item.Value.SetResourceType((ResourceType)stream.ReceiveNext());
                int ownerCountryId = (int)stream.ReceiveNext();
                if (ownerCountryId != -1)
                {
                    StartCoroutine(waitForAFrame());
                    IEnumerator waitForAFrame()
                    {
                        yield return new WaitForEndOfFrame();
                        item.Value.OwnerCountry = GameManager.Instance.countries.FirstOrDefault(x => x.id == ownerCountryId);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void UpdateHexagonIsEmpty(int id, bool isEmpty)
    {
        var hex = hexagons.Values.FirstOrDefault(x => x.id == id);
        hex.SetIsEmptyManual(isEmpty);
    }

    [PunRPC]
    public void UpdateHexagonOwnerCountry(int id, int ownerCountryId)
    {
        var hex = hexagons.Values.FirstOrDefault(x => x.id == id);
        hex.SetOwnerCountryManual(GameManager.Instance.countries.FirstOrDefault(x => x.id == ownerCountryId));
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("UpdateHexagon", RpcTarget.MasterClient, 0);
        }
    }
}
