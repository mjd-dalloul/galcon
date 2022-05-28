using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private GameObject iconPref;
    AudioManager audioManager;
    GamePrefrences gamePrefrences;
    public Sprite sound, mute;
    public GameObject sliderGO, btn, dark, stars, mars, fairy, fanta;
    Slider slider;
    bool isMute;
    float lastVol;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gamePrefrences = FindObjectOfType<GamePrefrences>();
        slider = sliderGO.GetComponent<Slider>();
        slider.value = audioManager.getVolume();
        lastVol = slider.value;
        if(slider.value == 0)
        {
            isMute = true;
            btn.GetComponent<Image>().sprite = mute;
        }
        else
        {
            isMute = false;
            btn.GetComponent<Image>().sprite = sound;
        }

        themeOnClick(PlayerPrefs.GetInt("BACKROUND", 1));
        PlanetOnClick(PlayerPrefs.GetInt("PLANET_SPRITE", 0));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAudioClick()
    {
        print("sound click");
        isMute = !isMute;
        if (isMute)
        {
            btn.GetComponent<Image>().sprite = mute;
            slider.value = 0.0f;
        }
        else
        {
            btn.GetComponent<Image>().sprite = sound;
            slider.value = 1.0f;
            audioManager.PlayClickSound();
        }
        audioManager.setVolume(slider.value);

    }

    public void OnSliderChange()
    {
        if(isMute && slider.value != 0.0f)
        {
            btn.GetComponent<Image>().sprite = sound;
            isMute = false;
        }
        if(!isMute && slider.value == 0)
        {
            btn.GetComponent<Image>().sprite = mute;
            isMute = true;
        }
        
        audioManager.setVolume(slider.value);
        if (Mathf.Abs(lastVol - slider.value) >= 0.09f)
        {
            audioManager.PlayClickSound();
            lastVol = slider.value;
        }
    }


    public void logOut()
    {
        audioManager.PlayClickSound();
        PlayerPrefs.SetString("name", "");
        SceneManager.LoadScene("AccountScene");
    }

    public void OnBackClick()
    {
        audioManager.saveVolume(slider.value);
        audioManager.PlayClickSound();
        SceneManager.LoadScene("MainScene");
    }

    public void themeOnClick(int type)
    {
        GamePrefrences.BackroundType backType = (GamePrefrences.BackroundType) type;
        if (backType == GamePrefrences.BackroundType.DARK)
        {
            Image starsImg = stars.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorBlack = Color.black;
            colorBlack.a = 0.6f;
            starsImg.color = colorBlack;
            Image darkImg = dark.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorGreen = Color.green;
            colorGreen.a = 1.0f;
            darkImg.color = colorGreen;
        }
        else if (backType == GamePrefrences.BackroundType.STARS)
        {
            Image starsImg = stars.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorGreen = Color.green;
            colorGreen.a = 1.0f;
            starsImg.color = colorGreen;
            Image darkImg = dark.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorBlack = Color.black;
            colorBlack.a = 0.6f;
            darkImg.color = colorBlack;
        }
        PlayerPrefs.SetInt("BACKROUND", type);
        gamePrefrences.SetBackround(backType);
    }

    public void PlanetOnClick(int type)
    {
        GamePrefrences.PlanetSpriteType PlanetType = (GamePrefrences.PlanetSpriteType) type;
        if (PlanetType == GamePrefrences.PlanetSpriteType.FAIRY)
        {
            setAllBlack();
            Image fairyImg = fairy.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorGreen = Color.green;
            colorGreen.a = 1.0f;
            fairyImg.color = colorGreen;
        }
        else if (PlanetType == GamePrefrences.PlanetSpriteType.MARS)
        {
            setAllBlack();
            Image marsImg = mars.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorGreen = Color.green;
            colorGreen.a = 1.0f;
            marsImg.color = colorGreen;
            
        }
        else if (PlanetType == GamePrefrences.PlanetSpriteType.FANTA)
        {
            setAllBlack();
            Image fantaImg = fanta.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color colorGreen = Color.green;
            colorGreen.a = 1.0f;
            fantaImg.color = colorGreen;
        }
        PlayerPrefs.SetInt("PLANET_SPRITE", type);
        gamePrefrences.SetPlanetSprite(PlanetType);

    }

    void setAllBlack()
    {
        Color colorBlack = Color.black;
        colorBlack.a = 0.6f;
        Image marsImg = mars.transform.GetChild(0).gameObject.GetComponent<Image>();
        marsImg.color = colorBlack;
        Image fairyImg = fairy.transform.GetChild(0).gameObject.GetComponent<Image>();
        fairyImg.color = colorBlack;
        Image fantaImg = fanta.transform.GetChild(0).gameObject.GetComponent<Image>();
        fantaImg.color = colorBlack;
    }

}
