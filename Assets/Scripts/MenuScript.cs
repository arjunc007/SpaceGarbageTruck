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
        if (Input.GetButtonDown("Shoot"))
        {
            SceneManager.LoadScene(1);
        }

        else if (Input.GetButtonDown("Grab"))
        {
            Application.Quit();
        }
	}
}