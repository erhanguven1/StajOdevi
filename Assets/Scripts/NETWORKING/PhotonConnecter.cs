using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnecter : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom("room", roomOptions);
    }

    public override void OnJoinedRoom()
    {

        StartCoroutine(waitAndCreate());
        IEnumerator waitAndCreate()
        {
            GameManager.Instance.photonView.RPC("IncreaseCountryCount", RpcTarget.AllBuffered);
            yield return new WaitForEndOfFrame();
            if (GameManager.Instance.countryCount == 1)
            {
                var country = PhotonNetwork.Instantiate("Country", new Vector3(0, 0, Random.Range(-2, 2)), Quaternion.identity);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            HexManager.instance.Initialize();
        }
        else
        {
            StartCoroutine(wait());
            IEnumerator wait()
            {
                yield return new WaitForEndOfFrame();
                HexManager.instance.Initialize();
            }
        }
    }

}
