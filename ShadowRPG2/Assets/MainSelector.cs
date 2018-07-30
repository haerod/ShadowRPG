﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSelector : MonoBehaviour
{
    public bool canClick = true;
    public List<Character> charaInitList;

    [HideInInspector]
    public static MainSelector instance;
    [HideInInspector]
    public Character selectedCharacter; // Personnage sur lequel on a cliqué

    private bool isActionStarted;
    private int layerToIgnore = ~(1 << 8); // Layer du décor


    void Awake()
    {
        if (!instance)
            instance = this;

    }

    void Start()
    {
        CreateTurnList();
    }

    void Update()
    {
        if(canClick)
        {
            CheckClick();
        }
    }



    // Check le clic gauche
    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0) && !isActionStarted)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, layerToIgnore))
            {
                //Clique sur un perso
                if (hit.transform.GetComponent<Character>() && (selectedCharacter == null || selectedCharacter.currentAction == "Idle"))
                {
                    Character perso = hit.transform.GetComponent<Character>();
                    // PJ et en vie
                    if ((perso.team == 0 || perso.team == 1 || perso.team == 2) && !perso.isDead)
                    {
                        ActionBar.instance.HideActionBar();
                        selectedCharacter = perso;
                        ActionBar.instance.DisplayActionBar(selectedCharacter, perso == TurnBar.instance.ReturnCurrentCharacter());
                        return;
                    }
                }
                //(IDEM) Clique sur le slot d'un perso
                else if (hit.transform.tag == "Slot" && (selectedCharacter == null || selectedCharacter.currentAction == "Idle"))
                {
                    if (hit.transform.GetComponent<Slot>().currentChara != null)
                    {
                        Character perso = hit.transform.GetComponent<Slot>().currentChara;
                        // PJ et en vie
                        if ((perso.team == 0 || perso.team == 1 || perso.team == 2) && !perso.isDead)
                        {
                            ActionBar.instance.HideActionBar();
                            selectedCharacter = perso;
                            ActionBar.instance.DisplayActionBar(selectedCharacter, perso == TurnBar.instance.ReturnCurrentCharacter());
                            return;
                        }
                    }
                }

                // ACTIONS
                // =======
                // Sélectionne le bon slot s'il fait partie des possibilités d'action du joueur
                else if ((hit.transform.GetComponent<Slot>() || hit.transform.GetComponent<Character>()) && selectedCharacter.currentAction != "Idle")
                {

                    Slot tempSlot = null;
                    if (hit.transform.GetComponent<Slot>())
                    {
                        if (selectedCharacter.allowedSlots.Contains(hit.transform.GetComponent<Slot>()))
                            tempSlot = hit.transform.GetComponent<Slot>();
                    }
                    else if (hit.transform.GetComponent<Character>())
                    {
                        if (selectedCharacter.allowedSlots.Contains(hit.transform.GetComponent<Character>().currentSlot))
                            tempSlot = hit.transform.GetComponent<Character>().currentSlot;
                    }

                    // Sécurité, empêche de lancer la suite si le tempslot est null (le slot ou perso cliqué ne correspondait pas à l'action voulue)
                    if (tempSlot == null)
                    {
                        return;
                        //////////////
                    }

                    // Lance MOUVEMENT
                    if (selectedCharacter.currentAction == "Move")
                    {
                        selectedCharacter.StartMoveCharacter(tempSlot);
                        ActionBar.instance.HideActionBar();
                        ActionBar.instance.UpdateAndDisplaySelectedPlayerFeedback(selectedCharacter);
                        ActionBar.instance.UpdateCoverValue(selectedCharacter);
                        canClick = false;
                        return;
                        //////////////
                    }
                    // Lance MELEE
                    if (selectedCharacter.currentAction == "Melee")
                    {
                        selectedCharacter.StartAttackMelee(tempSlot.currentChara);
                        /*CombatCamera cc = Camera.main.GetComponent<CombatCamera>();
                        cc.targetList.Add(selectedCharacter.transform);
                        cc.targetList.Add(tempSlot.currentChara.transform);
                        cc.isFreeMode = false;*/
                        ActionBar.instance.HideActionBar();
                        isActionStarted = true;
                        return;
                        //////////////
                    }

                    if (selectedCharacter.currentAction == "Distance")
                    {
                        selectedCharacter.StartAttackDistance(tempSlot.currentChara);
                        ActionBar.instance.HideActionBar();
                        isActionStarted = true;
                        return;
                        //////////////
                    }
                }

                // Enlève l'UI en cliquant dans le vide
                else if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && selectedCharacter)
                {
                    ActionBar.instance.HideActionBar();
                    selectedCharacter.currentSlot.HidePossibilities();
                    selectedCharacter.HideRangeCircle();
                    ChangeCharaAction(0);
                    selectedCharacter = null;
                    return;
                    //////////////
                }
            }
        }
    }




    // Si le joueur clique sur le bouton MOUVEMENT
    public void ClickOnMovement()
    {
        selectedCharacter.ClearSlots();
        selectedCharacter.currentSlot.ShowMovementPossibilities();
        ChangeCharaAction(1);
    }

    // Si le joueur clique sur le bouton MELEE
    public void ClickOnMelee()
    {
        selectedCharacter.ClearSlots();
        selectedCharacter.currentSlot.ShowMeleePossibilities();
        ChangeCharaAction(2);
    }

    // Si le joueur clique sur le bouton DISTANCE
    public void ClickOnDistance()
    {
        selectedCharacter.ClearSlots();
        selectedCharacter.currentSlot.ShowDistancePossibilities();
        selectedCharacter.DisplayRangeCircle();
        ChangeCharaAction(3);
    }

    // Si le joueur clique sur le bouton RECHARGER ENERGIE
    public void ClickOnReloadState()
    {
        if(selectedCharacter.currentStage < 6)
        {
            selectedCharacter.ReloadEnergy();
        }
    }



    // Donne la bonne action au chara
    // Si l'index = 0, tous les bools sont false et le personnage sélectionné Clear ses slots
    public void ChangeCharaAction(int boolIndex)
    {

        //selectedCharacter.DistibuteEnergy();
        ActionBar.instance.HideEnergy();

        switch (boolIndex)
        {
            case 0:
                selectedCharacter.currentAction = "Idle";
                selectedCharacter.ClearSlots();
                break;
            case 1:
                selectedCharacter.currentAction = "Move";
                break;
            case 2:
                selectedCharacter.currentAction = "Melee";
                break;
            case 3:
                selectedCharacter.currentAction = "Distance";
                break;
            default:
                Debug.LogError("Cet index n'existe pas !!! (" + boolIndex + ")");
                break;
        }

        ActionBar.instance.GenerateEnergy(selectedCharacter);
    }

    //Arrête l'action lancée et retourne à la barre d'action (optionnel, if true)
    public void StopAction(bool displayActionBar = true)
    {
        isActionStarted = false;
        if(displayActionBar)
            ActionBar.instance.DisplayActionBar(selectedCharacter, true);
    }

    // Crée la turn list
    public void CreateTurnList()
    {
        int min = 0;
        int max = 0;
        List<Character> tempList = new List<Character>();

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++) // Calcule min
        {
            if (Dealer.instance.allCharacters[i].init < min)
                min = Dealer.instance.allCharacters[i].init;
        }

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++) // Calcule max
        {
            if (Dealer.instance.allCharacters[i].init > max)
                max = Dealer.instance.allCharacters[i].init;
        }

        for (int i = min; i < max + 1; i++) // Organise les persos dans l'ordre
        {
            tempList.Clear();
            for (int j = 0; j < Dealer.instance.allCharacters.Length; j++)
                // Pour chaque valeur d'init possible, récupère tous les persos ayant cette init
            {

                if (Dealer.instance.allCharacters[j].init == i)
                    tempList.Add(Dealer.instance.allCharacters[j]);
            }
            if (tempList.Count > 0)
                //Prend les persos possibles et les ajoute à la liste au hasard
            {
                tempList = Dealer.instance.ShuffleObjectList(tempList);
                foreach (Character chara in tempList)
                {
                    charaInitList.Add(chara);
                }
            }
        }

        charaInitList.Reverse(); // Retourne la liste
        TurnBar.instance.CreateBar();
    }
}
