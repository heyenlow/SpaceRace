using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public enum GameType
    {
        Multiplayer,
        Singleplayer,
        Tutorial,
        Watch
    };

    public enum AIDifficulty
    {
        Easy,
        Med,
        Hard,
        NotSet
    };
    public enum NetworkMode
    {
        Host,
        Join,
        NotSet
    };

    public static GameType gameType { get; set; }
    public static AIDifficulty difficulty { get; set; }
    public static NetworkMode netMode { get; set; }
}
