using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float detectionRadius = 0.4f;

    [SerializeField]
    GameObject bulletObject;
    [SerializeField]
    float bulletSpeed = 30f;
    [SerializeField]
    AudioClip bulletSound, engineIdle, engineRunning, pickupSound;

    [SerializeField]
    Transform leftGun, rightGun;
    [SerializeField]
    Text scoreText;

    //Input buttons
    [SerializeField]
    string ThrustButton, TurnButton, GrabButton, ShootButton;

    Rigidbody2D rigidBody;
    AudioSource audioSource;
    ShipStats stats;
    bool engineOn = false;
    public ParticleSystem particle;
    public GameObject explosion;

    bool isDocking = false;
    bool hasGrabbed = false;
    GameObject grabbedWreckage = null;
    GameObject grabbedPart = null;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        stats = GetComponent<ShipStats>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = engineIdle;
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {

        //If not grabbed wreckage, enable controls
        if (!grabbedWreckage && !isDocking)
        {
            float vel = Input.GetAxis(ThrustButton);
            rigidBody.AddRelativeForce(new Vector2(0, vel * stats.GetAcceleration()));
            if (vel != 0 && !engineOn)
            {
                //Turn Engine on
                audioSource.clip = engineRunning;
                audioSource.Play();
                particle.Play();
                engineOn = true;
            }
            else if(vel == 0 && engineOn)
            {
                //Turn engine off
                audioSource.clip = engineIdle;
                audioSource.Play();
                particle.Stop();
                engineOn = false;
            }
            
            transform.Rotate(0, 0, -Input.GetAxis(TurnButton) * stats.GetTurningSpeed());
        }
        
        if(Input.GetButtonDown(GrabButton))
        {
            GrabObject();
        }

        if(Input.GetButtonDown(ShootButton))
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
        if (grabbedWreckage)
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
        }
        else if (grabbedPart)
        {
            grabbedPart.transform.parent = null;
            grabbedPart.GetComponent<Rigidbody2D>().simulated = true;
            grabbedPart = null;
            hasGrabbed = false;
            return;
        }
        
        Collider2D wreckage = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Wreckage"));
        Collider2D part = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Part"));
        if (wreckage && wreckage.GetComponent<WreckageController>().StartMine(gameObject))
        {
            //Attach player to wreckage
            hasGrabbed = true;
            grabbedWreckage = wreckage.gameObject;
            
            StartCoroutine(Dock());
        }
        else if(part)
        {
            //Pick up part
            GrabPart(part.transform);
        }
    }

    IEnumerator Dock()
    {
        //Add drill/ other animation
        isDocking = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0.0f;
        yield return new WaitForSeconds(2);
        isDocking = false;
        yield return null;
    }

    public void GrabPart(Transform part)
    {
        if (part)
        {
            grabbedPart = part.gameObject;
            grabbedPart.GetComponent<Rigidbody2D>().simulated = false;
            grabbedPart.transform.parent = transform;
            grabbedPart.transform.localPosition = new Vector3(0, 0.1f, 0);
            grabbedPart.GetComponent<Collider2D>().isTrigger = false;
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
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
                scoreText.text = stats.GetScore().ToString();
                Destroy(grabbedPart);
                grabbedPart = null;
                hasGrabbed = false;
                //Turn engine off
                audioSource.clip = engineIdle;
                audioSource.Play();
                particle.Stop();
                engineOn = false;
            }
        }
        else if(GO.tag == "Bullet")
        {
            stats.SetShipStrength(-10);
            //Show sparks at hitpoint

            if(stats.GetShipStrength() < 0)
            {
                //Play Explosion
                StartCoroutine(Explode());
            }
            Destroy(GO);
        }
    }

    IEnumerator Explode()
    {
        GameObject exp = Instantiate(explosion, transform);
        yield return new WaitForSeconds(2);
        Destroy(exp);
        //Reset to start


    }
}
