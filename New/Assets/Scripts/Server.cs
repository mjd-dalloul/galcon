using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class Server : MonoBehaviour {
    public GameObject go;
    private SocketIOComponent socket;
    private static Server _instance;
    private Attacker ah;
    // Start is called before the first frame update
    void Start() {
        ah = FindObjectOfType<Attacker>();
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("welcome", WelcomeCB);
        socket.On("AttackCB", AttackCB);
        socket.On("mapCB", mapCB);
    }

    // Update is called once per frame
    void Update() {

    }

    public static Server Instcance() {
        return _instance;
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void WelcomeCB(SocketIOEvent e) {
        Debug.Log(e.data);
        mapNotifyServer();
    }

    public void mapNotifyServer() {
        socket.Emit("mapNotifyServer");
    }

    public void mapCB(SocketIOEvent e) {
        print(e.data);
    }

    public void AttackNotifyServer(int source, int target, bool action) {
        Dictionary<string, string> attackData = new Dictionary<string, string>();

        attackData["source"] = source.ToString();
        attackData["target"] = target.ToString();
        attackData["action"] = action.ToString();
        socket.Emit("AttackNotifyServer", new JSONObject(attackData));
    }

    public void AttackCB(SocketIOEvent e) {
        Dictionary<string, string> data = e.data.ToDictionary();
        int source = int.Parse(data["source"].ToString());
        int target = int.Parse(data["target"].ToString());
        bool action = bool.Parse(data["action"].ToString());
        ah.modifyAttacks(source, target, action);
    }

}