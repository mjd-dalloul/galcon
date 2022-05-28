using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameSettings;

public class LevelManager : MonoBehaviour {

    public static GameObject planetPrefab;

    private static Player[] players;
    private static MainPlayer human;
    private static List<Planet> planets;
    private static Server server;
    private static Attacker attacker;
    private static int mainPlayerID;
    private static GameType gameMode;
    private static int playersCount;
    private static int planetsCount;
    private static GameController gameController;
    private static SaveLoadManager.GameData gameData;
    public static SaveLoadManager.GameData GameData { get => gameData; set => gameData = value; }
    
    void Awake() {
        planetPrefab = (GameObject)Resources.Load("Prefabs/Planet", typeof(GameObject));
    }
    
    void Start() {        
        gameController = FindObjectOfType<Canvas>().GetComponent<GameController>();
        attacker = GetComponent<Attacker>();
        if(GameSettings.getGameType() == GameSettings.GameType.MULTIPLAYER)
        {
            server = Server.Instance();
            server.setAttacker(attacker);
        }

        gameMode = GameSettings.getGameType();
        mainPlayerID = GameSettings.getMainPlayerId();
        playersCount = GameSettings.getPlayersCount();
        planetsCount = GameSettings.getPlanetsCount();
        
        players = new Player[playersCount];
        planets = new List<Planet>(planetsCount);

        // create players
        for (int ID = 0; ID < playersCount; ID++)
        {
            switch (GameSettings.getPlayerType(ID))
            {
                case GameSettings.PlayerType.MAIN_PLAYER:
                    players[ID] = instantiate_MainPlayer(ID).GetComponent<Player>();
                    players[ID].playerLayer = 9 + ID;
                    break;
                case GameSettings.PlayerType.NETWORK_PLAYER:
                    players[ID] = instantiate_NetworkPlayer(ID).GetComponent<Player>();
                    players[ID].playerLayer = 9 + ID;
                    break;
                default:
                    players[ID] = instantiate_BotPlayer(ID).GetComponent<Player>();
                    players[ID].playerLayer = 9 + ID;
                    break;
            }
        }

        // create map
        var planetsInfo = MapGenerator.generate(planetsCount, playersCount, GameSettings.getSeed());
            instantiate_Map(planetsInfo);

        // start bots
        for (int i = 0; i < playersCount; i++)
        {
            if (GameSettings.isBot(GameSettings.getPlayerType(i)))
            {
                players[i].GetComponent<Bot>().strartBotThread();
            }
        }
        
    }

    private static MainPlayer instantiate_MainPlayer(int id)
    {
        GameObject g = Instantiate((GameObject)Resources.Load("Prefabs/Human", typeof(GameObject)));
        g.GetComponent<Player>().setID(id);
        g.GetComponent<Player>().setColor(GameSettings.getColor(id));
        return g.GetComponent<MainPlayer>();
    }
    private static Bot instantiate_BotPlayer(int id)
    {
        GameObject g = Instantiate((GameObject)Resources.Load("Prefabs/Bot", typeof(GameObject)));
        g.GetComponent<Player>().setID(id);
        g.GetComponent<Player>().setColor(GameSettings.getColor(id));
        g.GetComponent<Bot>().setDifficulity(GameSettings.getPlayerType(id));
        return g.GetComponent<Bot>();
    }
    private static Player instantiate_NetworkPlayer(int id)
    {
        GameObject g = Instantiate((GameObject)Resources.Load("Prefabs/Player", typeof(GameObject))); 
        g.GetComponent<Player>().setID(id);
        g.GetComponent<Player>().setColor(GameSettings.getColor(id));
        return g.GetComponent<Player>();
    }
    private static Planet instantiate_Planet(int ID, float x, float y, float r, int shipsCount){
        GameObject go = Instantiate(planetPrefab, new Vector3(x, y, -1), Quaternion.identity);
        Planet planet = go.GetComponent<Planet>();
        planet.setRadius(r);
        planet.setID(ID);
        planet.setShipsCount(shipsCount);
        return planet;
    }
    private static void instantiate_Map(MapGenerator.PlanetInfo[] planetsInfo) {
        for(int i=0 ; i<planetsCount ; i++)
        {
            var p = planetsInfo[i];
            Planet planet = instantiate_Planet(i, p.x, p.y, p.r, p.s);
            if (i < playersCount) {
                planet.changeOwner(players[i]);
            }
            planets.Insert(i, planet);
        }
        // get saved map
        restore_map();
    }

    static void restore_map()
    {
        if (GameSettings.getGameType() == GameType.SAVED_PRACTICE)
        {
            // To prevent player from loosing while changing planets owenrs
            foreach (var p in players)
            {
                p.incFlaotingShips();
            }

            // get saved planets
            for (int i = 0; i < planetsCount; i++)
            {
                planets[i].setShipsCount(gameData.shipsCount[i]);
                if (gameData.planetsType[i] == PlanetType.CONCURRED)
                    planets[i].changeOwner(getPlayer(gameData.planetsOwner[i]));
            }
            //get saved ships
            GameData.ships.ForEach(ship =>
            {
                getPlanet(ship.sourceID).instantiateShip(getPlanet(ship.targetID), new Vector3(ship.x, ship.y, -1.0f));
            });

            // Undo temporary floatingShips changes
            foreach (var p in players)
            {
                p.decFloatingShips();
            }
        }
    }
    
    public static void startAttack(int sourceID, int targetID)
    {
        if (gameMode == GameSettings.GameType.MULTIPLAYER)
        {
            server.AttackNotifyServer(sourceID, targetID, true);
        }
        else
        {
            attacker.modifyAttacks(sourceID, targetID, true);
        }
    }

    public static void stopAttack(int sourceID, int targetID)
    {
        if (gameMode == GameSettings.GameType.MULTIPLAYER)
        {
            server.AttackNotifyServer(sourceID, targetID, false);
        }
        else
        {
            attacker.modifyAttacks(sourceID, targetID, false);
        }
    }

    public static void cancelAttack(int ID) {
        attacker.removeAttacksFromSource(ID);
    }

    public static void removePlayer(Player p)
    {
        playersCount--;
        players[p.getID()] = null;
        
        if (p.getID() == mainPlayerID) {
            
            gameController.ActivateGameStop(GameController.GameStopType.LOSE);
        }
        else if((playersCount) == 1 && players[mainPlayerID] != null){
            gameController.ActivateGameStop(GameController.GameStopType.WIN);
            if(GameSettings.getGameType() == GameSettings.GameType.MULTIPLAYER) {
                server.winRoom();
            }
        }
    }

    
    public static List<Planet> getPlanets() => planets;
    public static List<Tuple<int, int>> getCurrentAttacks() => attacker.getCurrentAttacks();
    public static Player getPlayer(int id) => players[id];
    public static Planet getPlanet(int id) => planets[id];


    public static void saveGame()
    {
        SaveLoadManager.saveData();
    }
    

}