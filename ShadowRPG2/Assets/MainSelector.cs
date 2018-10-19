using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSelector : MonoBehaviour
{
    public bool canClick = true;

    [HideInInspector]
    public static MainSelector instance;
    //[HideInInspector]
    //public Character selectedCharacter; // Personnage sur lequel on a cliqué

    [HideInInspector]
    public bool isActionStarted;
    private int layerToIgnore = ~(1 << 8); // Layer du décor


    void Awake()
    {
        if (!instance)
            instance = this;
    }

    void Update()
    {
        if (canClick)
        {
            CheckClick();
        }
    }



    // Check le clic gauche
    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0) && !isActionStarted)
        {
            //if (!IsMouseOverBars())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000, layerToIgnore))
                {
                    // Sélectionne un personnage si aucune action n'est en cours
                    if (TurnBar.instance.currentChara.currentAction == Character.Action.Idle)
                    {
                        ClickOnCharaWithoutAction(hit);
                    }

                    // Sélectionne un personnage ou un slot pour y appliquer une action
                    else
                    {
                        ClickOnCharaWithAction(hit);
                    }
                }
            }
        }
        
        else if (Input.GetMouseButtonDown(1) && !isActionStarted)  // DEPLACEMENT AUTO
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, layerToIgnore))
            {
                // Sélectionne un personnage si aucune action n'est en cours
                if (TurnBar.instance.currentChara.currentAction == Character.Action.Idle)
                {

                    if (hit.transform.GetComponent<Slot>())
                    {
                        Slot newSlot = hit.transform.GetComponent<Slot>();
                        if(newSlot.currentChara == null)
                        {
                            TurnBar.instance.currentChara.AutoMove(newSlot);
                        }
                    }
                }
            }
        }
    }

    // CLIC SUR PERSONNAGE (pour afficher dans l'action bar)
    void ClickOnCharaWithoutAction(RaycastHit castHit)
    {
        //Clique sur un perso
        if (castHit.transform.tag == "Character")
        {
            Character chara = castHit.transform.GetComponent<Character>();
            // PJ et en vie
            if ((chara.team == 0 || chara.team == 1 || chara.team == 2) && !chara.isDead)
            {
                bool isPlayable = (chara.team == TurnBar.instance.currentTeam && !chara.isActionEnded); // isPlayable = true IF est de la bonne team et en vie, ELSE = false
                ActionBar.instance.UpdateActionBar(chara, isPlayable);
                if (isPlayable)
                    TurnBar.instance.currentChara = chara;
                return;
            }
        }
        //(IDEM) Clique sur le slot d'un perso
        else if (castHit.transform.tag == "Slot")
        {
            if (castHit.transform.GetComponent<Slot>().currentChara != null)
            {
                Character chara = castHit.transform.GetComponent<Slot>().currentChara;
                // PJ et en vie
                if ((chara.team == 0 || chara.team == 1 || chara.team == 2) && !chara.isDead)
                {
                    bool isPlayable = (chara.team == TurnBar.instance.currentTeam && !chara.isActionEnded); // isPlayable = true IF est de la bonne team et en vie, ELSE = false
                    ActionBar.instance.UpdateActionBar(chara, isPlayable);
                    if (isPlayable)
                        TurnBar.instance.currentChara = chara;
                    return;
                }
            }
        }
    }

    // CLIC SUR UN SLOT (pour lancer une action dessus)
    void ClickOnCharaWithAction(RaycastHit castHit)
    {
        if ((castHit.transform.GetComponent<Slot>() || castHit.transform.GetComponent<Character>()))
        {
            Slot tempSlot = null;
            if (castHit.transform.GetComponent<Slot>()) // Si c'est un slot
            {
                if (TurnBar.instance.currentChara.allowedSlots.Contains(castHit.transform.GetComponent<Slot>())) // S'il est autorisé par l'action
                    tempSlot = castHit.transform.GetComponent<Slot>();
            }
            else if (castHit.transform.GetComponent<Character>()) // Si c'est un chara
            {
                if (TurnBar.instance.currentChara.allowedSlots.Contains(castHit.transform.GetComponent<Character>().currentSlot)) // Si son slot est autorisé par l'action
                    tempSlot = castHit.transform.GetComponent<Character>().currentSlot;
            }

            // Sécurité, empêche de lancer la suite si le tempslot est null (le slot ou perso cliqué ne correspondait pas à l'action voulue)
            if (tempSlot == null)
            {
                return;
                //////////////
            }

            // Lance MOUVEMENT
            if (TurnBar.instance.currentChara.currentAction == Character.Action.Move)
            {
                TurnBar.instance.currentChara.StartMoveCharacter(tempSlot);
                //ActionBar.instance.HideActionBar();
                DisplaySelectedPlayerFeedback(TurnBar.instance.currentChara);
                ActionBar.instance.UpdateCoverValue(TurnBar.instance.currentChara);
                canClick = false;
                return;
                //////////////
            }

            // Lance MELEE
            if (TurnBar.instance.currentChara.currentAction == Character.Action.Melee)
            {
                TurnBar.instance.currentChara.StartMoveToAttackMelee(tempSlot.currentChara);
                //ActionBar.instance.HideActionBar();
                isActionStarted = true;
                return;
                //////////////
            }

            // Lance DISTANCE
            if (TurnBar.instance.currentChara.currentAction == Character.Action.Distance)
            {
                TurnBar.instance.currentChara.StartAttackDistance(tempSlot.currentChara);
                //ActionBar.instance.HideActionBar();
                isActionStarted = true;
                return;
                //////////////
            }
        }

        // Retourne à l'action bar
        else
        {
            InterruptAction();
            return;
            //////////////
        }
    }

    // Coupe l'action pour retoruner à l'action bar
    void InterruptAction()
    {
        TurnBar.instance.currentChara.currentSlot.HidePossibilities();
        TurnBar.instance.currentChara.HideRangeCircle();
        TurnBar.instance.currentChara.ChangeCharaAction(Character.Action.Idle);
        isActionStarted = false;
    }



    // Fait apparaitre le feedback montrant quel PJ a été sélectionné
    public void DisplaySelectedPlayerFeedback(Character chara)
    {
        Dealer.instance.selectedCharaFeedback.position = chara.currentSlot.transform.position;
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(true);
    }

    // Cache le feedback montrant quel PJ a été sélectionné
    public void HideSelectedCharacterFeedback()
    {
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(false);
    }


    // Vérifie si la souris est au-dessus des barres d'UI (action bar et turn bar)
    /*public bool IsMouseOverBars()
    {
        RectTransform rtActionBar = ActionBar.instance.GetComponent<RectTransform>();
        RectTransform rtTurnBar = TurnBar.instance.GetComponent<RectTransform>();

        if (Input.mousePosition.y < rtActionBar.rect.height || Input.mousePosition.y > (Screen.height - rtTurnBar.rect.height))
            return true;

        return false;
    }*/
}

