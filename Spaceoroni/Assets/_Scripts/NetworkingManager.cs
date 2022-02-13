using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    #endregion

    #region Private Fields
    bool isConnecting;
    string gameVersion = "1";

    #endregion

    [SerializeField]
    private GameObject MainMenuPanel;
    [SerializeField]
    private GameObject HostName;
    [SerializeField]
    private GameObject NewtorkingInfo;
    private string netinfo = "";

    #region Monobehaviour CallBacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        MainMenuPanel.SetActive(true);
    }
    #endregion

    private void Update()
    {
        NewtorkingInfo.GetComponent<TextMeshProUGUI>().text = netinfo + " " + PhotonNetwork.CountOfRooms.ToString();
    }




    #region Public Methods

    public void ConnectToPUN()
    {
        if(!PhotonNetwork.IsConnected)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void HostGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("CreateRoom called with name:");// + HostName.GetComponent<TextMeshPro>().text);
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            
        }
        else
        {
            Debug.Log("Not Connected");
        }
       
    }

    public void JoinGame()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
            netinfo += "Joined Room";
        }
    }
    #endregion


    #region MonoBehaviorPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        if (isConnecting)
        {
            isConnecting = false;
            Debug.Log("Connected to master");
        }
    }

        public override void OnDisconnected(DisconnectCause cause)
    {
        
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        //PhotonNetwork.LoadLevel("Main");
        MainMenuPanel.SetActive(false);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCameraFromStartScreenToGameBoard();
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("You Created a Room");
    }

    #endregion
}
