using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    private LevelManager levelManager;

    void Start() {
        levelManager = GetComponent<LevelManager>();
    }

    public void addPlanet(int x, int y, float r, int ID) {
        levelManager.instantiatePlanet(x, y, r, ID);
    }
}