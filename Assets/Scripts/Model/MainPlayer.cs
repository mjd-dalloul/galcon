using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour {

    private List<int> selection;
    public Player player;
    private bool isAttacking = false;

    private void Start() {
        selection = new List<int>();
        player = GetComponent<Player>();
        // player.setLayer(9);
    }

    public void startAttack(int target) {
        foreach(int source in selection) {
            if(source != target) {
                LevelManager.startAttack(source, target);
            }
        }
        isAttacking = true;
    }
    public void stopAttack(int target) {
        foreach(int source in selection) {
            if(source != target) {
                LevelManager.stopAttack(source, target);
            }
        }
        isAttacking = false;
    }

    // call on each planet.changeOwner()
    public void validateSelection() {
        selection.RemoveAll(src => LevelManager.getPlanet(src).getOwner() != player);
    }

    public void addToSelection(int planetId) {
        if(!selection.Contains(planetId)) {
            selection.Add(planetId);
        }
    }

    public void removeFromSelection(int planetId) {
        if(selection.Contains(planetId)) {
            selection.Remove(planetId);
        }
    }

    public void SelectAll() {
        foreach(int plantId in player.planets){
            addToSelection(plantId);
            LevelManager.getPlanet(plantId).setSelection(true);
        }
    }

    public void clearSelection() {
        foreach(int planetId in selection) {
            LevelManager.getPlanet(planetId).setSelection(false);
        }
        selection.Clear();
    }
    public bool hasSelected(int planetId) => selection.Contains(planetId);
}