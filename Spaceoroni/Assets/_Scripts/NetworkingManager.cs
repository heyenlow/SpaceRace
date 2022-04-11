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

    private int playersWaiting = 0;
    #endregion

    [SerializeField]
    private GameObject HostName;
    [SerializeField]
    private GameObject NewtorkingInfo;
    [SerializeField]
    private GameObject WaitingForPlayer;
    [SerializeField]
    private GameObject MultiplayerMenu;
    [SerializeField]
    private GameObject Connecting;
    [SerializeField]
    private GameObject UnableToConnect;
    [SerializeField]
    private GameObject Join;
    [SerializeField]
    private GameObject Host;
    [SerializeField]
    private GameObject HostMenu;
    [SerializeField]
    private GameObject hostRoomName;
    [SerializeField]
    private Transform RoomListingsContent;
    [SerializeField]
    private Listing listing;
    [SerializeField]
    private GameObject Chat;
    [SerializeField]
    private GameObject PlayerDisconnect;
    [SerializeField]
    private GameObject PostGamePanel;
    [SerializeField]
    private GameObject PostGamePanel_PlayerDisconnected;
    [SerializeField]
    private GameObject WaitingToPlayAgain;
    [SerializeField]
    private GameObject PostGamePanel_PlayAgain;
    [SerializeField]
    private GameObject ConnectionFailed;

    [SerializeField]
    private PanelFader PanelFader;
    [SerializeField]
    private Game gameManager;


    private string netinfo = "";
    private bool leftRoom = false;

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

        if(HostMenu.activeSelf)
        {
            if(hostRoomName.GetComponent<TMP_InputField>().text != "" && hostRoomName.GetComponent<TMP_InputField>().text != null)
            {
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    HostGame();
                    HostMenu.SetActive(false);
                    WaitingForPlayer.SetActive(true);
                }
            }
        }
       if(playersWaiting == 2)
        {
            notWaiting();
            playersWaiting--;
            gameManager.restartAfterMultiplayer = true;
            gameManager.RestartGame();
            WaitingToPlayAgain.SetActive(false);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();
        }
    }


    #region Public Methods

    public void OnClick_Multiplayer()
    {
        if (!PhotonNetwork.IsConnected && Application.internetReachability != NetworkReachability.NotReachable)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            Connecting.SetActive(true);
            Join.SetActive(false);
            Host.SetActive(false);
        }
        else if(PhotonNetwork.IsConnected)
        {
            Connecting.SetActive(false);
            Join.SetActive(true);
            Host.SetActive(true);
        }
        else if(!PhotonNetwork.IsConnected && Application.internetReachability == NetworkReachability.NotReachable)
        {
            Connecting.SetActive(false);
            Join.SetActive(false);
            Host.SetActive(false);
            UnableToConnect.SetActive(true);
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

    public void isWaiting()
    {
        if (PhotonNetwork.InRoom)
        {
            WaitingToPlayAgain.SetActive(true);
            bool waiting = true;
            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            if(hash.ContainsKey("waiting"))
            {
                hash.Remove("waiting");
                hash.Add("waiting", waiting);
            }
            else
            {
                hash.Add("waiting", waiting);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            if(PhotonNetwork.LocalPlayer.CustomProperties["waiting"].Equals(true))
            {
                Debug.Log("This value is true and the function worked properly!");
            }


        }
    }

    public void notWaiting()
    {
        bool waiting = false;
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        if (hash.ContainsKey("waiting"))
        {
            hash.Remove("waiting");
            hash.Add("waiting", waiting);
            if (playersWaiting > 0)
            {
                playersWaiting--;
                Debug.Log(playersWaiting);
            }
        }
        else
        {
            hash.Add("waiting", waiting);
            if(playersWaiting > 0)
            {
                playersWaiting--;
                Debug.Log(playersWaiting);
            }
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties["waiting"].Equals(false))
        {
            Debug.Log("This value is false and the function worked properly!");
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
            UnableToConnect.SetActive(false);
            Join.SetActive(true);
            Host.SetActive(true);
        }

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

        public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN because: " + cause);
        isConnecting = false;
        if (leftRoom)
        {
            gameManager.QuitGame();
            ConnectionFailed.SetActive(true);
            PanelFader.FadePanel(ConnectionFailed);
            leftRoom = false;
        }
        else if(Connecting)
        {
            Connecting.SetActive(false);
            UnableToConnect.SetActive(true);
        }
        else
        {
            ConnectionFailed.SetActive(true);
            PanelFader.FadePanel(ConnectionFailed);
        }

        if(playersWaiting > 0)
        {
            playersWaiting--;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

        RoomListingsContent.DestroyChildren();

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();
            Chat.SetActive(true);
            gameManager.StartGame();
        }

        notWaiting();

    }

    public override void OnLeftRoom()
    {
        RoomListingsContent.DestroyChildren();
        Chat.SetActive(false);
        leftRoom = true;
        Debug.Log("This Client has left a PUN room.");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("You Created a Room");
        notWaiting();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
       if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            WaitingForPlayer.SetActive(false);
            gameManager.StartGame();
            Chat.SetActive(true);
        }
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() ", otherPlayer);
        RoomListingsContent.DestroyChildren();
        if (PostGamePanel.activeSelf)
        {
            PostGamePanel_PlayAgain.SetActive(false);
            PostGamePanel_PlayerDisconnected.SetActive(true);
        }
        else
        {
            PlayerDisconnect.SetActive(true);
        }
        HighlightManager.unHighlightEverything();

        if(playersWaiting > 0)
        {
            playersWaiting--;
        }
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

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if(changedProps.ContainsKey("waiting"))
        {
            if (changedProps["waiting"].Equals(true))
            {
                playersWaiting++;
                Debug.Log("Players Waiting: " + playersWaiting);
            }
            else if(changedProps["waiting"].Equals(false) && playersWaiting > 0)
            {
                playersWaiting--;
                Debug.Log("Players Waiting: " + playersWaiting);
            }
        }
        else
        {
            Debug.Log("changed props did not contain waiting");
        }

    }

    #endregion
}
