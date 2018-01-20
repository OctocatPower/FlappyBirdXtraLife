using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CotcSdk;

public class Connexion : MonoBehaviour
{
    private bool connected = false;
    private bool connectedWithAccount = false;
    private Gamer Player;
    private Cloud cloud;
    private Bundle MaxScore;
    private GUIController GuiControl;

    public

    // Use this for initialization
    void Start()
    {
        GuiControl = gameObject.GetComponent<GUIController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnonymeConnexion()
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
                Player = gamer;
                SetUserValue("MaxScore", new Bundle(0));
            }, ex =>
            {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        });
    }

    public void OnEmailConnexionClick()
    {
        CotcGameObject cotc = FindObjectOfType<CotcGameObject>();
        string email = GameObject.Find("Email").GetComponent<InputField>().text;
        string mdp = GameObject.Find("MotDePasse").GetComponent<InputField>().text;
        cotc.GetCloud().Done(cloud => {
            cloud.Login(
                network: "email",
                networkId: email,
                networkSecret: mdp)
            .Done(gamer => {
                Player = gamer;
                Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                Debug.Log("Login data: " + gamer);
                Debug.Log("Server time: " + gamer["servertime"]);
                connectedWithAccount = true;
                GuiControl.OnReturnClick();
            }, ex => {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                if (error.ServerData["name"] == "BadArgument")
                {
                    GameObject.Find("Error").GetComponent<Text>().text = "Erreur, l'adresse e-mail ou le mot de passe n'est pas valide.";
                }
                else if (error.ServerData["name"] == "BadUserCredentials")
                {
                    GameObject.Find("Error").GetComponent<Text>().text = "Erreur, le mot de passe n'est pas correcte.";
                }
                else
                {
                    GameObject.Find("Error").GetComponent<Text>().text = "Erreur.";
                }
            });
        });
    }

    public void DieWithScore(int score)
    {
        if (connected)
        {
            GetMaxScoreValue();
            Debug.Log("Score : " + score + " max score : " + MaxScore["MaxScore"]);
            if (MaxScore == null || MaxScore["MaxScore"] < score)
            {
                Bundle NewBestScore = new Bundle(score);
                SetUserValue("MaxScore", NewBestScore);
            }
        }
    }

    public void SetUserValue(string key, Bundle value)
    {
        if (connected)
        {
            Player.GamerVfs.Domain("private").SetValue(key, value)
            .Done(setUserValueRes =>
            {
                Debug.Log("User data set: " + setUserValueRes.ToString());
                GetMaxScoreValue();
            }, ex =>
            {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
                Debug.LogError("Could not set user data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
        }
    }

    public void GetMaxScoreValue()
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


    public Bundle getMaxScore
    {
        get
        {
            return this.MaxScore;
        }

        set
        {
            this.MaxScore = value;
        }
    }

    public bool isconnectedWithAccount
    {
        get
        {
            return this.connectedWithAccount;
        }
        set
        {
            this.connectedWithAccount = value;
        }
    }
}
