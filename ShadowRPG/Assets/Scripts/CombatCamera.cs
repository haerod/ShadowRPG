using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    public float maxY;
    public float minY;
    public float camYModifier;
    public float camYspeed;

    private Vector3 newCamPoz;

	void Awake ()
    {
        newCamPoz = transform.position;
	}
	
	void Update ()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            PozCam(-1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            PozCam(1);
        }
        if(newCamPoz != transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, newCamPoz, camYspeed);
            if (SquadManager.SquadMInstance.panelActions.activeInHierarchy)
            {
                SquadManager.SquadMInstance.UpdatePanelPozOnGameObject(SquadManager.SquadMInstance.panelActions, SquadManager.SquadMInstance.selectedEnemy.transform.position);
            }
            else if (SquadManager.SquadMInstance.panelPower.activeInHierarchy)
            {
                SquadManager.SquadMInstance.UpdatePanelPozOnGameObject(SquadManager.SquadMInstance.panelPower, SquadManager.SquadMInstance.selectedEnemy.transform.position);
            }
        }
    }

    void PozCam (int multiplier)
    {
        float newY = Mathf.Clamp(transform.position.y + (camYModifier * multiplier), minY, maxY);
        newCamPoz = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
