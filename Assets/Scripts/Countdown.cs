using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    float timeRemaining = 120.0f;
    public Text timeText;
	// Use this for initialization
	void Start ()
    {
        //text = GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeRemaining -= Time.deltaTime;

        float minutes = Mathf.Floor(timeRemaining / 60);
        float seconds = timeRemaining % 60;
        timeText.text = "Time " + minutes + ":" +  Mathf.RoundToInt(seconds);
        if (timeRemaining <= 0)
            SceneManager.LoadScene(0);
        
	}
}
