using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Shoot_KB") || Input.GetButtonDown("Shoot_JS"))
        {
            SceneManager.LoadScene(1);
        }

        else if (Input.GetButtonDown("Grab_KB") || Input.GetButtonDown("Grab_KB"))
        {
            Application.Quit();
        }

        else if (Input.GetButtonDown("Thrust_KB") || Input.GetButtonDown("Thrust_KB"))
        {
            SceneManager.LoadScene(2);
        }
	}
}