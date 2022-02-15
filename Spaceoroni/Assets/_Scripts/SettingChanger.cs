using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingChanger : MonoBehaviour
{
    public void setNetworkingModeJoin ()
    {
        GameSettings.netMode = GameSettings.NetworkMode.Join;
    }
    public void setNetworkingModeHost()
    {
        GameSettings.netMode = GameSettings.NetworkMode.Host;
    }

    public void setGameTypeMultiplayer()
    {
        GameSettings.gameType = GameSettings.GameType.Multiplayer;
    }
}
