using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public enum GameType
    {
        PRACTICE, MULTIPLAYER, SAVED_PRACTICE
    }
    public enum PlayerType 
    {
        MAIN_PLAYER, NETWORK_PLAYER, NEWIBE_BOT, AMETURE_BOT, EXPERT_BOT, PROFFESIONAL_BOT, MASTER_BOT, LEGENDARY_BOT
    }
    public enum PlanetType
    {
        NEUTRAL, CONCURRED
    }

    public static double[][] playersConsts = new double[][]{
        new double[]{0.0, 0.0, 0.0, 0.0},
        new double[]{0.0, 0.0, 0.0, 0.0},
        new double[]{1.0, 1.0, 1.0, 1.0},
        new double[]{0.7, 0.5, 0.1, 5.0},
        new double[]{1.0, 0.3, 0.1, 3.0},
        new double[]{0.7, 0.5, 0.1, 2.0},
        new double[]{0.7, 0.3, 0.1, 1.0},
        new double[]{1.0, 0.1, 0.3, 0.9},
    };


    private static Color[] colors = new Color[]{Color.red, Color.blue, Color.yellow, Color.magenta, Color.green};

    private static string roomName;
    private static GameType gameType;
    private static PlayerType[] playersType;
    private static string[] playersName;
    private static int playersCount = 0;
    private static int maxPlayersCount = 5;
    public static int MaxPlayersCount { get => maxPlayersCount; set => maxPlayersCount = value; }
    private static int planetsCount = 20;
    private static int mainPlayerId;
    private static int seed = 25;

    public static bool isBot(PlayerType p) => ((int)p) > 1;
    
    // setters & getters 
    public static void setPlayersCount(int playersCount) {
        GameSettings.playersCount = playersCount;
        playersType = new PlayerType[playersCount];
        playersName = new string[playersCount];

    }
    public static void setPlanetsCount(int planetsCount) {
        GameSettings.planetsCount = planetsCount;
    }
    
    public static void setGameType(GameType gameMode) => GameSettings.gameType = gameMode;
    public static void setPlayerType(int id, PlayerType type) => playersType[id] = type;
    public static void setPlayerName(int id, string name) => playersName[id] = name;
    public static void setMainPlayerId(int id) => mainPlayerId = id;
    public static void setSeed(int s) => seed=s;
    public static void setRoomName(string s) => roomName=s;


    public static int getPlayersCount() => GameSettings.playersCount;
    public static int getPlanetsCount() => GameSettings.planetsCount;
    public static GameType getGameType() => GameSettings.gameType;
    public static PlayerType getPlayerType(int id) => playersType[id];
    public static string getPlayerName(int id) => playersName[id];
    public static Color getColor(int id) => colors[id];
    public static int getMainPlayerId() => mainPlayerId;
    public static int getSeed() => seed;
    public static string getRoomName() => roomName;

}

 