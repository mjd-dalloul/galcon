using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        FindObjectOfType<ChatManager>().addPendingMessages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
