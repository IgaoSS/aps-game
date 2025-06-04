using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance { get; private set; }
    private int playersGaming = 0;
    private Dictionary<int, int> playerScores = new Dictionary<int, int>();
    
    private Dictionary<int, string> playerScoreTexts = new Dictionary<int, string>();
    public Text PlayerScore1, PlayerScore2, Timer, CurrentPhase;
    private int playerNumber;
    
    private float totalTime = 120f; // Tempo total em segundos
    private float currentTime; // Tempo atual em segundos
    private bool isGameRunning = true; // Flag para controlar se o jogo está em andamento
    private bool changeLevel = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    [PunRPC]
    private void Start()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        photonView.RPC("AddPlayer", RpcTarget.AllBuffered);

        //int idx = PhotonNetwork.PlayerList.Length;
        //string name = PhotonNetwork.PlayerList[idx - 1].NickName;

        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        Player localPlayer = PhotonNetwork.CurrentRoom.GetPlayer(id);
        string name = localPlayer.NickName;
        UpdateTextScore(id, 0, name);

        PlayerPrefs.SetInt("level", 1);
        UpdateLevel(1);
        currentTime = totalTime; // Inicializar o tempo atual com o tempo total
    }

    [PunRPC]
    private void Update()
    {
        if (isGameRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                isGameRunning = false;
                Debug.Log("Acabou o tempo");
                EndGame();
            }

            // Formatar o tempo para exibir apenas os segundos
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            string timeFormatted = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            Timer.text = timeFormatted;

            if ((timeSpan.Seconds == 30 || timeSpan.Seconds == 0) && changeLevel == true)
            {
                Debug.Log("trocar");
                int lvl = PlayerPrefs.GetInt("level") + 1;
                UpdateLevel(lvl);
                changeLevel = false;
            }
            else if (timeSpan.Seconds != 30 && timeSpan.Seconds != 0)
            {
                changeLevel = true;
            }


            if (PhotonNetwork.PlayerList.Length < playersGaming)
            {
                isGameRunning = false;
                EndGame();
            }
        }
    }

    [PunRPC]
    private void AddPlayer()
    {
        playersGaming++;
        Debug.Log("qto jogando pg " + playersGaming);
        Debug.Log("qto jogando pn " + PhotonNetwork.PlayerList.Length);
        Debug.Log("nome " + PhotonNetwork.PlayerList[0].NickName);

        if (playersGaming == PhotonNetwork.PlayerList.Length)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        //playerNumber = PhotonNetwork.PlayerList.Length;
        //string scoreTextName = "PlayerScore" + playerNumber;


        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        string scoreTextName = "PlayerScore" + playerNumber;

        float newPosition = 0f;

        //if (playerNumber == 1)
        //{

        if (id == 1)
        {

            PlayerScore1.gameObject.SetActive(true);
            PlayerScore2.gameObject.SetActive(false);
            newPosition = -1f;
            PhotonNetwork.Instantiate("Player", new Vector3(newPosition, 0, 0), Quaternion.identity);
        }
        else
        {
            PlayerScore1.gameObject.SetActive(false);
            PlayerScore2.gameObject.SetActive(true);
            newPosition = 1f;
            PhotonNetwork.Instantiate("Player2", new Vector3(newPosition, 0, 0), Quaternion.identity);
        }


        //playerScoreTexts.Add(playerNumber, scoreTextName);
        playerScoreTexts.Add(id, scoreTextName);
    }

    [PunRPC]
    public void AddScore(int playerId, int value)
    {
        if (!playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] = 0;
        }

        playerScores[playerId] += value;

        int id = int.Parse(playerId.ToString().Substring(0, 1));
        string name = PhotonNetwork.PlayerList[id - 1].NickName;

        //int id = PhotonNetwork.LocalPlayer.ActorNumber;
        //Player localPlayer = PhotonNetwork.CurrentRoom.GetPlayer(id);
        //string name = localPlayer.NickName;
        UpdateTextScore(id, playerScores[playerId], name);
    }

    [PunRPC]
    public void SubScore(int playerId, int value)
    {
        if (!playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] = 0;
        }

        playerScores[playerId] -= value;

        int id = int.Parse(playerId.ToString().Substring(0, 1));
        string name = PhotonNetwork.PlayerList[id - 1].NickName;
        
        //int id = PhotonNetwork.LocalPlayer.ActorNumber;
        //Player localPlayer = PhotonNetwork.CurrentRoom.GetPlayer(id);
        //string name = localPlayer.NickName;

        UpdateTextScore(id, playerScores[playerId], name);
    }

    [PunRPC]
    public void UpdateTextScore(int id, int score, string name)
    {
        Debug.Log("id received: " + id);
        Debug.Log("score received: " + score);

        if (id == 1)
        {
            PlayerScore1.text = name + ": " + score + " pontos";
        }
        else
        {
            PlayerScore2.text = name + ": " + score + " pontos";
        }

    }

    [PunRPC]
    public void UpdateLevel(int x)
    {
        PlayerPrefs.SetInt("level", x);
        CurrentPhase.text = "Nível " + PlayerPrefs.GetInt("level").ToString();
    }

    [PunRPC]
    public void SaveAllScores()
    {
        int highScore = -1000;
        int id = 0;
        string nickname = "";

        foreach (var playerScore in playerScores)
        {
            Debug.Log("Score do jogador " + playerScore.Key + ": " + playerScore.Value);
            
            if (playerScore.Value > highScore)
            {
                highScore = playerScore.Value;
                id = int.Parse(playerScore.Key.ToString().Substring(0, 1));
            }
        }

        nickname = PhotonNetwork.PlayerList[0].NickName;
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            nickname = PhotonNetwork.PlayerList[id-1].NickName;
        }

        PlayerPrefs.SetString("WinnerNick", nickname);
        PlayerPrefs.SetInt("WinnerScore", highScore);

        Debug.Log(PlayerPrefs.GetString("WinnerNick"));
        Debug.Log(PlayerPrefs.GetInt("WinnerScore"));
    }

    [PunRPC]
    public void EndGame()
    {
        //Time.timeScale = 0;
        SaveAllScores();
        //PhotonNetwork.DestroyAll();
        SceneManager.LoadScene("Winner");
    }


    /*public static GameController instance;

    public Text timer;
    private float totalTime = 8f; // Tempo total em segundos
    private float currentTime; // Tempo atual em segundos
    private bool isGameRunning = true; // Flag para controlar se o jogo está em andamento

    private string x = "";
    private string champ = "Empate";
    private int highScore = -100, idWinner = -1;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }
    private void Start()
    {
        currentTime = totalTime; // Inicializar o tempo atual com o tempo total
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;

        if(isGameRunning)
        {
            if (currentTime <= 0f)
            {
                isGameRunning = false;
                Debug.Log("Acabou o tempo");
                ShowAllScores();
            }
            // Formatar o tempo para exibir apenas os segundos
            //string seconds = Mathf.FloorToInt(currentTime % 60f).ToString("00");
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            string timeFormatted = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            timer.text = timeFormatted;
        }

    }

    public void ShowGameOver()
    {
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);
    }

    

    public void ShowAllScores()
    {
        foreach (var playerScore in playerScores)
        {
            //Debug.Log("Score do jogador " + playerScore.Key + ": " + playerScore.Value);
            x = x + "Jogador " + playerScore.Key + ": " + playerScore.Value + "\n";
            Debug.Log(playerScore.Value > highScore);
            if(playerScore.Value > highScore)
            {
                highScore = playerScore.Value;
                idWinner = playerScore.Key;
                champ = "P";
                Debug.Log(champ);
            }
        }
        //x = x + "\n" + champ;
        //Debug.Log(x);
        
        //ShowGameOver();
    }*/

}
