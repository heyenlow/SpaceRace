using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public enum GameType
    {
        NotSet,
        Multiplayer,
        Singleplayer,
        Tutorial,
        Watch
    };

    public enum AIDifficulty
    {
        NotSet,
        Easy,
        Med,
        Hard
    };
    public enum NetworkMode
    {
        NotSet,
        Host,
        Join
    };

    public static GameType gameType { get; set; }
    public static AIDifficulty difficulty { get; set; }
    public static NetworkMode netMode { get; set; }
}
