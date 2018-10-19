using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    public float destructionTime;

	void Start ()
    {
        Destroy(gameObject, destructionTime);	
	}	
}
