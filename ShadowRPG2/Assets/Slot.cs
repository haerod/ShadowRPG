﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Slot[] connectedSlots;
    public int coverValue;
    [SerializeField] LayerMask lmIgnoreAim;

    [HideInInspector] public Character currentChara;

    List<Slot> slots = new List<Slot>(); // Pour ShowPossiblities()
    List<Slot> dustbin = new List<Slot>(); // Pour ShowPossibilities()

    SpriteRenderer sr;
    Animator anim;

    void Start ()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Dealer.instance.neutralSlot;
        GenerateConnexions();
        GetComponent<TooltipText>().textToDisplay = "<b>Couverture</b> : " + coverValue + "\n\n" + "La couverture d'une position réduit les chances d'être touché à distance.";
        anim = GetComponentInChildren<Animator>();
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
                connectedSlots[i].sr.color = Dealer.instance.freeSlot;
            }
            else
            {
                connectedSlots[i].sr.color = Dealer.instance.forbiddenSlot;
            }
        }
    }

    // Retourne true si le slot à tester est adjacent au slot faisant le test
    public bool IsNearby(Slot SlotToTest)
    {
        for (int i = 0; i < connectedSlots.Length; i++)
        {
            if (connectedSlots[i] == SlotToTest)
            {
                return true;
            }
        }
        return false;
    }

    // Change les materials des slots où la course est possible, en fonction du nombre de PE disponibles 
    // FONCTIONNEMENT : La fonction a deux listes de Slot : tockeckList qui contient tous les slots où vérifier les slots connectés pour s'étendre, et la stockList, qui contient les futurs slots à checker (ceux qui ont été nouvellement ajoutés).
    public void ShowRunPossibilities()
    {
        List<Slot> tocheckList = new List<Slot>();
        List<Slot> stockList = new List<Slot>();
        Slot curSlot;

        tocheckList.Add(this);

        for(int i = 0; i < currentChara.currentEnergy; i++ ) // Pour chaque PE du chara
        {


            if (i != 0) // Attibue tous les éléments de la stocklist à la tocheckList
            {
                tocheckList.Clear();
                for (int l = 0; l < stockList.Count; l++)
                {
                    tocheckList.Add(stockList[l]);
                }
                stockList.Clear();
            }

            for (int j = 0; j < tocheckList.Count; j++) // Pour chaque élément de la tocheckList, vérifier les slots adjacents
            {

                for (int k = 0; k < tocheckList[j].connectedSlots.Length; k++) 
                    // Pour chaque slot connecté, vérifier s'il est à disponible et n'existe pas déjà dans allowedSlot
                {
                    curSlot = tocheckList[j].connectedSlots[k];
                    if (curSlot.currentChara == null)
                    {
                        if (!currentChara.allowedSlots.Contains(curSlot))
                        {
                            stockList.Add(curSlot);
                            currentChara.allowedSlots.Add(curSlot);
                            curSlot.sr.color = Dealer.instance.freeSlot;
                        }
                    }
                    else
                    {
                        curSlot.sr.color = Dealer.instance.forbiddenSlot;
                    }
                }
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
                connectedSlots[i].sr.color = Dealer.instance.attackableSlot;
            }
            else
            {
                connectedSlots[i].sr.color = Dealer.instance.forbiddenSlot;
            }
        }
    }

    // Change le material des slots où l'attaque à distance est possible
    // NB : Pour rendre cette fonction plus lisible, elle est une succession de foreach qui éliminent des possibilités au fur et à mesure -> C'est pas opti, je sais
    public void ShowDistancePossibilities()
    {
        RaycastHit hit;
        //Slot targetSlot;
        //SpriteRenderer targetSr;
        Vector3 origin = currentChara.aimLowPoz.position;
        Vector3 end;

        //1. Crée une list de Slot contenant tous les slots du niveau
        for (int i = 0; i < Dealer.instance.allSlotArray.Length; i++)
        {
            slots.Add(Dealer.instance.allSlotArray[i]);
        }

        sr.color = Dealer.instance.forbiddenSlot;
        dustbin.Add(this);
        CleanDustbin();

        //2. Elimine les slots étant trop loin
        foreach (Slot slot in slots)
        {
            if (Vector3.Distance(transform.position, slot.transform.position) >= currentChara.range)
            {
                //slot.sr.color = Dealer.instance.forbiddenSlot;
                dustbin.Add(slot);
            }
        }
        CleanDustbin();

        //3. Elimine les slots vides
        foreach (Slot slot in slots)
        {
            if(slot.currentChara == null)
            {
                slot.sr.color = Dealer.instance.forbiddenSlot;
                dustbin.Add(slot);
            }
        }
        CleanDustbin();

        //4. Elimine les slots avec lesquels, en cas de raycast bas, il y a collision avec un autre personnage
        foreach (Slot slot in slots)
        {
            end = Dealer.instance.SetVectorY(slot.transform.position, currentChara.aimLowPoz.position.y);

            if (Physics.Raycast(origin, (end - origin), out hit, lmIgnoreAim))
            {
                if (hit.transform.tag == "Character")
                {
                    if(hit.transform.GetComponent<Character>() != slot.currentChara)
                    {
                        slot.sr.color = Dealer.instance.forbiddenSlot;
                        dustbin.Add(slot);
                    }
                }
            }
        }
        CleanDustbin();

        //5. Elimine les slots avec lesquels, en cas de raycast haut, il y a collision avec un mur
        foreach (Slot slot in slots)
        {
            end = Dealer.instance.SetVectorY(slot.transform.position, currentChara.aimLowPoz.position.y);

            if (Physics.Raycast(origin, (end - origin), out hit, lmIgnoreAim))
            {
                if (hit.transform.tag == "Obstacle")
                {
                    slot.sr.color = Dealer.instance.forbiddenSlot;
                    dustbin.Add(slot);
                }
            }
        }
        CleanDustbin();

        //6. Valide les slots restants
        foreach (Slot slot in slots)
        {
            currentChara.allowedSlots.Add(slot);
            slot.sr.color = Dealer.instance.attackableSlot;
        }

        slots.Clear();
    }

    // Rend le material d'origine à tous les slots
    public void HidePossibilities()
    {
        for (int i = 0; i < Dealer.instance.allSlotArray.Length; i++)
        {
            Dealer.instance.allSlotArray[i].sr.color = Dealer.instance.neutralSlot;
        }
    }

    void OnMouseOver()
    {
        anim.SetBool("Loop", true);
    }

    void OnMouseExit()
    {
        anim.SetBool("Loop", false);
    }



    // Elimine les slots à éliminer dans la List slots
    void CleanDustbin()
    {
        foreach (Slot slot in dustbin)
        {
            slots.Remove(slot);
        }
        dustbin.Clear();
    }
}
