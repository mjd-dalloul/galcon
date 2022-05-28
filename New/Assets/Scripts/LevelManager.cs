using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public GameObject planetPrefab;
    private Player[] players;
    private MainPlayer human;
    private List<Planet> planets;
    private Server server;
    private Attacker attacker;

    void Start() {
        players = FindObjectsOfType<Player>();
        human = FindObjectOfType<MainPlayer>();
        planets = new List<Planet>(FindObjectsOfType<Planet>());
        server = FindObjectOfType<Server>();
        attacker = GetComponent<Attacker>();
        IDForPlayers();
        IDForPlanets();
        generateMap();
    }

    private void IDForPlayers() {
        for (int i = 1; i < players.Length; i++) {
            players[i].setID(i);
        }
    }
    private void IDForPlanets() {
        for (int i = 0; i < planets.Count; i++)
            planets[i].setID(i);
    }

    public void startAttack(int sourceID, int targetID) {
        //without server
        attacker.modifyAttacks(sourceID, targetID, true);
        //with server
        //server.AttackNotifyServer(sourceID, targetID, true);
    }

    public void stopAttacking(int sourceID, int targetID) {
        //without server
        attacker.modifyAttacks(sourceID, targetID, false);
        //with server
        //server.AttackNotifyServer(sourceID, targetID, false);
    }

    private void generateMap() {
        for (int i = 0; i < players.Length; i++) {
            players[i].addPlanet(planets[i].getID());
            planets[i].setOwner(players[i]);
        }
    }

    public void cancelAttack(int ID) {
        attacker.removeAttack(ID);
    }

    public MainPlayer getMainPlayer() {
        return human;
    }

    public Planet getPlanet(int ID) {
        return planets[ID];
    }

    public Player getPlayer(int ID) {
        return players[ID];
    }

    public void instantiatePlanet(int x, int y, float r, int ID) {
        GameObject g = Instantiate(planetPrefab, new Vector3(x, y, -1), Quaternion.identity);
        Planet planet = g.GetComponent<Planet>();
        planet.setRadius(r);
        planet.setID(ID);
        planets[ID] = planet;
    }
    public List<Planet> getPlanets() {
        return planets;
    }
}