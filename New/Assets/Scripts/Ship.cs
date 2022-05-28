using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Ship : MonoBehaviour {
    [Range(1f, 100f)]
    public float speed;

    //todo fix null exception when make it private and try to get component
    public SpriteRenderer spriteRenderer;

    public Player owner;
    private GameObject target;
    private Animator animator;
    private Rigidbody2D rigid;
    private Vector3 targetPos;

    //if max ship over all the map <= 300 ship change it to polygon collider for more accurcy
    private CircleCollider2D _collider;

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _collider = GetComponent<CircleCollider2D>();
        transform.Rotate(0, 0, GetAngle(targetPos));
    }

    void Update() {

        transform.eulerAngles = Vector3.forward * GetAngle(targetPos);
    }
    void FixedUpdate() {
        if (!_collider.isTrigger)
            rigid.velocity = (targetPos - transform.position).normalized * speed * Time.deltaTime;

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
            if (target != other.GetComponent<Ship>().getTarget())
                Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        }
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
        gameObject.layer = owner.getLayer();
        spriteRenderer.color = owner.color;
    }
    public void setTarget(GameObject target) {
        this.target = target;
        targetPos = target.transform.position;
    }
    public GameObject getTarget() {
        return target;
    }
    public void destroyShip() {
        Destroy(gameObject);
    }
}