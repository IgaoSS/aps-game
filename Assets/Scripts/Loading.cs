using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Text loading;
    string msg = "";

    float timer = 0f;
    float interval = 0.5f;

    private void Start()
    {
        loading.text = "";
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            msg = msg + ". ";

            if (msg.Trim().Length >= 10)
            {
                msg = ". ";
            }

            loading.text = msg;
            
            timer = 0f;
        }
    }
}
