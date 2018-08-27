﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Stats")]

    public string charaName;
    public int team;
    public int currentEnergy;
    public int maxEnergy;
    [HideInInspector]
    public int stockedEnergy;
    public int currentLife;
    public int armor;
    public float range;
    public int init;
    public int[] percentByLife;

    private int successes;

    [Header("Divers")]

    public AudioClip charaTrack;
    public Slot currentSlot;
    [Tooltip("Idle, Move, Melee")]
    public string currentAction = "Idle";
    public Transform aimLowPoz;
    public Transform aimHighPoz;
    [SerializeField] float movementSpeed;
    public Sprite charaImage;


    private AttackWindow aw;
    [HideInInspector] public Character currentTarget;
    [HideInInspector] public Animator anim;
    [HideInInspector] public int isMovingToSlot;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public List<Slot> allowedSlots;
    [HideInInspector] public Character targetedBy;
    [HideInInspector] public TurnTile turnTile;

    CircleLineRenderer lineRendererRange;
    TooltipText ttTxt;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        lineRendererRange = GetComponentInChildren<CircleLineRenderer>();
        ttTxt = gameObject.AddComponent<TooltipText>();
        ttTxt.textToDisplay = charaName;
    }

    void Start()
    {
        if(currentSlot != null)
            PozOnSlot();
        else
            Debug.LogError("Le perso " + charaName + " n'a pas de slot attribué !!!");

        //ActionBar.instance.UpdateStage(this);
    }

    void Update()
    {
        if (isMovingToSlot != 0)
            MoveToSlot(isMovingToSlot);
    }





    /// <summary>
    /// Positionne le personnage au centre du slot
    /// </summary>
    void PozOnSlot()
    {
        Vector3 vecPozSlot = new Vector3(currentSlot.transform.position.x, transform.position.y, currentSlot.transform.position.z);
        transform.position = vecPozSlot;
        currentSlot.currentChara = this;
    }

    //Vide la liste allowedSlots
    public void ClearSlots()
    {
        currentSlot.HidePossibilities();
        HideRangeCircle();
        allowedSlots.Clear();
    }




    //MOUVEMENT : Lance le personnage
    public void StartMoveCharacter(Slot newSlot)
    {
        SetOneBoolAnimTrue("Run");
        isMovingToSlot = 1;
        currentSlot.currentChara = null;
        currentSlot = newSlot;
    }

    //MOUVEMENT : Arrête le personnage
    public void EndMoveCharacter()
    {
        MainSelector.instance.canClick = true;
        SetAllBoolAnimFalse();
        PozOnSlot();
        StartCoroutine(EndAction());
        //Camera.main.GetComponent<CombatCamera>().BackToFreeMode();
    }

    // ATTAQUE EN MELEE : Crée la fenêtre
    public void StartAttackMelee(Character enemy)
    {
        currentTarget = enemy;
        Dealer.instance.LookAtYAxis(transform, currentTarget.transform.position);
        aw = Instantiate(Dealer.instance.attackWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<AttackWindow>();
        aw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.attackWindow.GetComponent<RectTransform>().anchoredPosition;
        aw.chara = this;
        aw.ConstructBar();

        isMovingToSlot = 2;
        SetOneBoolAnimTrue("Run");  
    }

    // ATTAQUE EN MELEE : Termine l'attaque (avant anim)
    void EndAttackMelee1()
    {
        currentTarget.targetedBy = this;
        Destroy(aw.gameObject);
        MainSelector.instance.StopAction(false);
        ChangeCharaAction(0);
        SetOneBoolAnimTrue("Punch");
    }

    // ATTAQUE EN MELEE : Termine l'attaque (après anim)
    public void EndAttackMelee2()
    {
        if (GiveDamages(currentTarget, successes))
            StartPositionTaking();
        else
        {
            SetOneBoolAnimTrue("Run");
            isMovingToSlot = 3;
            //StartCoroutine(EndAction());
        }
    }

    // PRISE DE POSITION : Crée la fenêtre
    public void StartPositionTaking()
    {
        GameObject ptw = Instantiate(Dealer.instance.positionTakingWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas);
        ptw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.positionTakingWindow.GetComponent<RectTransform>().anchoredPosition;
        ptw.GetComponent<PositionTakingWindow>().linkedChara = this;
        MainSelector.instance.canClick = false;
    }

    // PRISE DE POSITION : Lance la prise de position
    public void EndPositionTaking()
    {
        StartMoveCharacter(currentTarget.currentSlot);
    }

    // ATTAQUE A DISTANCE : Crée la fenêtre
    public void StartAttackDistance(Character enemy)
    {
        currentTarget = enemy;
        Dealer.instance.LookAtYAxis(transform, currentTarget.transform.position);
        aw = Instantiate(Dealer.instance.attackWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<AttackWindow>();
        aw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.attackWindow.GetComponent<RectTransform>().anchoredPosition;
        aw.chara = this;
        aw.attackName = "Attaque à distance";
        aw.ConstructBar();
    }

    // ATTAQUE A DISTANCE : Exécute l'attaque
    public void EndAttackDistance(int successes)
    {
        HideRangeCircle();
        currentTarget.targetedBy = this;
        Destroy(aw.gameObject);
        MainSelector.instance.StopAction(false);
        ChangeCharaAction(0);
        SetOneBoolAnimTrue("Shoot");
        GiveDamages(currentTarget, successes - currentTarget.currentSlot.coverValue);
        StartCoroutine(EndAction());
    }

    // RECHARGER L'ENERGIE
    public void ReloadEnergy()
    {
        currentLife--;
        currentEnergy = maxEnergy;
        ActionBar.instance.UpdateStage(this);
        ActionBar.instance.UpdateEnergy(this);
    }

    // PASSER SON TOUR
    public void PassTurn()
    {
        StartCoroutine(EndAction());
    }



    // TERMINER UNE ACTION
    IEnumerator EndAction()
    {
        currentSlot.HidePossibilities();
        currentAction = "Idle";

        yield return new WaitForEndOfFrame();

        SetAllBoolAnimFalse();
        ClearSlots();
        successes = 0;
        MainSelector.instance.HideSelectedCharacterFeedback();
        TurnBar.instance.NextCharaTurn();

    }

    // Donne la bonne action au chara
    // Si l'index = 0, tous les bools sont false et le personnage sélectionné Clear ses slots
    public void ChangeCharaAction(int boolIndex)
    {
        switch (boolIndex)
        {
            case 0:
                currentAction = "Idle";
                ClearSlots();
                break;
            case 1:
                currentAction = "Move";
                break;
            case 2:
                currentAction = "Melee";
                break;
            case 3:
                currentAction = "Distance";
                break;
            default:
                Debug.LogError("Cet index n'existe pas !!! (" + boolIndex + ")");
                break;
        }

        ActionBar.instance.UpdateEnergy(this);
    }
    





    // Donne les dommages
    public bool GiveDamages(Character target, int damages)
    {
        if (target.ReciveDamages(damages))
            return true;
        return false;
    }

    // Reçoit les dommages et retourne TRUE si le personnage est mort
    public bool ReciveDamages(int damages)
    {
        int totalDamages = damages - armor - currentSlot.coverValue;
        if (totalDamages < 0)
            totalDamages = 0;

        ShowFeedbackAction(1, transform.position, totalDamages);

        if (currentLife - totalDamages <= 0)
        {
            Death();
            TurnBar.instance.RemoveTileAt(turnTile.index);
            return true;
            ///////
        }
        currentLife -= totalDamages;
        ActionBar.instance.UpdateStage(this);
        return false;
    }

    // Le perso meurt
    public void Death()
    {
        isDead = true;
        currentEnergy = 0;
        currentSlot.currentChara = null;
        SetOneBoolAnimTrue("Death");
        ttTxt.textToDisplay = "Ci-git <b>" + charaName + "</b>, tombé au champ d'honneur.";
        GetComponent<CapsuleCollider>().direction = 2;
    }

    // Roule les dés et ramène le jeu à la normale
    public void RollDices(int engagedDices)
    {
        int pct = percentByLife[currentLife];
        int result;
        for (int i = 0; i < engagedDices; i++)
        {
            result = Random.Range(1, 101);
            if (result <= pct)
            {
                successes++;
            }

            //Dé explosif
            if (result <= 15)
            {
                while (result == 6)
                {
                    result = Random.Range(1 , 101);
                    if (result <= 15)
                    {
                        successes++;
                    }
                }
            }
        }

        currentEnergy -= engagedDices;
        ShowFeedbackAction(0, transform.position, successes);
        if (currentAction == "Melee")
        {
            EndAttackMelee1();
        }
        else if (currentAction == "Distance")
        {
            EndAttackDistance(successes);
        }
    }

    // Met tous les bools de l'animator à false (remet le personnage en iddle)
    public void SetAllBoolAnimFalse()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }

    }

    // Met le bool de l'animator demandé à true et tous les autres à false
    public void SetOneBoolAnimTrue(string boolName)
    {
        SetAllBoolAnimFalse();
        anim.SetBool(boolName, true);
    }

    // Emmène le perso vers le slot :
    // - Sur le slot (1)
    // - Au bord du slot (2)
    // - Son slot d'origine : termine le tour (3)
    // - Son slot d'origine : continue le tour (4)
    public void MoveToSlot(int typeOfMovment)
    {
        if(typeOfMovment >= 1 && typeOfMovment <= 4)
        {
            SetOneBoolAnimTrue("Run");
        }

        if (typeOfMovment == 1) // Aller sur le slot
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 1.2f)
            {
                isMovingToSlot = 0;
                EndMoveCharacter();
                return;
                //////////////
            }

            Dealer.instance.LookAtYAxis(transform, currentSlot.transform.position);
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
        }
        else if (typeOfMovment == 2) // Aller au bord du slot
        {
            if (Vector3.Distance(transform.position, currentTarget.currentSlot.transform.position) <= 2.5f)
            {
                isMovingToSlot = 0;
                SetAllBoolAnimFalse();
                return;
                ////////////
            }
            else
            {

                Dealer.instance.LookAtYAxis(transform, currentTarget.currentSlot.transform.position);
                transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
            }
        }
        else if(typeOfMovment == 3 || typeOfMovment == 4) // Revenir au slot d'origine (et termine l'action)
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 1.2f)
            {
                isMovingToSlot = 0;
                SetAllBoolAnimFalse();
                PozOnSlot();
                if(typeOfMovment == 3)
                    StartCoroutine(EndAction());
                return;
                //////////////
            }

            Dealer.instance.LookAtYAxis(transform, currentSlot.transform.position);
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
        }
    }

    //Génère un feedback indiquant le nombre de :
    // - de réussites > 0
    // - de dommages > 1
    void ShowFeedbackAction(int typeOfFb, Vector3 pozWorldSpace, int value)
    {
        Vector3 poz = Camera.main.WorldToScreenPoint(pozWorldSpace);
        poz = new Vector3(poz.x, poz.y + 75, poz.z);
        GameObject fb = Instantiate(Dealer.instance.feedbackAction, poz, Quaternion.identity, Dealer.instance.mainCanvas);
        fb.GetComponentInChildren<Text>().text = value.ToString();
        Image img = fb.GetComponentInChildren<Image>();

        switch(typeOfFb)
        {
            case 0: //REUSSITES
                img.sprite = Dealer.instance.imageSuccess;
                break;
            case 1: //DOMMAGES
                img.sprite = Dealer.instance.imageDamages;
                break;
        }

        Destroy(fb.gameObject, 2f);
    }

    // Affiche le cercle de portée du personnage
    public void DisplayRangeCircle()
    {
        lineRendererRange.xradius = range/2 + ((range/10));
        lineRendererRange.ConstructCircle();
        lineRendererRange.GetComponent<LineRenderer>().enabled = true;
    }

    // Cache le cercle de portée du personnage
    public void HideRangeCircle()
    {
        lineRendererRange.GetComponent<LineRenderer>().enabled = false;
    }

}
