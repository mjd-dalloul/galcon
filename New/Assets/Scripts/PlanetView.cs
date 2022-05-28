using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetView : MonoBehaviour {
    private TextMesh shipsCountText;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private LineRenderer line;
    void Start() {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        shipsCountText = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>();
        animator = GetComponent<Animator>();
        line = GetComponent<LineRenderer>();
    }

    public void setShipsCount(string cnt) {
        shipsCountText.text = cnt;
    }

    public void setBoolForAnimation(string s, bool b) {
        animator.SetBool(s, b);

    }

    public void changePlanetColor(Color color) {
        spriteRenderer.color = color;

    }

}