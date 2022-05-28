using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenRoomsController : MonoBehaviour
{
    public GameObject RoomPrefab;
    public GameObject PlayerUIPrefab;
    public Transform RoomContentTransform;
    private AudioManager audioManager;
    private Server server;
    
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        server = FindObjectOfType<Server>();
        server.setOpenRoomsController(this);
        server.getOpenRooms();
    }

    public void clearRooms()
    {
        for (int i = 0; i < RoomContentTransform.childCount; i++)
        {
            Destroy(RoomContentTransform.GetChild(i).gameObject);
        }
    }

    void addRoom(Room room)
    {
        GameObject roomUI = Instantiate(RoomPrefab, RoomContentTransform, false);
        roomUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.name; 
        roomUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = room.players.Length.ToString() + " Players";
        roomUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate 
            {
                JoinRoomOnClick(room);
            });
        Transform PlayerContentTransform = roomUI.transform.GetChild(4).GetChild(0).GetChild(0).gameObject.transform;
        for (int i = 0; i < room.players.Length; i++)
        {
            GameObject PlayerUI = Instantiate(PlayerUIPrefab, PlayerContentTransform, false);
            PlayerUI.GetComponent<TextMeshProUGUI>().text = room.players[i].name;
            PlayerUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (room.players[i].isReady ? "Ready" : "");
        }
    }

    public void JoinRoomOnClick(Room room)
    {
        PlayClickSound();
        server.joinRoom(room.name);
        GameSettings.setRoomName(room.name);
        SceneManager.LoadScene("RoomScene");
    }
    public void openRoomsCB(Room[] rooms){
        clearRooms();
        foreach(Room r in rooms) {
            addRoom(r);
        }
    }
    public void NewRoomOnClick()
    {
        PlayClickSound();
        server.newRoom();
        // SceneManager.LoadScene("RoomScene");
    }

    public void BackOnClick()
    {
        PlayClickSound();
        SceneManager.LoadScene("MainScene");
    }

    void PlayClickSound()
    {
        audioManager.PlayClickSound();
    }
}
