using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public int GetValue(string tag)
    {
        if (tag == "Small Debris")
            return 20;
        else if (tag == "Medium Debris")
            return 50;
        else if (tag == "Large Debris")
            return 100;
        else
            return 0;
    }
}
