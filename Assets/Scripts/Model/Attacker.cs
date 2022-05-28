using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Attacker : MonoBehaviour {
    List<Tuple<int, int>> attacks;
    float time = 0, limit = 0.03f;

    void Start() {
        attacks = new List<Tuple<int, int>>();
    }
    int tt = 0;
    void Update() {
        time += Time.deltaTime;
        while (time >= limit)
        {
            time -= limit;
            foreach (Tuple<int, int> attack in attacks) {
                LevelManager.getPlanet(attack.Item1).sendShip(LevelManager.getPlanet(attack.Item2));
            }
        }
    }
    public void modifyAttacks(int souceID, int targetID, bool attack) {
        if (attack) {
            attacks.Add(new Tuple<int, int>(souceID, targetID));
        } else {
            attacks.Remove(new Tuple<int, int>(souceID, targetID));
        }
    }
    public void removeAttacksFromSource(int sourceID) {
        // resolve any pending ships
        Update();
        attacks.RemoveAll(atk => atk.Item1 == sourceID);
    }

    public List<Tuple<int, int>> getCurrentAttacks()
    {
        return this.attacks;
    }

    public void setCurrentAttacks(List<Tuple<int, int>> a)
    {
         this.attacks=a;
    }
}