using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyLevels : MonoBehaviour
{
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    public void setDifficulty(int level)
    {
        Debug.Log("setDifficulty : " + level);
        playClickSound();
        for(int i=1 ; i<GameSettings.getPlayersCount() ; i++)
            GameSettings.setPlayerType(i, (GameSettings.PlayerType)(level+2));
        GameSettings.setSeed(Random.Range(1, 1000));
        SceneManager.LoadScene("RoomScene");
    }

    public void OnBack()
    {
        playClickSound();
        SceneManager.LoadScene("BotsCountScene");
    }

    public void playClickSound()
    {
        audioManager.PlayClickSound();
    }
}
