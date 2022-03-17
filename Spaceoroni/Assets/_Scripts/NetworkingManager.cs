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

    private List<Listing> _listings = new List<Listing>(); 

    #endregion

    [SerializeField]
    private GameObject HostName;
    [SerializeField]
    private GameObject NewtorkingInfo;
    [SerializeField]
    private GameObject WaitingForPlayer;
    [SerializeField]
    private GameObject Connecting;
    [SerializeField]
    private GameObject Join;
    [SerializeField]
    private GameObject Host;
    [SerializeField]
    private GameObject hostRoomName;
    [SerializeField]
    private Transform RoomListingsContent;
    [SerializeField]
    private Listing listing;

    [SerializeField]
    private Game gameManager;


    private string netinfo = "";

    public const byte RAISE_TURN = 1;
    public const byte RAISE_INITIAL_BUILDERS = 2;
    public const byte RAISE_INITIAL_BUILDER = 3;
    

    #region Monobehaviour CallBacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    private void Update()
    {
        NewtorkingInfo.GetComponent<TextMeshProUGUI>().text = netinfo + " " + PhotonNetwork.CountOfRooms.ToString();
    }


    #region Public Methods

    public void OnClick_Multiplayer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            Connecting.SetActive(true);
            Join.SetActive(false);
            Host.SetActive(false);
        }
        else
        {
            Connecting.SetActive(false);
            Join.SetActive(true);
            Host.SetActive(true);
        }

    }

    public void HostGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            options.IsVisible = true;

            string name = hostRoomName.GetComponent<TMP_InputField>().text + "_"+ System.Guid.NewGuid().ToString();

            Debug.Log("CreateRoom called with name:" + name);
            PhotonNetwork.CreateRoom(name, options);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToCenterEarth();

        }
        else
        {
            Debug.Log("Not Connected");
        }
       
    }

    public static void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
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

            Connecting.SetActive(false);
            Join.SetActive(true);
            Host.SetActive(true);
        }

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

        public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        isConnecting = false;

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Room failed.");   
    }

    public override void OnJoinedRoom()
    {
        
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

        RoomListingsContent.DestroyChildren();

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();
            gameManager.StartGame();
        }

    }

    public override void OnLeftRoom()
    {
        RoomListingsContent.DestroyChildren();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("You Created a Room");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
       if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            WaitingForPlayer.SetActive(false);
            gameManager.StartGame();
        }
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() ", otherPlayer);
        RoomListingsContent.DestroyChildren();
        gameManager.QuitGame();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("OnRoomListUpdate called by PUN..." );

        foreach (RoomInfo info in roomList)
        {
            //Removed from rooms list.
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                    Debug.Log("Room name, " + info.Name + ", has been removed from roomList");
                }
            }

            //Added to rooms list.
            else
            {
                Listing newListing = Instantiate(listing, RoomListingsContent);
                if (listing != null)
                {
                    newListing.SetRoomInfo(info);
                    _listings.Add(newListing);
                    Debug.Log("Room name, " + info.Name + ", has been added to roomList ");
                }
            }
        }

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("You joined a PUN lobby! ");
    }

    #endregion
}
