using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFrontMainCam : MonoBehaviour
{
    Transform cam;

	void Awake ()
    {
        cam = Camera.main.transform;
	}
	
	void Update ()
    {
        transform.rotation = cam.rotation;
	}
}
