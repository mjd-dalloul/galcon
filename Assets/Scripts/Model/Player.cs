using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int ID;

    private bool isReady;
    public bool IsReady { get => isReady; set => isReady = value; }

    private new string name;
    public string Name { get => name; set => Name = value; }

    public List<int> planets;
    public GameObject shipPrefab;
    public Color color;
    public int floatingShips = 0;
    public LayerMask playerLayer;

    void Awake() {
        planets = new List<int>();
        IsReady = false;
        name = "player";
    }

    void Update()
    {
        
    }

    public void incFlaotingShips() {
        this.floatingShips++;
    }
    public void decFloatingShips(){
        this.floatingShips--;
        checkLost();
    }
    public void addPlanet(int p) {
        planets.Add(p);
    }
    public void removePlanet(int p) { 
        planets.Remove(p);
        checkLost(); 
    }
    
    public void checkLost() {
        if(planets.Count == 0 && floatingShips == 0) {
            Invoke("removeThisPlayer", 0.7f);
        }
    }

    public void removeThisPlayer() {
        
        LevelManager.removePlayer(this);
        Bot bot = gameObject.GetComponent<Bot>();
        if(bot != null) {
            // todo 
            // bot.stopThread();
        }
        Destroy(gameObject);
    }

    public void setID(int id) => this.ID = id;
    public int getID() =>  this.ID;

    public void setColor(Color color) => this.color = color;

    public void setLayerMask(int mask) => playerLayer = mask;
}