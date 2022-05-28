using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class SaveLoadManager : MonoBehaviour
{
   
    public static void saveData()
    {
        GameData gameData = new GameData();
        gameData.seed = GameSettings.getSeed();
        gameData.playersCount = GameSettings.getPlayersCount();
        gameData.planetsCount = GameSettings.getPlanetsCount();
        gameData.mainPlayerId = GameSettings.getMainPlayerId();

        //players
        gameData.playersType = new GameSettings.PlayerType[gameData.playersCount];
        gameData.playersName = new string[gameData.playersCount];
        for (int i = 0; i < gameData.playersCount; i++)
        {
            gameData.playersType[i] = GameSettings.getPlayerType(i);
            gameData.playersName[i] = GameSettings.getPlayerName(i);

        }

        // Planets
        gameData.shipsCount= new int[gameData.planetsCount];
        gameData.planetsOwner = new int[gameData.planetsCount];
        gameData.planetsType = new GameSettings.PlanetType[gameData.planetsCount];
        for (int i = 0; i < gameData.planetsCount; i++)
        {
            gameData.shipsCount[i] = LevelManager.getPlanet(i).getShipsCount();
            gameData.planetsType[i] = LevelManager.getPlanet(i).Type;
            if (gameData.planetsType[i] == GameSettings.PlanetType.CONCURRED)
            {
                gameData.planetsOwner[i] = LevelManager.getPlanet(i).getOwner().getID();
            }
        }

        // Ships
        Ship[] ships = FindObjectsOfType<Ship>();
        gameData.ships = new List<GameData.ShipInfo>();
        foreach(var ship in ships){
            GameData.ShipInfo info = new GameData.ShipInfo();
            info.x = ship.gameObject.transform.position.x;
            info.y = ship.gameObject.transform.position.y;
            info.sourceID = ship.getSource().GetComponent<Planet>().getID();
            info.targetID = ship.getTarget().GetComponent<Planet>().getID();
            gameData.ships.Add(info);
        }
        
        writeFile(gameData);
    }

   
   

    private static void writeFile(GameData gameData)
    {
        int last = PlayerPrefs.GetInt("last", 0);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/save"+last+".dat", FileMode.OpenOrCreate);
        bf.Serialize(file, gameData);
        file.Close();
    }

    private static GameData readFile(int l)
    {
        if (File.Exists(Application.persistentDataPath + "/save"+l+".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save"+l+".dat", FileMode.Open);
            GameData gameData = bf.Deserialize(file) as GameData;
            file.Close();
            return gameData;
        }
        else
        {
            return (null);
        }
    }

    public static bool loadData(int ll)
    {
        GameData gameData = readFile(ll);
        LevelManager.GameData = gameData;

        if (gameData == null)
        {
            return false;
        }
        else
        {
            GameSettings.setGameType(GameSettings.GameType.SAVED_PRACTICE);

            GameSettings.setSeed(gameData.seed);
            GameSettings.setPlayersCount(gameData.playersCount);
            GameSettings.setPlanetsCount(gameData.planetsCount);
            GameSettings.setMainPlayerId(gameData.mainPlayerId);

            for (int i = 0; i < gameData.playersCount; i++)
            {
                GameSettings.setPlayerType(i, gameData.playersType[i]);
                GameSettings.setPlayerName(i, gameData.playersName[i]);
            }

            return true;
        }
    }


   
    public static bool hasGame(int index)
    {
        return File.Exists(Application.persistentDataPath + "/save"+index+".dat");
    }
    


    public static void delete(int index)
    {
        File.Delete(Application.persistentDataPath + "/save" + index + ".dat");
    }



    [System.Serializable]
    public class GameData
    {
        [System.Serializable]
        public struct ShipInfo{
            public float x, y;
            public int sourceID, targetID;
        }
        public int playersCount, planetsCount, mainPlayerId;
        public List<ShipInfo> ships;
        public GameSettings.PlayerType[] playersType;
        public string[] playersName;
        public int seed;
        public int[] shipsCount, planetsOwner;
        public GameSettings.PlanetType[] planetsType;
    }
}
