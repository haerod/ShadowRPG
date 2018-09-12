﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    public bool canMove = true;

    [Header ("Free Mode")]

    [SerializeField] float maxY;
    [SerializeField] float minY;

    [SerializeField] float camYModifier;
    [SerializeField] float camYspeed;

    [SerializeField] float maxX;
    [SerializeField] float minX;
    [SerializeField] float maxZ;
    [SerializeField] float minZ;

    [SerializeField] float camXZModifier;
    [SerializeField] float camXZspeed;

    private Vector3 newCamPoz;
    private float newX;
    private float newY;
    private float newZ;

    [Header("Camera sur les bords")]

    [SerializeField] float xScreenBorder;
    [SerializeField] float yScreenBorder;

    [Header("Free rotation mode")]

    private float sensivity = 5;
    private Vector3 mousePozOrigin;
    private Quaternion rotQuat;

    [Header("Zoom Mode")]

    public List<Transform> targetList;
    [SerializeField] float zoomZOffset;
    [SerializeField][Range(0f,0.2f)] float camZoomSpeed;
    [SerializeField] float zoomDistMin;
    [SerializeField] float zoomDistMax;

    //[SerializeField]
    //private Transform leftPoint;
    //[SerializeField]
    //private Transform rightPoint;

    //private float maxDistance;
    private Vector3 newPozZoom;
    private float farDistance;
    private float pctDistance;
    private RaycastHit hit;
    private MouseOverHidableObject previousObj;
    [HideInInspector] public Vector3 lastPoz;
    [HideInInspector] public Quaternion lastRot;
    [HideInInspector] public bool isCinematicMode;

    bool isCenter;

    void Awake ()
    {
        newCamPoz = transform.position;
        newX = transform.position.x;
        newY = transform.position.y;
        newZ = transform.position.z;
	}
	
	void Update ()
    {
        if(!isCinematicMode)
            HideForwardBuildings();

        if (canMove)
        {
            if (Input.GetKey(KeyCode.Space))
                isCenter = true;
            else
                isCenter = false;

            if (isCenter)
                CenterOnCharacter();
            else
                FreeMode();
        }
    }



    // Mouvements de base (directions et zoom)
    void FreeMode()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // Zoom
        {
            newY = Mathf.Clamp(transform.position.y + (camYModifier * -1), minY, maxY);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // Dézoom
        {
            newY = Mathf.Clamp(transform.position.y + (camYModifier), minY, maxY);
        }

        float xMouse = Input.mousePosition.x;
        float yMouse = Input.mousePosition.y;

        if (Input.GetKey(KeyCode.RightArrow) || xMouse > Screen.width - xScreenBorder) // DROITE
        {
            newX = Mathf.Clamp(transform.position.x + (camXZModifier), minX, maxX);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || xMouse < xScreenBorder) // GAUCHE
        {
            newX = Mathf.Clamp(transform.position.x + (camXZModifier * -1), minX, maxX);
        }

        if (Input.GetKey(KeyCode.UpArrow) || yMouse > Screen.height - yScreenBorder) // HAUT
        {
            newZ = Mathf.Clamp(transform.position.z + (camXZModifier), minZ, maxZ);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || yMouse < yScreenBorder) // BAS
        {
            newZ = Mathf.Clamp(transform.position.z + (camXZModifier * -1), minZ, maxZ);
        }

        newCamPoz = new Vector3(newX, newY, newZ);
        if (newCamPoz != transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, newCamPoz, camYspeed);
        }
    }

    // Rotation de la caméra sur elle-même
    void FreeRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mousePozOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            if(Input.mousePosition.x > mousePozOrigin.x)
            {
                rotQuat = Quaternion.Euler(
                    transform.rotation.eulerAngles.x ,
                    transform.rotation.eulerAngles.y + sensivity,
                    transform.rotation.eulerAngles.z);
            }
            else if (Input.mousePosition.x < mousePozOrigin.x)
            {
                rotQuat = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y - sensivity,
                    transform.rotation.eulerAngles.z);
            }

            if (Input.mousePosition.y < mousePozOrigin.y)
            {
                rotQuat = Quaternion.Euler(
                    transform.rotation.eulerAngles.x + sensivity,
                    rotQuat.eulerAngles.y,
                    rotQuat.eulerAngles.z);
            }
            else if (Input.mousePosition.y > mousePozOrigin.y)
            {
                rotQuat = Quaternion.Euler(
                    transform.rotation.eulerAngles.x - sensivity,
                    rotQuat.eulerAngles.y,
                    rotQuat.eulerAngles.z);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, rotQuat, 0.1f);
        }
    }



    // Cache les bâtiments en face de la caméra
    void HideForwardBuildings()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                MouseOverHidableObject currentObj = hit.transform.GetComponent<MouseOverHidableObject>();

                if (previousObj == currentObj || previousObj == null)
                {
                    currentObj.isOver = true;
                    previousObj = currentObj;
                }
                else if (previousObj != currentObj)
                {
                    previousObj.isOver = false;
                    previousObj = null;
                }
            }
            else if (previousObj != null)
            {
                previousObj.isOver = false;
            }

            previousObj = hit.transform.GetComponent<MouseOverHidableObject>();
        }
    }

    
    // Centre la caméra sur le personnage
    void CenterOnCharacter()
    {
        float zoomMax = 3;
        float zoomMin = 12;
        float pctY = Dealer.instance.PercentBetweenTwoFloats(maxY, minY, transform.position.y); 
            // Calcule à quel pourcentage de zoom on en est
        float offsetZ = zoomMax + (zoomMin - zoomMax) * pctY; 
            // Calucule l'offset en z en fonction du pourcentage de zoom

        Transform chara = TurnBar.instance.GetCurrentCharacter().transform;
        Vector3 poz = new Vector3(
            chara.position.x,
            transform.position.y,
            chara.position.z - offsetZ);

        transform.position = Vector3.Lerp(transform.position, poz, 0.1f);
    }

    // Passe la caméra en cinematic mode
    public void StartCinematicMode(Transform newTransform)
    {
        canMove = false;
        lastPoz = transform.position;
        lastRot = transform.rotation;
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        isCinematicMode = true;
        Dealer.instance.tooltipUI.gameObject.SetActive(false);
    }

    // Ramène la caméra en mode normal
    public void StopCinematicMode()
    {
        transform.position = lastPoz;
        transform.rotation = lastRot;
        canMove = true;
        isCinematicMode = false;
    }

    // Mode zoom (commenté)
    /*
    // Zoom sur plusieurs targets
    void ZoomMode()
    {
        if (targetList.Count == 1)
        {
            newPozZoom = new Vector3(targetList[0].position.x, ZoomDistance(), targetList[0].position.z);
            transform.position = Vector3.Lerp(transform.position, newPozZoom, camZoomSpeed);
            return;
            /////////////
        }

        var bounds = new Bounds(targetList[0].position, Vector3.zero);

        for (int i = 0; i < targetList.Count; i++)
        {
            bounds.Encapsulate(targetList[i].position);
        }

        newPozZoom = new Vector3(bounds.center.x, ZoomDistance(), bounds.center.z + zoomZOffset);
        transform.position = Vector3.Lerp(transform.position, newPozZoom, camZoomSpeed);
    }

    //Calucl de la distance de zoom
    float ZoomDistance()
    {
        if(targetList.Count == 1)
            return zoomDistMin;
            ///////////////////

        farDistance = 0f;

        for (int i = 1; i < targetList.Count; i++)
        {
            if (Vector3.Distance(targetList[0].position, targetList[i].position) > farDistance)
                farDistance = Vector3.Distance(targetList[0].position, targetList[i].position);
            if (farDistance > zoomDistMax)
            {
                Debug.LogError("L'une des target est hors de la zone de jeu");
                return zoomDistMax;
                /////////////////
            }
        }

        pctDistance = farDistance / maxDistance;
        return Mathf.Clamp(zoomDistMax - (zoomDistMax * pctDistance), zoomDistMin, zoomDistMax);
    }

    // Retour en mode de base
    public void BackToFreeMode()
    {
        isFreeMode = true;
        targetList.Clear();
    }

   */
}
