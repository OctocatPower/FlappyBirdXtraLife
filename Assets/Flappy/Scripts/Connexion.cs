using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class Connexion : MonoBehaviour
{


    public GameObject IntroGUI, Score, Flappy, MenuGUI, Canvas, ConnectedButton, Play, ClassementGUI, ReturnButton, SuccesGUI;
    public bool connected = false;
    public GameObject[] Succes = new GameObject[3];

    private Gamer Player;
    private Cloud cloud;
    private Bundle MaxScore;

    public

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnexionClick()
    {
        CotcGameObject cotc = FindObjectOfType<CotcGameObject>();
        cotc.GetCloud().Done(cloud =>
        {
            cloud.LoginAnonymously()
            .Done(gamer =>
            {
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Signed in succeeded (Secret = " + gamer.GamerSecret + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);
                connected = true;
                GameObject.Find("Connexion").SetActive(false);
                ConnectedButton.SetActive(true);
                Player = gamer;
            }, ex =>
            {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });
    }

    public void OnPlayClick()
    {
        IntroGUI.SetActive(true);
        Score.SetActive(true);
        Flappy.SetActive(true);
        MenuGUI.SetActive(false);
        Canvas.SetActive(false);
    }

    public void OnClassementClick()
    {
        GetMaxScoreValue();
        ClassementGUI.SetActive(true);
        ReturnButton.SetActive(true);
        MenuGUI.SetActive(false);
        Play.SetActive(false);
        ConnectedButton.SetActive(false);

        ScoreManagerScript.Score = MaxScore["MaxScore"];
    }

    public void OnSuccesClick()
    {
        GetMaxScoreValue();
        SuccesGUI.SetActive(true);
        ReturnButton.SetActive(true);
        MenuGUI.SetActive(false);
        Play.SetActive(false);
        ConnectedButton.SetActive(false);

        if (MaxScore["MaxScore"] >= 10)
        {
            Succes[0].SetActive(true);
            if (MaxScore["MaxScore"] >= 50)
            {
                Succes[1].SetActive(true);
                if (MaxScore["MaxScore"] >= 100)
                {
                    Succes[2].SetActive(true);
                }
            }
        }
    }

    public void OnReturnClick()
    {
        ClassementGUI.SetActive(false);
        SuccesGUI.SetActive(false);
        ReturnButton.SetActive(false);
        MenuGUI.SetActive(true);
        Play.SetActive(true);
        ConnectedButton.SetActive(true);
        ScoreManagerScript.Score = 0;
    }

    public void DieWithScore(int score)
    {
        if (connected)
        {
            GetMaxScoreValue();
            if (MaxScore == null || MaxScore["MaxScore"] < score)
            {
                Bundle NewBestScore = new Bundle(score);
                SetUserValue("MaxScore", NewBestScore);
            }
        }
    }

    private void SetUserValue(string key, Bundle value)
    {
        if (connected)
        {
            Player.GamerVfs.Domain("private").SetValue(key, value)
            .Done(setUserValueRes =>
            {
                Debug.Log("User data set: " + setUserValueRes.ToString());
            }, ex =>
            {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
                Debug.LogError("Could not set user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
        }
    }

    private void GetMaxScoreValue()
    {
        if (connected)
        {
            Player.GamerVfs.Domain("private").GetValue("MaxScore")
            .Done(getUserValueRes =>
            {
                MaxScore = getUserValueRes["result"];
                Debug.Log("User data: " + MaxScore.ToString());

            }, ex =>
            {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.Log("Could not get user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
        }
    }
}
