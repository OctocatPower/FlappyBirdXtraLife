using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour {

    public GameObject IntroGUI, Score, Flappy, MenuGUI, Canvas, ConnectedButton, MenuButton, ClassementGUI, ReturnButton, SuccesGUI, EmailConnexion, Compte;
    public GameObject[] Succes = new GameObject[3];

    private Connexion connexion;

    // Use this for initialization
    void Start () {
        connexion = gameObject.GetComponent<Connexion>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnConnexionClick()
    {
        connexion.AnonymeConnexion();
        ConnectedButton.SetActive(true);
        GameObject.Find("Connexion").SetActive(false);
    }

    public void OnConnexionNonAnonymeClick()
    {
        EmailConnexion.SetActive(true);
        ReturnButton.SetActive(true);
        MenuButton.SetActive(false);
        ConnectedButton.SetActive(false);
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
        connexion.GetMaxScoreValue();
        ClassementGUI.SetActive(true);
        ReturnButton.SetActive(true);
        MenuGUI.SetActive(false);
        MenuButton.SetActive(false);
        ConnectedButton.SetActive(false);

        ScoreManagerScript.Score = connexion.getMaxScore["MaxScore"];
    }

    public void OnSuccesClick()
    {
        connexion.GetMaxScoreValue();
        SuccesGUI.SetActive(true);
        ReturnButton.SetActive(true);
        MenuGUI.SetActive(false);
        MenuButton.SetActive(false);
        ConnectedButton.SetActive(false);

        if (connexion.getMaxScore["MaxScore"] >= 10)
        {
            Succes[0].SetActive(true);
            if (connexion.getMaxScore["MaxScore"] >= 50)
            {
                Succes[1].SetActive(true);
                if (connexion.getMaxScore["MaxScore"] >= 100)
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
        EmailConnexion.SetActive(false);
        MenuGUI.SetActive(true);
        MenuButton.SetActive(true);
        ConnectedButton.SetActive(true);

        if (connexion.isconnectedWithAccount)
            Compte.SetActive(false);

        ScoreManagerScript.Score = 0;
        connexion.GetMaxScoreValue();
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

}
