using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class Server : MonoBehaviour {
    private SocketIOComponent socket;
    private static Server _instance;    

    void Awake() {
        socket = GetComponent<SocketIOComponent>();
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public static Server Instance() {
        return _instance;
    }

    void Start()
    {
        // AccountsManager
        socket.On("loginCB", loginCB);
        socket.On("registerCB", registerCB);
    
        // OpenRoomsContorller + RoomManager
        socket.On("openRoomsCB", openRoomsCB);

        // MainSceneController
        socket.On("xpCB", xpCB);
        socket.On("rankingCB", rankingCB);
        socket.On("emailCB", emailCB);
        

        // Attacker + Chat
        socket.On("AttackCB", AttackCB);
        socket.On("messageCB", messageCB);

        // Game Sync
        socket.On("getGameState", getGameState);
        socket.On("gameStateUpdate", gameStateUpdate);
        
        //errors
        socket.On("error", (err) => {
        });
    }

     // -------------------- AcccountsManager -----------------------
    private AccountsManager accountsManager;
    public void setAcccountsManager(AccountsManager acc) => this.accountsManager = acc; 
    
    public void login(string username, string password) {
        Dictionary<string, string> d = new Dictionary<string, string>();
        d["username"] = username;
        d["password"] = password;
        socket.Emit("login", new JSONObject(d));
    }
    public void loginCB(SocketIOEvent e) {
        var d = e.data.ToDictionary();
        var result = bool.Parse(d["result"]);
        var msg = d["msg"];
        this.accountsManager.loginCB(result, msg);
    }

    public void register(string email, string username, string password) {
        Dictionary<string, string> d = new Dictionary<string, string>();
        d["email"] = email;
        d["username"] = username;
        d["password"] = password;
        socket.Emit("register", new JSONObject(d));
    }
    public void registerCB(SocketIOEvent e) {
        Dictionary<string, string> d =  e.data.ToDictionary();
        bool result = bool.Parse(d["result"]);
        string msg = d["msg"];
        accountsManager.RegisterCB(result, msg);
    }


    // -------------------- OpenRoomsController + RoomManger -----------------------4
    private RoomManager roomManager;
    private OpenRoomsController openRoomsController;
    public void setOpenRoomsController(OpenRoomsController openRoomsController) {
        this.openRoomsController = openRoomsController;
    }
    public void setRoomManager(RoomManager roomManager) {
        this.roomManager = roomManager;
    }
    public void getOpenRooms() => socket.Emit("openRooms");
    public struct OpenRoomsWrapper { public Room[] rooms;}
    public void openRoomsCB(SocketIOEvent e) {
        var rooms = LitJson.JsonMapper.ToObject<OpenRoomsWrapper>(e.data.ToString()).rooms;
        if(this.openRoomsController != null) {
            this.openRoomsController.openRoomsCB(rooms);
        }
        if(this.roomManager != null) {
            this.roomManager.openRoomsCB(rooms);
        }
    }
    public void newRoom() => socket.Emit("newRoom");
    public void joinRoom(string roomName) {
        socket.Emit("joinRoom", new JSONObject("{\"roomName\":\"" + roomName + "\"}"));
    }
    public void leaveRoom() => socket.Emit("leaveRoom");

    public void toggleReady() => socket.Emit("toggleReady");


    // -------------------- Attacker -----------------------
    private Attacker attacker;
    public void setAttacker(Attacker attacker) {
        this.attacker = attacker;
    }

    public void AttackNotifyServer(int source, int target, bool action)
    {
        Dictionary<string, string> attackData = new Dictionary<string, string>();
        attackData["source"] = source.ToString();
        attackData["target"] = target.ToString();
        attackData["action"] = action.ToString();
        socket.Emit("AttackNotifyServer", new JSONObject(attackData));
    }
    public void AttackCB(SocketIOEvent e)
    {
        Dictionary<string, string> data = e.data.ToDictionary();
        int source = int.Parse(data["source"].ToString());
        int target = int.Parse(data["target"].ToString());
        bool action = bool.Parse(data["action"].ToString());
        attacker.modifyAttacks(source, target, action);
    }

    // -------------------- Chat -----------------------
    public void messageNotifyServer(int ID, string msg)
    {
        Dictionary<string, string> d = new Dictionary<string, string>();
        d["msg"] = msg;
        d["ID"] = ID.ToString();
        socket.Emit("messageNotifyServer", new JSONObject(d));
    }

    public void messageCB(SocketIOEvent e)
    {
        Dictionary<string, string> d = e.data.ToDictionary();
        int ID = int.Parse(d["ID"]);
        string msg = d["msg"];
        FindObjectOfType<ChatManager>().MessageCB(ID, msg);
    }

    public void startGameCB(SocketIOEvent e)
    {
        roomManager.StartGame();
    }

    // -------------------- Game Sync -----------------------

    public struct PlanetsWrapper{
        public int[] planets;
        public int[] owners;
        public GameSettings.PlanetType[] types;
    }
    public void getGameState(SocketIOEvent e) {
        
        if(LevelManager.getPlanets() == null) return;

        var planets = new int[GameSettings.getPlanetsCount()];
        var owners = new int[GameSettings.getPlanetsCount()];
        var types = new GameSettings.PlanetType[GameSettings.getPlanetsCount()];
        for(int i=0 ; i<planets.Length ; i++){
            planets[i] = LevelManager.getPlanet(i).getShipsCount();
            types[i] = LevelManager.getPlanet(i).Type;
            if(types[i] == GameSettings.PlanetType.CONCURRED) {
                owners[i] = LevelManager.getPlanet(i).getOwner().getID();
            }
        }
        PlanetsWrapper pw = new PlanetsWrapper();
        pw.planets = planets;
        pw.owners = owners;
        pw.types = types;
        var json = LitJson.JsonMapper.ToJson(pw);
        
        socket.Emit("gameState", new JSONObject(json));
    }

    public void gameStateUpdate(SocketIOEvent e) {

        
        var pw = LitJson.JsonMapper.ToObject<PlanetsWrapper>(e.data.ToString());
        var planets = pw.planets;
        var owners = pw.owners;
        var types= pw.types;

        if(LevelManager.getPlanets() == null) return;

        for(int i=0 ; i<planets.Length ; i++) {
            LevelManager.getPlanet(i).setShipsCount(planets[i]);
            Player owner = null;
            if(types[i] == GameSettings.PlanetType.CONCURRED) {
                owner = LevelManager.getPlayer(owners[i]);
            }
            LevelManager.getPlanet(i).changeOwner(owner);
        }
    }


    // -------------------- MainSceneController -----------------------
    private MainSceneController mainSceneController;
    public void setMainSceneController(MainSceneController mainSceneController) {
        this.mainSceneController = mainSceneController;
    }
    public void getEmail() => socket.Emit("getEmail");
    public void getXp() => socket.Emit("getXp");
    public void getRanking() => socket.Emit("getRanking");
    
    public void xpCB(SocketIOEvent e) {
        int xp = int.Parse(e.data.ToDictionary()["xp"]);        
        if(this.mainSceneController != null) {
            mainSceneController.xpCB(xp);
        }
    }
    public void emailCB(SocketIOEvent e) {
        string email = e.data.ToDictionary()["email"];        
        if(this.mainSceneController != null) {
            mainSceneController.emailCB(email);
        }
    }
    public struct RankWrapper {
        public struct Entry {
            public string username;
            public int xp;
        }
        public Entry[] ranking;
    }
    public void rankingCB(SocketIOEvent e) {
        var ranking = LitJson.JsonMapper.ToObject<RankWrapper>(e.data.ToString()).ranking;
        if(mainSceneController != null) {
            mainSceneController.rankingCB(ranking);
        }
    }
    public void winRoom() {
        socket.Emit("winRoom");
    }

    public void logout() {
        socket.Emit("logout");
    }

}