using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Resources.Load("Prefabs/Server", typeof(GameObject)) as GameObject);//.GetComponent<Server>();
        // if (PlayerPrefs.GetString("name", "") == "")
            Invoke("gotoLogin", 2.3f);
        // else
        //     Invoke("gotoMain", 2.3f);
    }
    
    public void gotoMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void gotoLogin()
    {
        SceneManager.LoadScene("AccountScene");
    }


}
