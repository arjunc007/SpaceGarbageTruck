using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public float roundTime = 2;
    float timeRemaining;
    public Text timeText;

    public static bool endGame;

	// Use this for initialization
	void Start ()
    {
        endGame = false;
        timeRemaining = roundTime * 60;
        //text = GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0)
        {
            EndGame();
            return;
        }

        float minutes = Mathf.Floor(timeRemaining / 60);
        float seconds = timeRemaining % 60;
        //int milliseconds = Mathf.FloorToInt((seconds - Mathf.Floor(seconds)) * 100f);
        timeText.text = "Time " + Mathf.Floor(minutes).ToString("00") + ":" + Mathf.Floor(seconds).ToString("00");// + ":" + milliseconds.ToString("00");
        
	}

    void EndGame()
    {
        if(!endGame)
        {
        endGame = true;
        //Bring score to center
        Animation[] scoreAnims = transform.root.Find("Score").GetComponentsInChildren<Animation>();
        foreach (var anim in scoreAnims)
            anim.Play();

        timeText.text = "Press any key to continue";
        //Set game status to end
        //Wait till Keypress
        }
    }
}
