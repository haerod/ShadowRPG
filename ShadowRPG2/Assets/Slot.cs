using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Slot[] connectedSlots;
    public int coverValue;
    [HideInInspector]
    public Character currentChara;

    private int layerIgnoreAim = ~(1 >> 9);


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
        Slot targetSlot;
        SpriteRenderer targetSr;
        Vector3 origin = currentChara.aimLowPoz.position;
        Vector3 end;

        for (int i = 0; i < Dealer.instance.allSlotArray.Length; i++)
        {
            targetSlot = Dealer.instance.allSlotArray[i];
            targetSr = targetSlot.GetComponentInChildren<SpriteRenderer>();
            end = Dealer.instance.SetVectorY(targetSlot.transform.position, currentChara.aimLowPoz.position.y);

            if (Vector3.Distance(transform.position, targetSlot.transform.position) <= currentChara.range && targetSlot.currentChara != null && targetSlot != this) 
                // Elimine les slots hors de portée, vides et lui-même
            {

                if (Physics.Raycast(origin, (end - origin), out hit, layerIgnoreAim)) // Raycast les slots
                {
                    Debug.DrawLine(hit.transform.position, hit.transform.position + Vector3.up, Color.cyan, Mathf.Infinity);
                    Debug.DrawLine(origin, end, Color.blue, Mathf.Infinity);

                    if (hit.transform.tag == "Obstacle") // Si c'est un obstacle
                    {
                        origin = currentChara.aimHighPoz.position;
                        end = Dealer.instance.SetVectorY(targetSlot.transform.position, currentChara.aimHighPoz.position.y);

                        if (Physics.Raycast(origin, (end - origin), out hit, layerIgnoreAim)) // Raycast le slot depuis plus haut
                        {
                            Debug.DrawLine(origin, end, Color.green, Mathf.Infinity);

                            if (hit.transform.tag == "Character") // Si c'est un personnage
                            {
                                if (hit.transform.GetComponent<Character>().currentSlot == targetSlot.currentChara) // Si c'est le personnage du slot
                                {
                                    currentChara.allowedSlots.Add(targetSlot);
                                    targetSr.color = Dealer.instance.attackableSlot;
                                }
                                else
                                {
                                    targetSr.color = Dealer.instance.forbiddenSlot;
                                }
                            }
                            else
                            {
                                targetSr.color = Dealer.instance.forbiddenSlot;
                            }
                        }
                    }
                    else if (hit.transform.tag == "Character") // Si c'est un personnage
                    {
                        print(hit.transform.GetComponent<Character>().currentSlot == targetSlot.currentChara);
                        if (hit.transform.GetComponent<Character>().currentSlot == targetSlot.currentChara) // Si c'est le personnage du slot
                        {
                            currentChara.allowedSlots.Add(targetSlot);
                            targetSr.color = Dealer.instance.attackableSlot;
                        }
                        else
                        {
                            targetSr.color = Dealer.instance.forbiddenSlot;
                        }
                    }
                    else
                    {
                        targetSr.color = Dealer.instance.forbiddenSlot;
                    }
                }
                else
                {
                    currentChara.allowedSlots.Add(targetSlot);
                    targetSr.color = Dealer.instance.attackableSlot;
                }
            }
            else
            {
                targetSr.color = Dealer.instance.forbiddenSlot;
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
