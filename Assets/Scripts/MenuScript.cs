using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    [SerializeField]
    string MP_scnName = "Multiplayer", SP_scnName = "SinglePlayer";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Shoot_KB") || Input.GetButtonDown("Shoot_JS") || Input.GetButtonDown("Shoot_KB2"))
        {
            SceneManager.LoadScene(SP_scnName);
        }

        else if (Input.GetButtonDown("Grab_KB") || Input.GetButtonDown("Grab_JS") || Input.GetButtonDown("Grab_KB2"))
        {
            SceneManager.LoadScene(MP_scnName);

            //Application.Quit();
        }

        else if (Input.GetButtonDown("Thrust_KB") || Input.GetButtonDown("Thrust_JS") || Input.GetButtonDown("Thrust_KB2"))
        {
            SceneManager.LoadScene(MP_scnName);
        }
	}
}