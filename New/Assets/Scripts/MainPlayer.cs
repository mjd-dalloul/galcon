using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour {

    public static int IDMainPlayer = 0;
    private List<int> holder;
    private int ID;
    public Player player;
    private LevelManager levelManager;

    private void Start() {
        holder = new List<int>();
        player = GetComponent<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        player.setLayer(9);
    }
    public void attackPlanet(int target) {
        List<int> p = new List<int>();
        foreach (int source in holder) {
            if (player != levelManager.getPlanet(source).getOwner())
                p.Add(source);
            else {
                print("11111");
                if (source != target) {
                    print("222");
                    print(source + " " + target);
                    player.attack(source, target);
                }
            }
        }
        foreach (int planet in p) {
            removeFromHolder(planet, target, false);
        }
    }

    public void stopAttacking(int targetID) {
        clearHolder(targetID);
    }

    public void clearHolder(int targetID) {
        for (int i = holder.Count - 1; i >= 0; i--) {
            player.stopAttacking(holder[i], targetID);
            levelManager.getPlanet(holder[i]).cancelSelect();
            holder.RemoveAt(i);
        }
    }

    public void addToHolder(int planetID) {
        holder.Add(planetID);
    }

    public void removeFromHolder(int planetID, int targetID, bool stopAttack) {
        if (stopAttack)player.stopAttacking(planetID, targetID);
        holder.Remove(planetID);
        levelManager.getPlanet(planetID).cancelSelect();
    }

    public bool alreadySelected(int target) {
        foreach (int planet in holder)
            if (planet == target)
                return true;
        return false;
    }
}