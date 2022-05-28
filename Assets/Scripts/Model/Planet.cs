using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Planet : MonoBehaviour {
    // todo : move to GameSettings
    [Range(1, 999)]
    public int maxShips;

    [Range(1, 999)]
    public int shipsCount;

    public int ID;
    public Player owner = null;
    private float radius;
    private Vector3 position;
    
    
    private bool isAttacked = false;
    // todo ?!
    private double dragTime = 0;
    private double idleTime = 0;
    private int fastClicksCount = 0;
    public PlanetView view;
	private float time;
    private GameSettings.PlanetType type = GameSettings.PlanetType.NEUTRAL;
    public GameSettings.PlanetType Type { get => type;}

    void Awake() {
        view = GetComponent<PlanetView>();
        // trigger updating the view text
        setShipsCount(shipsCount);
        position = transform.position;
    }
	
	void Update()
    {
        time += Time.deltaTime;
        while (time >= 0.6f * 1/this.radius)
        {
            time -= 0.6f * 1/this.radius;
            if (!isNeutal()) {
                setShipsCount(Mathf.Min(shipsCount+1, maxShips));
            }
        }

        if(dragTime == 0) {
            idleTime += Time.deltaTime;
        } else {
            idleTime = 0;
        }

        if(idleTime > .3f) {
            fastClicksCount = 0;
        }
    }

    /*
    private void fixCircleCollider() {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        transform.GetChild(0).transform.localScale = new Vector3(radius / 1.6666667f, radius / 1.6666667f, radius / 1.6666667f);
        circleCollider.radius = radius;
    }

    private void OnMouseDown() {
        if(LevelManager.getPlayer(GameSettings.getMainPlayerId()) == null) return;
        dragTime = 0;
    }
    */

    void OnMouseDrag() {
        if(LevelManager.getPlayer(GameSettings.getMainPlayerId()) == null) return;
        MainPlayer mainPlayer = LevelManager.getPlayer(GameSettings.getMainPlayerId()).GetComponent<MainPlayer>();
        dragTime += Time.deltaTime;
        if (dragTime >.3f && !isAttacked) {
            isAttacked = true;
            mainPlayer.startAttack(ID);
        }
        
        if (isAttacked) {
            if (owner == mainPlayer.player) {
                view.setSelecteType(PlanetView.SelectType.SUPPORT);
            }
            else {
                view.setSelecteType(PlanetView.SelectType.ATTACK);
            }
        }
    }
    private void OnMouseUp() {
        if(LevelManager.getPlayer(GameSettings.getMainPlayerId()) == null) return;
        MainPlayer mainPlayer = LevelManager.getPlayer(GameSettings.getMainPlayerId()).GetComponent<MainPlayer>();
        if (isAttacked) {
            isAttacked = false;
            view.setSelecteType(PlanetView.SelectType.CANCEL);
            mainPlayer.stopAttack(ID);
        }
        else if (mainPlayer.player == owner) {
            if (mainPlayer.hasSelected(ID)) {
                view.setSelecteType(PlanetView.SelectType.CANCEL);
                mainPlayer.removeFromSelection(ID);
            } else {
                view.setSelecteType(PlanetView.SelectType.SELECT);
                mainPlayer.addToSelection(ID);
            }
        }

        if(dragTime < .3f) {
            fastClicksCount++;
        } else {
            fastClicksCount = 0;
        }

        if(fastClicksCount == 2) {
            fastClicksCount = 0;
            mainPlayer.SelectAll();
        }

        dragTime = 0;
    }

    public void setSelection(bool select) {
        if(select) {
            view.setSelecteType(PlanetView.SelectType.SELECT);
        } else {
            view.setSelecteType(PlanetView.SelectType.CANCEL);
        }
    }
    public void sendShip(Planet target) {
        instantiateShip(target);
    }

    private void instantiateShip(Planet target) {
        if (shipsCount <= 0) return;
        shipsCount--;
        setShipsCount(shipsCount);

        Vector3 position2Instantiate = getPosition(target.gameObject.transform.position);
        position2Instantiate.z = -1;
        
        GameObject g = Instantiate(owner.shipPrefab, position2Instantiate, Quaternion.identity, transform);
        Ship ship = g.GetComponent<Ship>();
        
        ship.setOwner(owner);
        ship.setSource(this.gameObject);
        ship.setTarget(target.gameObject);

        owner.incFlaotingShips();
    }

    public void instantiateShip(Planet target, Vector3 position2Instantiate)
    {
        GameObject g = Instantiate(owner.shipPrefab, position2Instantiate, Quaternion.identity, transform);
        Ship ship = g.GetComponent<Ship>();

        ship.setOwner(owner);
        ship.setSource(this.gameObject);
        ship.setTarget(target.gameObject);

        owner.incFlaotingShips();
    }

    public void shipsEntered(int attackerID) {
        Player attacker = LevelManager.getPlayer(attackerID);
        if (attacker == owner) {
            shipsCount++;
        }
        else {
            shipsCount--;
            if (shipsCount < 0) {
                shipsCount = -shipsCount;
                changeOwner(attacker);
            }
        }
       setShipsCount(shipsCount);
    }

    public void changeOwner(Player newOwner) {
        // todo : delete this after testing
        Player oldOwner = this.owner;
        this.owner = newOwner;
        if(oldOwner == newOwner) {
            return;
        }
        else if (newOwner == null) {
            type = GameSettings.PlanetType.NEUTRAL;
            if (oldOwner != null) oldOwner.removePlanet(ID);
            LevelManager.cancelAttack(ID);
            view.setSelecteType(PlanetView.SelectType.CANCEL);
            view.changePlanetColor(Color.white);
        }
        else {
            type = GameSettings.PlanetType.CONCURRED;
            if (oldOwner != null) oldOwner.removePlanet(ID);
            newOwner.addPlanet(ID);
            LevelManager.cancelAttack(ID);
            view.setSelecteType(PlanetView.SelectType.CANCEL);
            view.changePlanetColor(newOwner.color);


        }

        if(oldOwner != null && oldOwner.getID() == GameSettings.getMainPlayerId()) {
            LevelManager.getPlayer(GameSettings.getMainPlayerId())
                .GetComponent<MainPlayer>()
                .validateSelection();
        }
    }

    public void setShipsCount(int shipsCount)
    {
        this.shipsCount = shipsCount;
        // if (isNeutal() || owner.getID() == GameSettings.getMainPlayerId())
        {
            view.setShipsCount(shipsCount.ToString());
        }
        // else
        {
            // view.setShipsCount("");
        }
    }


    ///setters

    public void setID(int ID) => this.ID = ID;

    public void setRadius(float r) {
        radius = r;
        // fixCircleCollider
        {
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            transform.GetChild(0).transform.localScale = new Vector3(radius / 1.6666667f, radius / 1.6666667f, radius / 1.6666667f);
            circleCollider.radius = radius;
        }
    }

    ///Getters
    public Player getOwner() => owner;
    public int getID() => ID;
    public int getShipsCount() => shipsCount;
    public bool isNeutal() => owner == null;
    public float getRadius() => radius;
    public Vector3 getPosition() => position;

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