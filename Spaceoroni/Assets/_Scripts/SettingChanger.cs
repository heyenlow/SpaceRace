using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingChanger : MonoBehaviour
{

    //SET GAME TYPE FUNCTIONS ------------------------------------
    public void setGameTypeMultiplayer() { GameSettings.gameType = GameSettings.GameType.Multiplayer; }
    public void setGameTypeTutorial() { GameSettings.gameType = GameSettings.GameType.Tutorial; }
    public void setGameTypeWatch() { GameSettings.gameType = GameSettings.GameType.Watch; }
    public void setGameTypeSinglePlayer() { GameSettings.gameType = GameSettings.GameType.Singleplayer; }
    public static void setGameTypeReset() { GameSettings.gameType = GameSettings.GameType.NotSet; }

    //SET NETWORKING MODE FUNCTIONS ------------------------------------
    public void setNetworkingModeJoin () { GameSettings.netMode = GameSettings.NetworkMode.Join; }
    public void setNetworkingModeHost() { GameSettings.netMode = GameSettings.NetworkMode.Host; }
    public static void setNetworkingModeReset() { GameSettings.netMode = GameSettings.NetworkMode.NotSet; }


    //SET NETWORKING MODE FUNCTIONS ------------------------------------
    public void setAIDifficultyEasy() { GameSettings.difficulty = GameSettings.AIDifficulty.Easy; }
    public void setAIDifficultyMedium() { GameSettings.difficulty = GameSettings.AIDifficulty.Med; }
    public void setAIDifficultyHard() { GameSettings.difficulty = GameSettings.AIDifficulty.Hard; }
    public static void setAIDifficultyReset() { GameSettings.difficulty = GameSettings.AIDifficulty.NotSet; }

    //RESET ALL TO NOTSET
    public static void resetGameSettings()
    {
        setGameTypeReset();
        setAIDifficultyReset();
        setNetworkingModeReset();
    }

}
