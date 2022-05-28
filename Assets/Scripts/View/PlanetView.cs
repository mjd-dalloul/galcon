using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetView : MonoBehaviour {
    private TextMesh shipsCountText;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private LineRenderer line;
    private GameObject selectSpriteGO;
    private SpriteRenderer selectSprite;
    private GamePrefrences gamePrefrences;
    void Awake() {
        gamePrefrences = FindObjectOfType<GamePrefrences>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = gamePrefrences.PlanetSprite;
        spriteRenderer.color = Color.cyan;
        shipsCountText = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>();
        selectSpriteGO = transform.GetChild(0).transform.GetChild(1).gameObject;
        selectSprite = selectSpriteGO.GetComponent<SpriteRenderer>();
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

    public void setSelecteType(SelectType selectType)
    {
        switch (selectType)
        {
            case SelectType.ATTACK:
                {
                    if(!selectSpriteGO.activeSelf)
                        selectSpriteGO.SetActive(true);
                    selectSprite.color = Color.red;
                    break;
                }
            case SelectType.SUPPORT:
                {
                    if (!selectSpriteGO.activeSelf)
                        selectSpriteGO.SetActive(true);
                    selectSprite.color = Color.green;
                    break;
                }
            case SelectType.SELECT:
                {
                    if (!selectSpriteGO.activeSelf)
                        selectSpriteGO.SetActive(true);
                    selectSprite.color = Color.cyan;
                    break;
                }
            case SelectType.CANCEL:
                {
                    selectSpriteGO.SetActive(false);
                    break;
                }


        }
    }

    public enum SelectType
    {
        ATTACK, SUPPORT, SELECT, CANCEL
    }

}