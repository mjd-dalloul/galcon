using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainSceneController : MonoBehaviour
{
    public TextMeshProUGUI Username, XP, Rank, Email;
    public GameObject Leaderboard, contents, profileUI;
    private AudioManager audioManager;
    private Server server;
    // Start is called before the first frame update
    void Start()
    { 
        audioManager = FindObjectOfType<AudioManager>();
        server = Server.Instance();
        server.setMainSceneController(this);
        
        Username.text = PlayerPrefs.GetString("username", "N/A");
        server.getXp();
        server.getEmail();
        server.getRanking();

        bool ok = false;
        for (int i = 1; i < 6; i++)
        {
            ok |= SaveLoadManager.hasGame(i);
        }
        if (!ok)
        {
            transform.GetChild(5).gameObject.GetComponent<Button>().interactable = false;
        }
    }
    
    public void clearLeaderboard() {
        GameObject bannerRef = contents.transform.GetChild(0).gameObject;
        GameObject banner = Instantiate(bannerRef);
        for (int i = 0; i < contents.transform.childCount; i++)
        {
            Destroy(contents.transform.GetChild(i).gameObject);
        }
        banner.transform.SetParent(contents.transform, false);
    }
    
    public void AddToLeaderboard(int rank, string username, int xp)
    {
        GameObject l = Instantiate(Leaderboard, contents.transform, false);
        l.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rank.ToString();
        l.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = username;
        l.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = xp.ToString();
    }

    public void LogOutOnClick()
    {
        playClickSound();
        server.logout();
        SceneManager.LoadScene("AccountScene");
    }

    public void profileOnClick()
    {
        playClickSound();
        profileUI.SetActive(!profileUI.active);
    }

    public void practiceOnClick()
    {
        playClickSound();
        GameSettings.setGameType(GameSettings.GameType.PRACTICE);
        GameSettings.setRoomName("PractiseRoom");
        SceneManager.LoadScene("BotsCountScene");
    }

    public void startOnClick()
    {
        playClickSound();
        GameSettings.setGameType(GameSettings.GameType.MULTIPLAYER);
        SceneManager.LoadScene("OpenRoomsScene");
    }

    public void settingsOnClick()
    {
        playClickSound();
        SceneManager.LoadScene("SettingsScene");
    }

    public void savedOnClick()
    {
        playClickSound();
        SceneManager.LoadScene("SavedScene");
    }

    public void quitOnClick()
    {
        //playClickSound();
        Application.Quit();
    }

    public void playClickSound()
    {
        audioManager.PlayClickSound();
    }

    public string getRankFromXp(int xp){
        if(xp < 100) {
            return "Newbie";
        } else if(xp < 1000) {
            return "Intermidate";
        } else if(xp < 10000) {
            return "Expert";
        } else {
            return "Legandery";
        }
    } 
    public void xpCB(int xp) {
        XP.text = xp.ToString();
        Rank.text = getRankFromXp(xp);
    }
    public void emailCB(string email) {
        Email.text = email;
    }
    public void rankingCB(Server.RankWrapper.Entry[] ranking) {
        clearLeaderboard();
        for(int i=0 ; i < ranking.Length ; i++) {
            AddToLeaderboard(i+1, ranking[i].username, ranking[i].xp);
        }
    }
}
