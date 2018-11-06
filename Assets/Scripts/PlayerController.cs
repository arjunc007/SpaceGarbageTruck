using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float detectionRadius = 0.4f;

    [SerializeField]
    GameObject bulletObject;
    [SerializeField]
    float bulletSpeed = 15f;
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
    public GameObject explosion, sparks;
    public AudioClip explosionSound, hitSound;


    Vector3 startPosition;
    bool isAnimating = false;
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
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        //
        if(Countdown.endGame)
        {
            if(Input.anyKeyDown)
            {
                Countdown.endGame = false;
                SceneManager.LoadScene(0);
            }
            return;
        }

        //If not grabbed wreckage, enable controls
        if (!grabbedWreckage && !isAnimating)
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
        bullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * transform.up;
        Destroy(bullet, 2.0f);

        bullet = Instantiate(bulletObject, rightGun.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed*transform.up;
        Destroy(bullet, 2.0f);

    }

    void GrabObject()
    {
        //If already has grabbed something, release it
        if (hasGrabbed)
        {
            ReleaseGrabbed();
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

    void ReleaseGrabbed()
    {
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
    }

    IEnumerator Dock()
    {
        //Add drill/ other animation
        isAnimating = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0.0f;
        yield return new WaitForSeconds(2);
        isAnimating = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
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
            ContactPoint2D[] contacts = new ContactPoint2D[5];
            int numContacts = collision.GetContacts(contacts);
            if (numContacts > 0)
            {
                GameObject p = Instantiate(sparks, contacts[0].point, Quaternion.LookRotation(-contacts[0].normal, Vector2.up));
                Destroy(p, 0.2f);
                audioSource.PlayOneShot(hitSound);
            }

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
        isAnimating = true;
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 2f);
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        //Disable renderer, collider
        Collider2D col = GetComponent<PolygonCollider2D>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        col.enabled = false;
        renderer.enabled = false;

        //Reset to start
        yield return new WaitForSeconds(5); //Time to wait before respawn
        isAnimating = false;
        stats.SetShipStrength(100);
        transform.SetPositionAndRotation(startPosition, Quaternion.identity);
        ReleaseGrabbed();


        col.enabled = true;
        renderer.enabled = true;
    }
}
