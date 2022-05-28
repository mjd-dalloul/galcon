using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotsCount : MonoBehaviour
{
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void setPlayersCount(int playersCount)
    {
        playClickSound();
        GameSettings.setPlayersCount(playersCount);
        GameSettings.setMainPlayerId(0);
        GameSettings.setPlayerType(0, GameSettings.PlayerType.MAIN_PLAYER);
        SceneManager.LoadScene("DifficultyScene");
    }

    public void startOldGame()
    {
        playClickSound();
        SaveLoadManager.loadData(0);
        SceneManager.LoadScene("GameScene");
    }

    public void onBack()
    {
        playClickSound();
        SceneManager.LoadScene("MainScene");
    }

    public void playClickSound()
    {
        audioManager.PlayClickSound();
    }
}
