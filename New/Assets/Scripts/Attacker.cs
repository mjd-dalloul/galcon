using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Attacker : MonoBehaviour {
    List<Tuple<int, int>> attacks;
    LevelManager levelManager;

    void Start() {
        attacks = new List<Tuple<int, int>>();
        levelManager = GetComponent<LevelManager>();
    }
    void Update() {
        foreach (Tuple<int, int> attack in attacks) {
            levelManager.getPlanet(attack.Item1).startAttacking(levelManager.getPlanet(attack.Item2));
        }
    }
    public void modifyAttacks(int souceID, int targetID, bool attack) {
        if (attack) {
            attacks.Add(new Tuple<int, int>(souceID, targetID));
        } else {
            attacks.Remove(new Tuple<int, int>(souceID, targetID));
        }
    }
    public void removeAttack(int sourceID) {
        for (int i = 0; i < attacks.Count; i++) {
            if (attacks[i].Item1 == sourceID) {
                attacks.RemoveAt(i);
            }
        }
    }
}