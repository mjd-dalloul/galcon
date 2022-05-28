using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int ID;
    private List<int> planets;
    private LevelManager levelManager;
    public GameObject shipPrefab;
    public Color color;
    //todo change to player ID
    private int layer = 8;

    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        planets = new List<int>();
    }

    public void attack(int sourceID, int targetID) {
        levelManager.startAttack(sourceID, targetID);
    }

    public void stopAttacking(int sourceID, int targetID) {
        levelManager.stopAttacking(sourceID, targetID);

    }

    public bool isAlly(int target) {
        foreach (int planet in planets)
            if (planet == target)return true;
        return false;
    }

    public void addPlanet(int p) {
        planets.Add(p);
    }
    public void removePlanet(int p) {
        planets.Remove(p);
    }
    public void setID(int ID) {
        this.ID = ID;
    }
    public int getID() {
        return ID;
    }

    public void setLayer(int layer) {
        this.layer = layer;
    }
    public int getLayer() {
        return layer;
    }
}