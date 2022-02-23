using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PUNGameManager : MonoBehaviourPunCallbacks
{
    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level");
        PhotonNetwork.LoadLevel("Main");
    }

    public override void OnLeftRoom()
    {
        Debug.LogFormat("Player left room");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() ", other);

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient", PhotonNetwork.IsMasterClient);
        }
    }
}
