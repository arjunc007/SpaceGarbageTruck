using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float detectionRadius = 0.4f;

    Rigidbody2D rigidBody;
    ShipStats stats;
    public ParticleSystem particle;

    bool hasGrabbed = false;
    public GameObject grabbedWreckage = null;
    public GameObject grabbedPart = null;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        stats = GetComponent<ShipStats>();
    }
	
	// Update is called once per frame
	void Update () {

        //If not grabbed wreckage, enable controls
        if (!grabbedWreckage)
        {
            if (Input.GetButton("Thrust"))
            {
                rigidBody.AddRelativeForce(new Vector2(0, Input.GetAxis("Thrust") * stats.GetAcceleration()));
                if (Input.GetButtonDown("Thrust"))
                {
                    GetComponent<AudioSource>().Play();
                    particle.Play();
                }
            }
            if (Input.GetButtonUp("Thrust"))
            {
                GetComponent<AudioSource>().Stop();
                particle.Stop();
            }

            if (Input.GetButton("Turn"))
            {
                transform.Rotate(0, 0, -Input.GetAxis("Turn") * stats.GetTurningSpeed());
            }
        }
        
        if(Input.GetButtonDown("Grab"))
        {
            GrabObject();
        }

        if(Input.GetButtonDown("Shoot"))
        {
            Shoot();
        }

        Vector2 dir = rigidBody.velocity.normalized;
        float mag = rigidBody.velocity.magnitude;
        rigidBody.velocity = dir * Mathf.Min(mag, stats.GetMaxSpeed());
	}

    void Shoot()
    {
        //If holding object, don't shoot
        if (hasGrabbed)
        {
            return;
        }

        //Normal shooting

    }

    void GrabObject()
    {
        //If already has grabbed something, release it
        if(grabbedWreckage)
        {
            Transform t = grabbedWreckage.GetComponent<WreckageController>().StopMine();
            //If grabbed something, call grabpart, otherwise set hasgrabbed to false 
            if (t != null)
            {
                GrabPart(t);
            }
            else
            {
                hasGrabbed = false;
            }

            grabbedWreckage = null;
            return;
        } else if (grabbedPart)
        {
            grabbedPart.transform.parent = null;
            grabbedPart = null;
            hasGrabbed = false;
        }

        Collider2D wreckage = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Wreckage"));
        if(wreckage && !hasGrabbed)
        {
            //Attach player to wreckage
            hasGrabbed = true;
            grabbedWreckage = wreckage.gameObject;
            grabbedWreckage.GetComponent<WreckageController>().StartMine();
            StartCoroutine(Dock());
        }
    }

    IEnumerator Dock()
    {
        //Add drill/ other animation
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0.0f;
        yield return null;
    }

    public void GrabPart(Transform part)
    {
        if (part)
        {
            grabbedPart = part.gameObject;
            grabbedPart.transform.parent = transform;
            grabbedPart.transform.localPosition = new Vector3(0, 0.1f, 0);
            grabbedPart.GetComponent<Collider2D>().isTrigger = false;
        }

        grabbedWreckage = null;
    }
}
