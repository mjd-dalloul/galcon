using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject Dialog;
    public Button send;
    public InputField input;

    private GameObject panel;
    private GameObject panelDIA;
    private GameObject gameOver;
    private GameObject watch;
    private GameObject mainMenue;
    private GameObject rematch;
    private GameObject save;
    private GameObject load;
    private GameObject dontBtn;
    private GameObject saveBtn;
    private GameObject inputSave;
    private AudioManager audioManager;
    
    private bool pause = false;
    private bool settings = false;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        panel = PauseUI.transform.GetChild(0).gameObject;
        gameOver = PauseUI.transform.GetChild(1).gameObject;
        watch = PauseUI.transform.GetChild(2).gameObject;
        mainMenue = PauseUI.transform.GetChild(3).gameObject;
        rematch = PauseUI.transform.GetChild(4).gameObject;
        panelDIA = Dialog.transform.GetChild(0).gameObject;
        dontBtn = Dialog.transform.GetChild(1).gameObject;
        saveBtn = Dialog.transform.GetChild(2).gameObject;
        inputSave = Dialog.transform.GetChild(3).gameObject;

        if(GameSettings.getGameType() != GameSettings.GameType.MULTIPLAYER)
        {
            send.interactable = false;
            input.interactable = false;
        }
    }

    public void onValueChanged()
    {
        if(inputSave.GetComponent<InputField>().text != "")
        {
            saveBtn.GetComponent<Button>().interactable = true;
        }
    }

    public void ActivateGameStop(GameStopType type)
    {
        PauseUI.SetActive(true);
        if(type == GameStopType.WIN)
        {
            pause = true;
            gameOver.GetComponent<TMPro.TextMeshProUGUI>().text = "winner!";
            watch.SetActive(true);
            rematch.SetActive(true);
        }
        else if(type == GameStopType.LOSE)
        {
            pause = true;
            gameOver.GetComponent<TMPro.TextMeshProUGUI>().text = "game over";
            watch.SetActive(true);
            rematch.SetActive(true);
        }
        else if (type == GameStopType.PAUSE)
        {
           
            if (!pause)
            {
                DeactivateGameStop();
                return;
            }
            if (gameOver.GetComponent<TMPro.TextMeshProUGUI>().text == "")
            {
                gameOver.GetComponent<TMPro.TextMeshProUGUI>().text = "Leave Game?";
                watch.SetActive(false);
                rematch.SetActive(false);
            }
            else if(gameOver.GetComponent<TMPro.TextMeshProUGUI>().text != "Leave Game?")
            {
                watch.SetActive(true);
                rematch.SetActive(true);
            }

        }
    }

    public void DeactivateGameStop()
    {
        playClickSound();
        PauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickPause()
    {
        playClickSound();
        pause = !pause;
        ActivateGameStop(GameStopType.PAUSE);
    }
    

    public void OnClickRematch()
    {
        playClickSound();
        SceneManager.LoadScene("BotsCountScene");
        

    }
    public void OnClickMainMenue()
    {
        playClickSound();
        if (GameSettings.getGameType() == GameSettings.GameType.MULTIPLAYER)
        {
            Server.Instance().leaveRoom();
            SceneManager.LoadScene("MainScene");
            return;
        
        }

        Dialog.SetActive(true);
        PauseUI.SetActive(false);


    }

    public void onClickDont()
    {
        playClickSound();
        SceneManager.LoadScene("MainScene");
    }


    public void onClickSave()
    {
        playClickSound();
        string name = inputSave.GetComponent<InputField>().text;
        print(name);
        int last = PlayerPrefs.GetInt("last", 0);
        last++;
        if (last == 6)
            last = 1;
        print(last);
        PlayerPrefs.SetInt("last", last);
        PlayerPrefs.SetString("save" + last, name);
        LevelManager.saveGame();
        SceneManager.LoadScene("MainScene");
    }

    public void playClickSound()
    {
        audioManager.PlayClickSound();
    }

    public enum GameStopType
    {
        WIN, LOSE, PAUSE
    }
}
