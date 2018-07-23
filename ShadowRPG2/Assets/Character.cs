using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Stats")]

    public string charaName;
    [Tooltip("0 = Joueur, 1 = Enemy, -1 = Allié")]
    public int team;
    public int currentEnergy;
    [SerializeField]
    private int maxEnergy;
    [HideInInspector]
    public int stockedEnergy;
    public int currentStage;
    [SerializeField]
    private int maxStage;
    [SerializeField]
    private string[] stageDescription;
    public int armor;
    public float range;

    private int successes;

    [Header("Divers")]

    public Slot currentSlot;
    [Tooltip("Idle, Move, Melee")]
    public string currentAction = "Idle";  //Penser à les ajouter dans MainSelector.ChangeCharaAction()
    public Transform aimLowPoz;
    public Transform aimHighPoz;
    [SerializeField]
    private float movementSpeed;
    private AttackWindow aw;
    private Character currentTarget;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    [Tooltip("0 = Ne bouge pas, 1 = Mouvement complet, 2 = Mouvement au bord du slot, 3 = Retour au slot d'origine")]
    public int isMovingToSlot;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public List<Slot> allowedSlots;
    [HideInInspector]
    public Character targetedBy;

    private CircleLineRenderer lineRendererRange;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        lineRendererRange = GetComponentInChildren<CircleLineRenderer>();
    }

    void Start()
    {
        if(currentSlot != null)
            PozOnSlot();
        else
            Debug.LogError("Le perso " + charaName + " n'a pas de slot attribué !!!");

        UpdateStage();
    }

    void Update()
    {
        if (isMovingToSlot != 0)
            MoveToSlot(isMovingToSlot);
    }





    //Positionne le personnage au centre du slot
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
        EndAction();
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

    // ATTAQUE EN MELEE : Termine l'attaque 
    public void EndAttackMelee(int successes)
    {
        currentTarget.targetedBy = this;
        Destroy(aw.gameObject);
        MainSelector.instance.StopAction(false);
        MainSelector.instance.ChangeCharaAction(0);
        SetOneBoolAnimTrue("Shoot");
        GetComponentInChildren<ParticleSystem>().Play();
        if (GiveDamages(currentTarget, successes))
            StartPositionTaking();
        else
        {
            isMovingToSlot = 3;
            EndAction();
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
        MainSelector.instance.ChangeCharaAction(0);
        SetOneBoolAnimTrue("Shoot");
        GetComponentInChildren<ParticleSystem>().Play();
        GiveDamages(currentTarget, successes - currentTarget.currentSlot.coverValue);
        EndAction();
        //ClearEnergy();
    }

    // RECHARGER L'ENERGIE
    public void ReloadEnergy()
    {
        print(transform);
        currentStage++;
        currentEnergy = maxEnergy;
        UpdateStage();
        MainSelector.instance.GenerateEnergy();
    }

    // TERMINER UNE ACTION
    void EndAction()
    {
        currentSlot.HidePossibilities();
        currentAction = "Idle";
        ClearSlots();
        successes = 0;
    }



    // Donne les dommages
    public bool GiveDamages(Character target, int damages)
    {
        if (target.ReciveDamages(damages))
            return true;
        return false;
    }

    // Reçoit les dommages et retourne si le personnage est mort
    public bool ReciveDamages(int damages)
    {
        int totalDamages = damages - armor - currentSlot.coverValue;
        if (totalDamages < 0)
            totalDamages = 0;

        ShowFeedbackAction(1, transform.position, totalDamages);

        if (currentStage + totalDamages > 6)
        {
            Death();
            return true;
            ///////
        }
        currentStage += totalDamages;
        UpdateStage();
        return false;
    }

    // Met à jour l'etat du personnage (UI) et bloque l'action reload
    public void UpdateStage()
    {
        Dealer.instance.stageDesription.text = currentStage + "+";
        Dealer.instance.stageDescriptionTextual.text = stageDescription[currentStage];
        for (int i = 0; i < Dealer.instance.stageImagesArray.Length; i++)
        {
            if(i != (currentStage-2))
                Dealer.instance.stageImagesArray[i].sprite = Dealer.instance.spriteVoidState;
            else
                Dealer.instance.stageImagesArray[i].sprite = Dealer.instance.spriteCurrentState;
        }

        if (currentStage < 6 && currentEnergy != maxEnergy)
            Dealer.instance.buttonReloadEnergy.interactable = true;
        else
            Dealer.instance.buttonReloadEnergy.interactable = false;
    } 

    // Le perso meurt
    public void Death()
    {
        isDead = true;
        currentEnergy = 0;
        currentSlot.currentChara = null;
        SetOneBoolAnimTrue("Death");
        TooltipText ttTxt = gameObject.AddComponent<TooltipText>();
        ttTxt.textToDisplay = "Ci-git <b>" + charaName + "</b>, tombé au champ d'honneur.";
        GetComponent<CapsuleCollider>().direction = 2;
    }

    // Roule les dés et ramène le jeu à la normale
    public void RollDices(int engagedDices)
    {
        int stage = currentStage;
        int result;
        for (int i = 0; i < engagedDices; i++)
        {
            result = Random.Range(1, 7);
            if (result >= stage)
            {
                successes++;
            }

            //Dé explosif
            if (result == 6)
            {
                while (result == 6)
                {
                    result = Random.Range(1 , 7);
                    if (result >= stage)
                    {
                        successes++;
                    }
                }
            }
        }

        currentEnergy -= engagedDices;
        if(currentAction == "Melee")
        {
            EndAttackMelee(successes);
            ShowFeedbackAction(0, transform.position, successes);
        }
        else if (currentAction == "Distance")
        {
            EndAttackDistance(successes);
            ShowFeedbackAction(0, transform.position, successes);
        }
    }


    // Met tous les bools del'animator à false (remet le personnage en iddle)
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
    // - Son slot d'origine (3)
    public void MoveToSlot(int typeOfMovment)
    {
        if (typeOfMovment == 1) // Aller sur le slot
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 1f)
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
            if (Vector3.Distance(transform.position, currentTarget.currentSlot.transform.position) <= 2f)
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
        else if(typeOfMovment == 3) // Revenir au slot d'origine
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 0.9f)
            {
                isMovingToSlot = 0;
                SetAllBoolAnimFalse();
                PozOnSlot();
                EndAction();
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
