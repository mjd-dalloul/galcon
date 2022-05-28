using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountsManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject LoadingView, LoginMsg, RegisterMsg, LoginEmail, RegisterEmail
        , LoginPass, RegisterPass, RegisterConfPass, RegisterUsername;
    Server server;
    string loginUsername, loginPassword;
    void Awake()
    {
        LoadingView = transform.GetChild(11).gameObject;
        LoginMsg = transform.GetChild(12).gameObject;
        RegisterMsg = transform.GetChild(13).gameObject;
        LoginEmail = transform.GetChild(0).gameObject;
        LoginPass = transform.GetChild(1).gameObject;
        RegisterEmail = transform.GetChild(2).gameObject;
        RegisterPass = transform.GetChild(3).gameObject;
        RegisterConfPass = transform.GetChild(4).gameObject;
        RegisterConfPass.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate { onEndEdit(); });
        RegisterUsername = transform.GetChild(5).gameObject;

        server = Server.Instance();
        server.setAcccountsManager(this);
    }

    void Start() 
    {
        // resoter previous login info
        LoginEmail.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("username", "");
        LoginPass.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("password", "");
    }

    private bool checkPasswordMatch()
    {
        return RegisterPass.GetComponent<TMP_InputField>().text == RegisterConfPass.GetComponent<TMP_InputField>().text;
    }
    private void onEndEdit()
    {
        if(!checkPasswordMatch())
        {
            RegisterMsg.GetComponent<Text>().text = "*Password does not match";
        }
    }

    public void LoginOnClick()
    {
        Text msg = LoginMsg.GetComponent<Text>();
        msg.text = "";
        RegisterMsg.GetComponent<Text>().text = "";
        string username = LoginEmail.GetComponent<TMP_InputField>().text;
        string password = LoginPass.GetComponent<TMP_InputField>().text;
        
        // last entered info
        loginPassword = password;
        loginUsername = username;
        

        if(username == "")
        {
            loginCB(false, "*Plaese Enter your Email");
        }
        else if (password == "")
        {
            loginCB(false, "*Plaese Enter your Password");
        }
        else
        {
            LoadingView.SetActive(true);
            server.login(username, password);
            // server.LoginNotifyServer(email, password);
        }
    }

    public void ResgisterOnClick()
    {
        LoginMsg.GetComponent<Text>().text = "";
        RegisterMsg.GetComponent<Text>().text = "";
        string email = RegisterEmail.GetComponent<TMP_InputField>().text;
        string password = RegisterPass.GetComponent<TMP_InputField>().text;
        string confPAss = RegisterConfPass.GetComponent<TMP_InputField>().text;
        string username = RegisterUsername.GetComponent<TMP_InputField>().text;
        if (email == "")
        {
            RegisterCB(false, "*Plaese Enter your Email");
            return;
        }
        if (password == "")
        {
            RegisterCB(false, "*Plaese Enter your Password");
            return;
        }
        if (confPAss == "")
        {
            RegisterCB(false, "*Plaese Confirm Password");
            return;
        }
        if (username == "")
        {
            RegisterCB(false, "*Plaese Enter a Username");
            return;
        }
        if (!checkPasswordMatch())
        {
            RegisterCB(false, "*Password does not match");
            RegisterPass.GetComponent<TMP_InputField>().text = "";
            RegisterConfPass.GetComponent<TMP_InputField>().text = "";
            return;
        }
        LoadingView.SetActive(true);
        server.register(email, username, password);
        
        // server.RegisterNotifyServer(email, password, username);
    }

    public void loginCB(bool result, string msg)
    {
        LoadingView.SetActive(false);
        if (!result) {
            LoginMsg.GetComponent<Text>().text = msg;
        }
        else {
            PlayerPrefs.SetString("name", loginUsername);
            PlayerPrefs.SetString("username", loginUsername);
            PlayerPrefs.SetString("password", loginPassword);
            SceneManager.LoadScene("MainScene");
        }
    }

    public void RegisterCB(bool state, string msg)
    {
        LoadingView.SetActive(false);
        if (!state) {
            RegisterMsg.GetComponent<Text>().text = msg;
        }
        else {
            string username = RegisterUsername.GetComponent<TMP_InputField>().text;
            string password = RegisterPass.GetComponent<TMP_InputField>().text;
            
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("password", password);

            RegisterEmail.GetComponent<TMP_InputField>().text = "";
            RegisterPass.GetComponent<TMP_InputField>().text = ""; 
            RegisterConfPass.GetComponent<TMP_InputField>().text = "";
            RegisterUsername.GetComponent<TMP_InputField>().text = "";

            // fill login fields
            Start();

            // PlayerPrefs.SetString("name", username);
            // SceneManager.LoadScene("MainScene");
        }
    }
}
