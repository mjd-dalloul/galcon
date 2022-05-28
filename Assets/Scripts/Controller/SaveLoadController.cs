using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveLoadController : MonoBehaviour
{
    private static GameObject Saves;
   
    // Start is called before the first frame update
    void Start()
    {
        Saves = FindObjectOfType<SaveLoadController>().gameObject;
        saveExists();
    }


    public void OnClick(int index)
    {
        SaveLoadManager.loadData(index);
        SceneManager.LoadScene("gameScene");
    }

    public void BackOnClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void clearOnClick()
    {
        for (int i = 1; i < 6; i++)
        {
            SaveLoadManager.delete(i);
        }
        PlayerPrefs.SetInt("last", 0);
        SceneManager.LoadScene("MainScene");
    }

    public static void saveExists()
    {
        for (int i = 1; i < 6; i++)
        {
            if (SaveLoadManager.hasGame(i))
            {
                Saves.transform.GetChild(i - 1).gameObject.SetActive(true);
                Saves.transform.GetChild(i - 1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("save" + i, "save1");
            }
            else Saves.transform.GetChild(i - 1).gameObject.SetActive(false);
        }
    }
}
