using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Range(1, 999)]
    public int maxShips;
    [Range(1, 999)]
    public int shipsCount;

    public Player owner;
    private LevelManager levelManager;
    private float radius;
    public int ID;
    private bool getAttacked = false;
    private double pressedTime = 0;
    private PlanetView view;
    private Vector3 position;

    void Start() {
        shipsCount = maxShips;
        getComponents();
        findObjects();
        setText();
        fixCircleCollider();
        owner = null;
        position = transform.position;
        //todo replace it
        InvokeRepeating("increaseShips", 1f, 1 / radius);
    }

    private void setText() {
        view.setShipsCount(shipsCount.ToString());
    }
    private void getComponents() {
        view = GetComponent<PlanetView>();
    }

    private void findObjects() {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void fixCircleCollider() {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        radius = transform.GetChild(0).transform.localScale.x * 1.6666667f;
        print("SSS" + radius);
        //transform.GetChild(0).transform.localScale = new Vector3(radius / 1.6666667f, radius / 1.6666667f, radius / 1.6666667f);
        circleCollider.radius = radius;
    }

    private void increaseShips() {
        if (shipsCount < maxShips)shipsCount++;
        view.setShipsCount(shipsCount.ToString());
    }

    private void OnMouseDown() {
        pressedTime = 0;
    }
    private void OnMouseUp() {
        MainPlayer mainPlayer = levelManager.getMainPlayer();
        if (getAttacked) {
            getAttacked = false;
            mainPlayer.stopAttacking(ID);
            return;
        }
        if (mainPlayer.player == owner) {
            if (mainPlayer.alreadySelected(ID)) {
                mainPlayer.removeFromHolder(ID, -1, false);
            } else {
                mainPlayer.addToHolder(ID);
                view.setBoolForAnimation("selected", true);
            }
        }
    }

    void OnMouseDrag() {
        MainPlayer mainPlayer = levelManager.getMainPlayer();
        pressedTime += Time.deltaTime;
        if (pressedTime >.3f && !getAttacked) {
            getAttacked = true;
            mainPlayer.attackPlanet(ID);
        }
    }

    public void cancelSelect() {
        view.setBoolForAnimation("selected", false);
    }

    public void startAttacking(Planet target) {
        instantiateShips(target);
        //StartCoroutine("instantiateShips", target);
    }

    private void instantiateShips(Planet target) {
        Vector3 position2Instantiate = getPosition(target.gameObject.transform.position);
        position2Instantiate.z = -1;
        GameObject g;
        if (shipsCount <= 0)
            return;
        g = Instantiate(owner.shipPrefab, position2Instantiate, Quaternion.identity);
        Ship ship = g.GetComponent<Ship>();
        ship.setOwner(owner);
        ship.setTarget(target.gameObject);
        g.transform.SetParent(transform);
        shipsCount--;
        view.setShipsCount(shipsCount.ToString());
    }

    public void shipsEntered(int attackerID) {
        //todo cancel select
        Player attacker = levelManager.getPlayer(attackerID);
        if (attacker == owner)shipsCount++;
        else {
            shipsCount--;
            if (shipsCount <= 0) {
                shipsCount -= shipsCount;
                changeOwner(attacker);
            }
        }
        view.setShipsCount(shipsCount.ToString());
    }

    private void changeOwner(Player newOwner) {
        //todo make planet stop attack
        if (owner != null)
            owner.removePlanet(ID);
        owner = newOwner;
        owner.addPlanet(ID);
        levelManager.cancelAttack(ID);
        view.changePlanetColor(owner.color);
    }

    ///setters
    public void setOwner(Player owner) {
        this.owner = owner;
        view.changePlanetColor(owner.color);
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public void setRadius(float r) {
        radius = r;
        fixCircleCollider();
    }

    ///Getters
    public Player getOwner() {
        return owner;
    }

    public int getID() {
        return ID;
    }

    public int getShipsCount() {
        return shipsCount;
    }

    public bool isNeutal() {
        return owner == null;
    }
    public float getRadius() {
        return radius;
    }

    public Vector3 getPosition() {
        return position;
    }

    ///helper function
    private Vector3 getPosition(Vector3 position) {
        return rotationFormula(GetAngle(position) * Mathf.Deg2Rad);
    }
    private Vector3 rotationFormula(float angle) {
        float shift = 0f;
        return new Vector3(-Mathf.Sin(angle) * (radius + shift) + transform.position.x, Mathf.Cos(angle) * (radius + shift) + transform.position.y, -1f);
    }
    private float GetAngle(Vector3 position) {
        float xDist = position.x - transform.position.x;
        float yDist = position.y - transform.position.y;
        float angle = 0.0f;
        if (0 < xDist && 0 <= yDist) {
            float dist = Mathf.Abs(yDist) / Mathf.Abs(xDist);
            angle = Mathf.Rad2Deg * (Mathf.Atan(dist));
            angle = angle + 270;
        } else if (0 <= xDist && 0 > yDist) {
            float dist = Mathf.Abs(xDist) / Mathf.Abs(yDist);
            angle = Mathf.Rad2Deg * (Mathf.Atan(dist));
            angle = angle + 180;
        } else if (0 >= xDist && 0 > yDist) {
            float dist = Mathf.Abs(xDist) / Mathf.Abs(yDist);
            angle = Mathf.Rad2Deg * (Mathf.Atan(dist));
            angle = 180 - angle;
        } else {
            float dist = Mathf.Abs(yDist) / Mathf.Abs(xDist);
            angle = Mathf.Rad2Deg * (Mathf.Atan(dist));
            angle = 90 - angle;
        }
        return angle;
    }

}