using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class Connexion : MonoBehaviour {


    public GameObject IntroGUI, Score, Flappy, MenuGUI, Canvas, ClassementButton;
    private Cloud cloud;
    public bool connected = false;
    private Gamer Player;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnConnexionClick()
    {
        CotcGameObject cotc = FindObjectOfType<CotcGameObject>();
        cotc.GetCloud().Done(cloud => {
            cloud.LoginAnonymously()
            .Done(gamer => {
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Signed in succeeded (Secret = " + gamer.GamerSecret + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);
                connected = true;
                ChangeMenu();
                Player = gamer;
            }, ex => {
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

    }

    private void ChangeMenu()
    {
        GameObject.Find("Connexion").SetActive(false);
        ClassementButton.SetActive(true);
    }

    public void DieWithScore (int score)
    {
        if (connected)
        {
            Bundle BestScore = GetUserValue("MaxScore");
            if ((BestScore != null && BestScore.AsInt() < score) || BestScore == null)
            {
                Debug.Log("TEEEEEEST");
                Bundle NewBestScore = new Bundle(score);
                SetUserValue("MaxScore", NewBestScore);
            }
        }

        Bundle value = new Bundle(score);
    }

    private void SetUserValue(string key, Bundle value)
    {
        if (connected)
        {
            // currentGamer is an object retrieved after one of the different Login functions.

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

    private Bundle GetUserValue(string key)
    {
        Bundle result = null;

        if (connected)
        {
            
            // currentGamer is an object retrieved after one of the different Login functions.

            Player.GamerVfs.Domain("private").GetValue(key)
            .Done(getUserValueRes =>
            {
                result = getUserValueRes["result"];
                Debug.Log("User data: " + result.ToString());
            }, ex =>
            {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
                Debug.Log("Could not get user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
        }
        return result;
    }
}
