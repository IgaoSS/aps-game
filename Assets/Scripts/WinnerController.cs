using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerController : MonoBehaviour
{
    public Text winnerText;
    
    void Start()
    {
        Debug.Log("Inicio  winner");

        string nick = PlayerPrefs.GetString("WinnerNick");
        int score = PlayerPrefs.GetInt("WinnerScore");

        string msg = "Vencedor\n" + nick + "\n\n" + "Score\n" + score;
        winnerText.text = msg;
    }

    public void ReturnMenu()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
