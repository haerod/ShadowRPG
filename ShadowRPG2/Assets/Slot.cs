using System.Collections;
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
    //Animator anim;

    void Start ()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Dealer.instance.neutralSlot;
        GenerateConnexions();
        GetComponent<TooltipText>().textToDisplay = "<b>Couverture</b> : " + coverValue + "\n\n" + "La couverture d'une position réduit les chances d'être touché à distance.";
        //anim = GetComponentInChildren<Animator>();
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
    public List<Slot> GetMovePossibilities()
    {
        List<Slot> tocheckList = new List<Slot>();
        List<Slot> stockList = new List<Slot>();
        Slot curSlot;
        List<Slot> movableSlotList = new List<Slot>();

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
                            movableSlotList.Add(curSlot);
                            //curSlot.sr.color = Dealer.instance.freeSlot;
                        }
                    }
                    else
                    {
                        //curSlot.sr.color = Dealer.instance.forbiddenSlot;
                    }
                }
            }
        }

        return movableSlotList;
    }

    // Retourne le chemin de slots le plus court sous la forme d'une liste de Slots
    // Les autres personnages sont considérés comme des éléments bloquants
    public List<Slot> ReturnPathfinding(Slot slotToGo)
    {
        List<List<Slot>> allPathes = new List<List<Slot>>();
        List<List<Slot>> stockList = new List<List<Slot>>();
        List<Slot> verifiedSlots = new List<Slot>();

        allPathes.Add(new List<Slot> { this });

        for (int i = 0; i < 10; i++) // WHILE (avec un max de 20 boucles)
        {
            for (int j = 0; j< allPathes.Count; j++) // Pour chaque path à vérifier
            {
                Slot slotToCheck = allPathes[j][allPathes[j].Count - 1]; // Récupère le dernier slot de la liste (celui dot on va vérifier les connexions)

                for (int k = 0; k < slotToCheck.connectedSlots.Length; k++) // Pour chaque connected slot
                {
                    Slot currentConnectedSlot = slotToCheck.connectedSlots[k];

                    if (currentConnectedSlot.currentChara == null) // S'il n'y a pas de personnage sur le slot
                    {
                        // SI c'est le bon slot 
                        if (currentConnectedSlot == slotToGo) // SLOT TROUVE
                        {
                            List<Slot> endList = new List<Slot>();
                            for (int n = 0; n < allPathes[j].Count; n++)
                            {
                                endList.Add(allPathes[j][n]);
                            }
                            endList.Add(currentConnectedSlot);
                            endList.Remove(this);

                            for (int z = 0; z < endList.Count; z++)
                            {
                                endList[z].GetComponentInChildren<SpriteRenderer>().color = Dealer.instance.freeSlot;
                            }

                            return endList;
                        }
                        // SINON
                        else
                        {
                            if (verifiedSlots.Contains(currentConnectedSlot)) // Si le slot a déjà été vérifié
                            {
                                // Ne rien faire, ça sera cleané après
                            }
                            else // Sinon l'ajouter à une List qui sera ajoutée à la stockList
                            {
                                List<Slot> listToAdd = new List<Slot>();
                                for (int m = 0; m < allPathes[j].Count; m++)
                                {
                                    listToAdd.Add(allPathes[j][m]);
                                }
                                listToAdd.Add(currentConnectedSlot);
                                stockList.Add(listToAdd);
                            }
                        }
                    }
                }
            }

            allPathes.Clear(); // Vider allPathes

            for (int l = 0; l < stockList.Count; l++) // Ajouter tous les path de la stocklist dans allPathes
            {
                allPathes.Add(stockList[l]);
            }

            stockList.Clear(); // Vider la stocklist
        }

        return null;
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
