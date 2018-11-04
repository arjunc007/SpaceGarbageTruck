using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float detectionRadius = 0.4f;

    [SerializeField]
    GameObject bulletObject;
    [SerializeField]
    float bulletSpeed = 30f;
    [SerializeField]
    AudioClip bulletSound, engineIdle, engineRunning;

    [SerializeField]
    Transform leftGun, rightGun;

    Rigidbody2D rigidBody;
    AudioSource audioSource;
    ShipStats stats;
    public ParticleSystem particle;

    bool isDocking = false;
    bool hasGrabbed = false;
    public GameObject grabbedWreckage = null;
    public GameObject grabbedPart = null;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        stats = GetComponent<ShipStats>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = engineRunning;
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {

        //If not grabbed wreckage, enable controls
        if (!grabbedWreckage && !isDocking)
        {
            if (Input.GetButton("Thrust"))
            {
                rigidBody.AddRelativeForce(new Vector2(0, Input.GetAxis("Thrust") * stats.GetAcceleration()));
                if (Input.GetButtonDown("Thrust"))
                {
                    audioSource.clip = engineRunning;
                    particle.Play();
                }
            }
            if (Input.GetButtonUp("Thrust"))
            {
                audioSource.clip = engineIdle;
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
        AudioSource.PlayClipAtPoint(bulletSound, transform.position);
        GameObject bullet = Instantiate(bulletObject, leftGun.position, transform.rotation);
        bullet = Instantiate(bulletObject, rightGun.position, transform.rotation);

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
        isDocking = true;
        yield return new WaitForSeconds(2);
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0.0f;
        isDocking = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject GO = collision.gameObject;

        if(GO.tag == "CollectionPoint")
        {
            if(grabbedPart)
            {
                //Add part points
                StartCoroutine(Dock());
                stats.SetScore(GO.GetComponent<CollectionPoint>().GetValue(grabbedPart.tag));
                Destroy(grabbedPart);
                grabbedPart = null;
                hasGrabbed = false;
            }
        }
    }
}
