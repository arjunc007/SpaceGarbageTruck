using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    float speed = 15f;
    float life = 3f;
    float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - startTime > life)
            Destroy(gameObject);
        transform.position += transform.up * speed * Time.deltaTime;	
	}

    void SetSpeed(float s)
    {
        speed = s;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            //Damage player
        }

        //Show sparks and destroy
        Destroy(gameObject);
    }
}
