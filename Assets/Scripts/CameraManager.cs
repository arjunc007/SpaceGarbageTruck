using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField]
    Transform Player1, Player2;
    [SerializeField]
    Vector2 threshold;

    bool sharedActive = true;

    Transform P1Cam, P2Cam, SharedCam;


	// Use this for initialization
	void Start () {
        SharedCam = transform.GetChild(0);
        P1Cam = transform.GetChild(1);
        P2Cam = transform.GetChild(2);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 dist = Player1.position - Player2.position;

        if (Mathf.Abs(dist.x) < threshold.x && Mathf.Abs(dist.y) < threshold.y)
        {
            if(!sharedActive)
            {
                sharedActive = true;
                P1Cam.gameObject.SetActive(false);
                P2Cam.gameObject.SetActive(false);
                SharedCam.gameObject.SetActive(true);
            }
        }
        else
        {
            if(sharedActive)
            {
                sharedActive = false;
                SharedCam.gameObject.SetActive(false);
                P1Cam.gameObject.SetActive(true);
                P2Cam.gameObject.SetActive(true);
            }
        }
	}
}
