using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMainScene : MonoBehaviour
{
    public Transform center;
    float radius;
    public float theta;
    public float speed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        float x = Mathf.Abs(transform.position.x - center.position.x);
        float y = Mathf.Abs(transform.position.y - center.position.y);
        radius = Mathf.Sqrt((x * x) + (y * y));
        theta = (theta * Mathf.PI * 2) / (360);
    }

    // Update is called once per frame
    void Update()
    {
        theta += Time.deltaTime*speed;
        if(theta >= 360)
        {
            theta = 0;
        }
        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);
        float x = sin * radius;
        float y = cos * radius;
        transform.position = new Vector3(x + center.position.x
            , y + center.position.y, transform.position.z);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x
            , transform.localEulerAngles.y, -(theta * 360) / (Mathf.PI * 2) - 90);
    }
}
