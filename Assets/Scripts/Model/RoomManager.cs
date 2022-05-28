using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    private GameObject[] PlayerUI;
    private Server server;
    private int currntPlayers = 0;
    GameSettings.GameType type;
    GamePrefrences gamePrefrences;

    void Start()
    {
        gamePrefrences = FindObjectOfType<GamePrefrences>();
        for (int i = 0; i < GameSettings.getPlayersCount(); i++)
        {
            transform.GetChild(i + 1).gameObject.SetActive(false);
        }
        startRoomManager();
        if (GameSettings.getGameType() == GameSettings.GameType.MULTIPLAYER)
        {
            server = Server.Instance();
            server.setRoomManager(this);
            server.getOpenRooms();
        }
        else
        {
            add_bots();
        }
    }

    public void openRoomsCB(Room[] rooms){
        foreach(Room r in rooms) {
            if(r.name == GameSettings.getRoomName()) {
                updateRoomInfo(r);
                break;
            }
        }
    }

    public void clearView(){
        for (int i = 0; i < GameSettings.MaxPlayersCount; i++)
        {
            startWaitingPlayer(i);
        }
   }

    private void updateRoomInfo(Room r) {
        

        GameSettings.setSeed(r.seed);
        GameSettings.setPlayersCount(r.players.Length);
        
        clearView();
        int readyPlayersCount = 0;

        for(int i=0 ; i<r.players.Length ; i++)
        {
            Debug.Log("player i = " + i + "  name : " + r.players[i].name);
            
            if(r.players[i].name == PlayerPrefs.GetString("username", ""))
            {
                Debug.Log("Found mainPlayer...");
                GameSettings.setPlayerType(i, GameSettings.PlayerType.MAIN_PLAYER);
                GameSettings.setMainPlayerId(i);
            }
            else 
            {
                GameSettings.setPlayerType(i, GameSettings.PlayerType.NETWORK_PLAYER);
            }

            if(r.players[i].isReady){
                transform.GetChild(i+1).GetChild(3).gameObject.SetActive(true);
                readyPlayersCount++;
            }
            
            GameSettings.setPlayerName(i, r.players[i].name);

            add_Player(i);            
        }

        if(readyPlayersCount == r.players.Length) {
            StartGame();
        }
    }

    private void add_bots()
    {
        for (int ID = 0; ID < GameSettings.getPlayersCount(); ID++)
        {
            if (GameSettings.isBot(GameSettings.getPlayerType(ID)))
            {
                GameSettings.setPlayerName(ID, "Bot-" + ID);
                transform.GetChild(ID+1).GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                GameSettings.setPlayerName(ID, PlayerPrefs.GetString("username", "Main Player"));
            } 
            add_Player(ID);
        }
    }

    public void startRoomManager()
    {
        int playersCount = GameSettings.MaxPlayersCount;
        PlayerUI = new GameObject[playersCount];
        for (int i = 0; i < playersCount; i++)
        {
            PlayerUI[i] = transform.GetChild(i + 1).gameObject;
            startWaitingPlayer(i);
        }
    }

    private void startWaitingPlayer(int ID)
    {
        PlayerUI[ID].SetActive(true);
        PlayerUI[ID].transform.GetChild(0).gameObject.SetActive(false);
        PlayerUI[ID].transform.GetChild(1).gameObject.SetActive(true);
        PlayerUI[ID].transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Waiting for players...";
        PlayerUI[ID].transform.GetChild(2).gameObject.SetActive(true);
        PlayerUI[ID].transform.GetChild(3).gameObject.SetActive(false);
    }

    public void add_Player(int ID)
    {
        currntPlayers++;
        PlayerUI[ID].transform.GetChild(0).gameObject.SetActive(true);
        PlayerUI[ID].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = gamePrefrences.PlanetSprite;
        PlayerUI[ID].transform.GetChild(0).gameObject.GetComponent<Image>().color = GameSettings.getColor(ID);

        PlayerUI[ID].transform.GetChild(1).gameObject.SetActive(true);
        PlayerUI[ID].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = GameSettings.getPlayerName(ID);
        
        PlayerUI[ID].transform.GetChild(2).gameObject.SetActive(false);

    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReadyOnClick(GameObject readyBtn)
    {
        //Todo set player is ready
        if (readyBtn.GetComponent<Image>().color == Color.green)
        {
            //set player to ready
            readyBtn.GetComponent<Image>().color = Color.red;
            readyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "not ready";
        }
        else
        {
            readyBtn.GetComponent<Image>().color = Color.green;
            readyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ready";
        }

        if(GameSettings.getGameType() == GameSettings.GameType.MULTIPLAYER) {
            server.toggleReady();
        }
        else {
            StartGame();
        }

    }
}
