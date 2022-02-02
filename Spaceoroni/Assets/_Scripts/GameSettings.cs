using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public enum GameType
    {
        Multiplayer,
        Singleplayer,
        Tutorial
    };

    public enum AIDifficulty
    {
        Easy,
        Med,
        Hard
    };
    public enum NetworkMode
    {
        Host,
        Join
    };

    public static GameType gameType { get; set; }
    public static AIDifficulty difficulty { get; set; }
    public static NetworkMode netMode { get; set; }
}
