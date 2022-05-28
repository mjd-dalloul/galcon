using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Ship : MonoBehaviour {
    [Range(1f, 200f)]
    public float speed;

    public SpriteRenderer spriteRenderer;

    public Player owner;
    public GameObject source;
    public Planet sourcePlanet;
    private GameObject target;
    private Animator animator;
    private Rigidbody2D rigid;
    private Vector3 targetPos;
    private Vector3 initTargetPos;
    private CircleCollider2D _collider;


    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _collider = GetComponent<CircleCollider2D>();

        transform.Rotate(0, 0, GetAngle(targetPos));
        
    }

    public Tuple<Vector3, Vector3> getNewTargetPosX2(Planet otherPlanet, RaycastHit2D hit) {
        float radius = otherPlanet.getRadius();

        float dist = Vector3.Distance(this.transform.position, otherPlanet.getPosition());

        float theta = Mathf.Atan2(radius, dist) * 2.25f;

        Vector3 targetVec = new Vector3(hit.point.x - transform.position.x
            , hit.point.y - transform.position.y, 0);

        Vector3 newTargetVec = new Vector3(targetVec.x * Mathf.Cos(theta) - targetVec.y * Mathf.Sin(theta)
            , targetVec.x * Mathf.Sin(theta) + targetVec.y * Mathf.Cos(theta), targetPos.z);

        Vector3 newTargetPosR = new Vector3(transform.position.x + newTargetVec.x
            , transform.position.y + newTargetVec.y, transform.position.z);

        newTargetVec = new Vector3(targetVec.x * Mathf.Cos(-theta) - targetVec.y * Mathf.Sin(-theta)
                    , targetVec.x * Mathf.Sin(-theta) + targetVec.y * Mathf.Cos(-theta), targetPos.z);

        Vector3 newTargetPosL = new Vector3(transform.position.x + newTargetVec.x
            , transform.position.y + newTargetVec.y, transform.position.z);

        return new Tuple<Vector3, Vector3>(newTargetPosL, newTargetPosR);
    }

    void FixedUpdate() {
        if (!_collider.isTrigger) {
            if (targetPos == initTargetPos)
                rigid.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime;
            else {
                rigid.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime * 1.5f;
            }
        }


        float targetDist = Vector3.Distance(transform.position, targetPos);
        if (Physics2D.Raycast(transform.position, initTargetPos - transform.position
            , 20, 1).collider == null || targetDist <= 1.0f) {
            targetPos = initTargetPos;
            return;
        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPos - transform.position, 1.0f, 1);
        if (hit.collider != null) {
            GameObject other = hit.collider.gameObject;
            Planet otherPlanet = null;
            if (other == null) {
                //print("other is null");
            } else if (other == target || other == source) {
                //print("ignor hit");
            } else {
                otherPlanet = other.GetComponent<Planet>();
                if (otherPlanet == null) {
                    return;
                }

                Tuple<Vector3, Vector3> LR = getNewTargetPosX2(otherPlanet, hit);
                Vector3 newTargetPosL = LR.Item1;
                Vector3 newTargetPosR = LR.Item2;

                float distR = Vector3.Distance(otherPlanet.getPosition(), newTargetPosR);
                float distL = Vector3.Distance(otherPlanet.getPosition(), newTargetPosL);

                if (distR >= distL
                    && Vector3.Distance(sourcePlanet.getPosition(), newTargetPosR) - 5
                    < Vector3.Distance(sourcePlanet.getPosition(), initTargetPos)) {
                    targetPos = newTargetPosR;
                    transform.eulerAngles = Vector3.forward * GetAngle(targetPos);
                } else
                if (distR < distL
                    && Vector3.Distance(sourcePlanet.getPosition(), newTargetPosL) - 5
                    < Vector3.Distance(sourcePlanet.getPosition(), initTargetPos)) {
                    targetPos = newTargetPosL;
                    transform.eulerAngles = Vector3.forward * GetAngle(targetPos);
                }

            }
        } else {
            targetPos = initTargetPos;
            transform.eulerAngles = Vector3.forward * GetAngle(targetPos);
        }
    }


    public GameObject getSource() {
        return source;
    }

    public Player getOwner() {
        return owner;
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
    ///Setters
    public void setOwner(Player player) {
        owner = player; 
        spriteRenderer.color = owner.color;
        gameObject.layer = player.playerLayer;
    }
    public void setSource(GameObject source) {
        this.source = source;
        this.sourcePlanet = source.GetComponent<Planet>();
    }
    public void setTarget(GameObject target) {
        this.target = target;
        targetPos = target.transform.position;
        this.initTargetPos = targetPos;
    }
    public GameObject getTarget() {
        return target;
    }
    public void destroyShip() {
        owner.decFloatingShips();
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collisionInfo) {
        GameObject other = collisionInfo.gameObject;
        if (other == target) {
            Planet targetPlanet = target.GetComponent<Planet>();
            targetPlanet.shipsEntered(owner.getID());
            if (targetPlanet.getOwner() != owner) {
                spriteRenderer.color = Color.white;
                animator.SetTrigger("Exploed");
                _collider.isTrigger = true;
            } else {
                destroyShip();
            }
        } else if (other.GetComponent<Ship>() != null) {
            Ship otherShip = other.GetComponent<Ship>();
            if (target != otherShip.getTarget()) {
                Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }
}