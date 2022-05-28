using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePrefrences : MonoBehaviour
{
    public enum BackroundType{
        DARK, STARS
    }

    public enum PlanetSpriteType
    {
        MARS, FAIRY, FANTA
    }

    public Sprite PlanetSprite, marsSprite, FairySprite, FantaSprite;
    public Sprite BackroundDark;
    public Sprite BackroundStars;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent< SpriteRenderer>();
        SetBackround((BackroundType) PlayerPrefs.GetInt("BACKROUND", 1));
        SetPlanetSprite((PlanetSpriteType) PlayerPrefs.GetInt("PLANET_SPRITE", 0));
    }

    public void SetBackround(BackroundType type)
    {
        if(type == BackroundType.DARK)
        {
            spriteRenderer.transform.localScale = new Vector3(0.36f, 0.36f, 1.0f);
            spriteRenderer.sprite = BackroundDark;
        }
        else if (type == BackroundType.STARS)
        {
            spriteRenderer.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
            spriteRenderer.sprite = BackroundStars;
        }
    }

    public void SetPlanetSprite(PlanetSpriteType type)
    {
        if (type == PlanetSpriteType.MARS)
        {
            PlanetSprite = marsSprite;
        }
        else if (type == PlanetSpriteType.FAIRY)
        {
            PlanetSprite = FairySprite;
        }
        else if (type == PlanetSpriteType.FANTA)
        {
            PlanetSprite = FantaSprite;
        }
    }
}
