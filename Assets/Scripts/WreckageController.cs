using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WreckageController : MonoBehaviour {
   
    [SerializeField]
    float rotationSpeed = 1.0f;
    Image loader = null;
    public float mineTime = 8.0f;
    AudioSource audioSource;

    GameObject miner = null;

    float mineTimer = 0.0f;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        loader = GetComponentInChildren<Image>();
        loader.gameObject.SetActive(false);

        foreach(Transform child in transform)
        {
            Collider2D collider = child.GetComponent<Collider2D>();
            if (collider)
                collider.isTrigger = true;
        }

        rotationSpeed = Random.Range(1, 4);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        if(loader.gameObject.activeSelf)
        {
            mineTimer += Time.deltaTime;
            loader.fillAmount = mineTimer / mineTime;

            if(loader.fillAmount >= 1.0f)
            {
                miner.GetComponent<PlayerController>().GrabPart(StopMine());
            } else if (loader.fillAmount > 0.9f)
            {
                loader.color = Color.green;
            }
            else if (loader.fillAmount > 0.66f)
            {
                loader.color = Color.yellow;
            }
            else if (loader.fillAmount > 0.33f)
            {
                loader.color = Color.magenta;
            }
            else
                loader.color = Color.red;
        }
	}

    public bool StartMine(GameObject player)
    {
        if (miner == null)
        {
            miner = player;
            loader.gameObject.SetActive(true);
            audioSource.Play();
            mineTimer = 0;
            return true;
        }
        else
            return false;
    }

    public Transform StopMine()
    {
        loader.gameObject.SetActive(false);
        audioSource.Stop();

        loader.color = Color.red;
        miner = null;

        //Get number of children
        int numberOfParts = transform.childCount - 1;

        int childToReturn = numberOfParts;

        while(childToReturn > 0)
        {
            float cmp = childToReturn / (float)numberOfParts;

            if (loader.fillAmount >= cmp)
            {
                //If it was the last part, destroy object
                if (numberOfParts == 1)
                {
                    Destroy(gameObject);
                }
                //loader.fillAmount = 0;
                return transform.GetChild(childToReturn);
            }
            childToReturn--;
        }

        return null;
    }
}
