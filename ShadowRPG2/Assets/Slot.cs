using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Slot[] connectedSlots;
    public Character currentChara;
    public int coverValue;


	void Start ()
    {
        GenerateConnexions();
        GetComponent<TooltipText>().textToDisplay = "<b>Couverture</b> : " + coverValue + "\n\n" + "La couverture d'une position réduit les chances d'être touché à distance.";
    }

    // Crée des objets vides avec des Line renders pour créer des connexions
    void GenerateConnexions()
    {
        float sizeLR = 0.1f;

        for (int i = 0; i < connectedSlots.Length; i++)
        {
            GameObject newGo = new GameObject();
            newGo.transform.position = transform.position;
            newGo.transform.parent = transform;
            Quaternion rot = Quaternion.Euler(90, 0, 0);
            newGo.transform.rotation = rot;
            newGo.name = "LineRenderer " + gameObject.name + "-" + connectedSlots[i].name;
            LineRenderer lr = newGo.AddComponent<LineRenderer>();
            lr.startWidth = sizeLR;
            lr.endWidth = sizeLR;
            lr.alignment = LineAlignment.Local;
            lr.material = Dealer.instance.connectedSlotsLR;

            // Formule pour mettre les points de départ à une certaine distance du centre du slot : Centre + VecDirection * rayon
                // Centre = Centre du slot de départ
                // VecDirection = (Point d'arrivée - point de départ).normalized
                // rayon = rayon du cercle (distance à laquelle on veut mettre le point
            Vector3 offsetedPoz = (connectedSlots[i].transform.position - transform.position).normalized; // calucle VecDirection
            offsetedPoz *= 1.4f; // multiplie par Rayon du cercle
            offsetedPoz += transform.position; // ajoute Centre

            lr.SetPosition(0, offsetedPoz);
            lr.SetPosition(1, (connectedSlots[i].transform.position + transform.position) / 2);
        }
    }

    // Change les materials des slots où le mouvement est possible
    public void ShowMovementPossibilities()
    {
        for (int i = 0; i < connectedSlots.Length; i++)
        {
            if(connectedSlots[i].currentChara == null)
            {
                currentChara.allowedSlots.Add(connectedSlots[i]);
                connectedSlots[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.freeSlot;
            }
            else
            {
                connectedSlots[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.forbiddenSlot;
            }
        }
    }

    // Change le material des slots où l'attaque en melée est possible
    public void ShowMeleePossibilities()
    {
        for (int i = 0; i < connectedSlots.Length; i++)
        {
            if (connectedSlots[i].currentChara != null)
            {
                currentChara.allowedSlots.Add(connectedSlots[i]);
                connectedSlots[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.attackableSlot;
            }
            else
            {
                connectedSlots[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.forbiddenSlot;
            }
        }
    }

    // Change le material des slots où l'attaque à distance est possible
    public void ShowDistancePossibilities()
    {
        RaycastHit hit;

        for (int i = 0; i < Dealer.instance.allSlotArray.Length; i++)
        {
            if(Vector3.Distance(transform.position, Dealer.instance.allSlotArray[i].transform.position) <= currentChara.range)
            {
                Debug.DrawRay(transform.position, (Dealer.instance.allSlotArray[i].transform.position - transform.position) * 2, Color.blue, Mathf.Infinity);

                if (Physics.Raycast(transform.position, (Dealer.instance.allSlotArray[i].transform.position - transform.position), out hit))
                {
                    if(hit.transform.tag == "Slot" || hit.transform.tag == "Character")
                    {
                        currentChara.allowedSlots.Add(Dealer.instance.allSlotArray[i]);
                        Dealer.instance.allSlotArray[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.attackableSlot;
                    }
                    else
                    {
                        Dealer.instance.allSlotArray[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.forbiddenSlot;
                    }
                }
                else
                {
                    Dealer.instance.allSlotArray[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.forbiddenSlot;
                }
            }
            else
            {
                Dealer.instance.allSlotArray[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.forbiddenSlot;
            }
        }
    }

    // Rend le material d'origine aux slots où le mouvement est possible
    public void HidePossibilities()
    {
        for (int i = 0; i < Dealer.instance.allSlotArray.Length; i++)
        {
            Dealer.instance.allSlotArray[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.neutralSlot;
        }
    }
}
