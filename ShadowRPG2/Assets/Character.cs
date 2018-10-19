using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region VARIABLES

    [Header("Stats")]

    public string charaName;
    public int team;
    public int currentEnergy;
    public int maxEnergy;
    public int currentLife;
    public int maxLife;
    public int armor;
    public float range;
    
    private int successes;
    [HideInInspector] public int stockedEnergy;

    [Header("Skills")]

    public int skillDistance;
    public int skillMelee;
    public int skillDodge;
    public int skillParry;

    [Header("World UI")]

    public GameObject panelHealthEnergy;
    [SerializeField] RectTransform healthBar;
    [SerializeField] RectTransform energyBar;
    [SerializeField] CanvasScaler canvasScaler;

    [Header("Divers")]

    public AudioClip charaTrack;
    public Slot currentSlot;
    public Weapon currentWeapon;
    public enum Action { Melee, Distance, /* INSEREZ PROCHAINE ACTION ICI */ Idle, Move, Run};
    public Transform aimLowPoz;
    public Transform aimHighPoz;
    [SerializeField] float movementSpeed;
    public Sprite charaImage;
    public Transform faceCamTrans;
    public Transform leftShoulderCamTrans;
    public bool isActionEnded;

    NewActionWindow aw;
     public Action currentAction;
    [HideInInspector] public Character currentTarget;
    [HideInInspector] public Animator anim;
    [HideInInspector] public int isMovingToSlot;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public List<Slot> allowedSlots;
    [HideInInspector] public Character targetedBy;
    [HideInInspector] public TurnTile turnTile;
    [HideInInspector] public FeedbackWindow fw;

    enum TypeOfFeedback { Successes, Damages};
    CircleLineRenderer lineRendererRange;
    TooltipText ttTxt;
    List<Slot> path = new List<Slot>();

    #endregion

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        lineRendererRange = GetComponentInChildren<CircleLineRenderer>();
        ttTxt = gameObject.AddComponent<TooltipText>();
        ttTxt.textToDisplay = charaName;
        faceCamTrans = transform.GetChild(0).transform;
        currentAction = Action.Idle;
    }

    void Start()
    {
        if(currentSlot != null)
            PozOnSlot();
        else
            Debug.LogError("Le perso " + charaName + " n'a pas de slot attribué !!!");

        energyBar.sizeDelta = new Vector2(
            (Dealer.instance.energyPref.GetComponent<RectTransform>().rect.width * maxEnergy + energyBar.GetComponent<HorizontalLayoutGroup>().spacing * maxEnergy) / (canvasScaler.dynamicPixelsPerUnit*2),
            energyBar.rect.height);

        UpdateHealth();
        UpdateEnergy();
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
        int totalDamages = damages - armor;
        if (totalDamages < 0)
            totalDamages = 0;

        ShowFeedbackAction(TypeOfFeedback.Damages, transform.position, totalDamages);

        if (currentLife - totalDamages <= 0)
        {
            Death();
            return true;
            ///////
        }
        currentLife -= totalDamages;
        UpdateHealth();
        ActionBar.instance.UpdateHealth(this);
        return false;
    }

    // Le perso meurt
    public void Death()
    {
        isDead = true;
        currentEnergy = 0;
        currentSlot.currentChara = null;
        panelHealthEnergy.SetActive(false);
        SetOneBoolAnimTrue("Death");
        Destroy(ttTxt);
        Destroy(GetComponent<CapsuleCollider>());
    }

    // Convertit les PE en réussites
    public void RollSuccesses(int engagedEnergy)
    {
        int pct = SkillsValueDealer();
        int result;
        for (int i = 0; i < engagedEnergy; i++)
        {
            result = Random.Range(1, 101);
            if (result <= pct)
            {
                successes++;
            }

            /*//Dé explosif
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
            }*/
        }

        currentEnergy -= engagedEnergy;

        if(currentAction == Action.Melee || currentAction == Action.Distance)
        {
            currentTarget.RollOppsiteSuccesses(successes, currentAction);
        }

        DisplayFeedbackActionWindow();

        int successesToDisplay = 0;

        if (currentAction == Action.Melee)
        {
            ExecuteAttackMelee();
            successesToDisplay = successes - currentTarget.successes;
        }
        else if (currentAction == Action.Distance)
        {
            EndAttackDistance();
            successesToDisplay = successes - currentTarget.successes - currentTarget.currentSlot.coverValue;
        }
        else
        {
            print("current action changed : " + currentAction);
        }

        ShowFeedbackAction(TypeOfFeedback.Successes, transform.position, successesToDisplay);
    }

    // Teste les PE de défense et retourne les succès
    public void RollOppsiteSuccesses(int successesToOppose, Action actionType)
    {
        int successesToHave = 0;
        int skillReaction = 0;

        if(actionType == Action.Melee) // Si c'est une attaque en melee, ne prend pas en compte la cover
        {
            successesToHave = successesToOppose;
            skillReaction = skillParry;
        }
        else if(actionType == Action.Distance) // Si c'est une attaque à distance, prend en compte la cover
        {
            successesToHave = successesToOppose - currentSlot.coverValue;
            skillReaction = skillDodge;
        }

        if (successesToHave > 0) // S'il y a des réussites à faire, tester les PE
        {
            successes = 0;
            int result;

            while (successes < successesToHave)
            {
                if (currentEnergy == 0)
                {
                    break;
                }

                result = Random.Range(1, 101);
                if (result <= skillReaction)
                {
                    successes++;
                }
                currentEnergy--;

            }

            UpdateEnergy();
            //ShowFeedbackAction(TypeOfFeedback.Successes, transform.position, successes);
        }
    }

    // Fait apparaître la feedback window
    void DisplayFeedbackActionWindow()
    {
        fw = Instantiate(Dealer.instance.feedbackWindow, Dealer.instance.feedbackWindow.transform.position, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<FeedbackWindow>();
        fw.chara = this;
        fw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.feedbackWindow.GetComponent<RectTransform>().anchoredPosition;
        if (currentAction == Action.Melee)
            fw.buttonEndTurn.interactable = false;

        // ATTAQUE
        if (currentAction == Action.Distance)
            fw.attackTxt.text += "\n";
        fw.attackTxt.text += "REUSSITES : " + successes + "\n";
        fw.attackTxt.text += "REUSSITES CRITIQUES : 0\n";
        fw.attackTxt.text += "\n<b>TOTAL : " + successes + "</b>";

        int totalOppositeDistance = currentTarget.successes + currentTarget.currentSlot.coverValue;

        // DEFENSE
        if (currentAction == Action.Distance)
            fw.defenseTxt.text += "COUVERTURE : " + currentTarget.currentSlot.coverValue + "\n";
        fw.defenseTxt.text += "REUSSITES : " + currentTarget.successes + "\n";
        fw.defenseTxt.text += "REUSSITES CRITIQUES : 0\n";
        if(currentAction == Action.Distance)
            fw.defenseTxt.text += "\n<b>TOTAL : " + totalOppositeDistance + "</b>";
        else if (currentAction == Action.Melee)
            fw.defenseTxt.text += "\n<b>TOTAL : " + currentTarget.successes + "</b>";

        // TOTAL
        if (currentAction == Action.Distance)
            fw.totalTxt.text += (successes - totalOppositeDistance);
        else if (currentAction == Action.Melee)
            fw.totalTxt.text += (successes - currentTarget.successes);
    }

    // Retourne le % lié à la bonne compétence
    int SkillsValueDealer()
    {
        if (currentAction == Action.Melee)
            return skillMelee;
        else if (currentAction == Action.Distance)
            return skillDistance;
        else
            Debug.LogError("On fait appel à cette fonction sans compétence adéquate :" + currentAction);

        return 0;
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

        if (typeOfMovment == 1) // Aller sur le slot (avec pathfind)
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 1.2f)
            {
                if (currentSlot == path[path.Count - 1]) // Si c'est le dernier slot où il devait aller
                {
                    isMovingToSlot = 0;
                    EndMoveCharacter();
                }
                else
                {
                    PozOnSlot();
                    currentSlot = path[path.IndexOf(currentSlot) +1];
                    MainSelector.instance.DisplaySelectedPlayerFeedback(this);
                }
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
                StartAttackMelee();
                return;
                ////////////
            }
            else
            {
                Dealer.instance.LookAtYAxis(transform, currentTarget.currentSlot.transform.position);
                transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
            }
        }
        else if(typeOfMovment == 3 || typeOfMovment == 4) // Revenir au slot d'origine (et termine l'action si 3)
        {
            if (Vector3.Distance(transform.position, currentSlot.transform.position) <= 1.2f)
            {
                isMovingToSlot = 0;
                SetAllBoolAnimFalse();
                PozOnSlot();
                if(typeOfMovment == 3)
                    StartCoroutine(EndAction());
                else
                    Camera.main.GetComponent<CombatCamera>().StopActionMode();
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
    void ShowFeedbackAction(TypeOfFeedback type, Vector3 pozWorldSpace, int value)
    {
        Vector3 poz = pozWorldSpace;
        poz = new Vector3(poz.x, poz.y + 3, poz.z);
        GameObject fb = Instantiate(Dealer.instance.feedbackAction, poz, Quaternion.identity, Dealer.instance.feedbackCanvas);
        fb.GetComponentInChildren<Text>().text = value.ToString();
        Image img = fb.GetComponentInChildren<Image>();

        switch(type)
        {
            case TypeOfFeedback.Successes: //REUSSITES
                img.sprite = Dealer.instance.imageSuccess;
                break;
            case TypeOfFeedback.Damages: //DOMMAGES
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

    // Met à jour la barre de santé
    public void UpdateHealth()
    {
        for (int i = 0; i < healthBar.childCount; i++) // Détruit les lifeSlots précédents
        {
            Destroy(healthBar.GetChild(i).gameObject);
        }

        Dealer.instance.DisplayHealthBar(currentLife, maxLife, healthBar, 0.01f, true); // Spawn les nouveaux
    }

    // Met à jour la barre d'énergie
    public void UpdateEnergy()
    {
        // Détruit l'énergie précédente
        for (int i = 0; i < energyBar.childCount; i++)
        {
            Destroy(energyBar.GetChild(i).gameObject);
        }

        // Instancie les nouveaux symboles énergie
        for (int i = 0; i < maxEnergy; i++)
        {
            Image pe = Instantiate(Dealer.instance.energyPref, Vector3.zero, energyBar.rotation, energyBar).GetComponent<Image>();
            RectTransform pert = pe.GetComponent<RectTransform>();
            pert.sizeDelta = new Vector2(energyBar.rect.height, energyBar.rect.height);
            pert.position = energyBar.position;

            if (isActionEnded) // Si le joueur a terminé son tour
            {
                if (i <= currentEnergy - 1)
                    pe.color = Dealer.instance.protectionStar;
                else
                    pe.color = Dealer.instance.greyedStar;
            }
            else // Si c'est encore son tour
            {
                if (i <= currentEnergy - 1)
                    pe.color = Dealer.instance.activeStar;
                else
                    pe.color = Dealer.instance.greyedStar;
            }
        }
    }

    #region ACTIONS

    // Lance le mouvement automatique
    public void AutoMove(Slot newSlot)
    {
        currentAction = Action.Move;

        if (currentSlot.ReturnPathfinding(newSlot) != null) // S'il y a un chemin, LANCER PATHFINDING
        {
            print(TurnBar.instance.currentChara.charaName);

            path.Clear();
            path = currentSlot.ReturnPathfinding(newSlot);
            SetOneBoolAnimTrue("Run");
            currentSlot.currentChara = null;
            currentSlot = path[0];
            isMovingToSlot = 1;

            MainSelector.instance.DisplaySelectedPlayerFeedback(this);
            ActionBar.instance.UpdateCoverValue(this);
            ActionBar.instance.UpdateActionBar(this, false);
            MainSelector.instance.canClick = false;
        }
        else // Ramène à la normale
        {
            currentAction = Action.Idle;
        }
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
        StartCoroutine(EndAction(false));
        path.Clear();
    }

    // ATTAQUE EN MELEE : Lance le mouvement jusqu'au bord du slot
    public void StartMoveToAttackMelee(Character enemy)
    {
        currentTarget = enemy;
        isMovingToSlot = 2;
        SetOneBoolAnimTrue("Run");
    }

    // ATTAQUE EN MELEE : Crée la fenêtre
    void StartAttackMelee()
    {
        Dealer.instance.LookAtYAxis(transform, currentTarget.transform.position);
        aw = Instantiate(Dealer.instance.actionWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<NewActionWindow>();
        aw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.actionWindow.GetComponent<RectTransform>().anchoredPosition;
        aw.chara = this;
        aw.nameText.text = "Attaque en mêlée";
        aw.skillText.text = skillMelee + " %";

        Camera.main.GetComponent<CombatCamera>().StartActionMode(leftShoulderCamTrans);
        TurnBar.instance.HideAllWorldUIExeptOne(currentTarget);
        currentSlot.HidePossibilities();
        ActionBar.instance.UiActionMode();
    }

    // ATTAQUE EN MELEE : Lance l'attaque l'attaque
    void ExecuteAttackMelee()
    {
        currentTarget.targetedBy = this;
        Destroy(aw.gameObject);
        ChangeCharaAction(0);
        SetOneBoolAnimTrue("Punch");
    }

    // ATTAQUE EN MELEE : Termine l'attaque (après anim)
    public void EndAttackMelee()
    {
        SetAllBoolAnimFalse();

        int totalSuccesses = successes - currentTarget.successes;

        if (totalSuccesses < 0)
            totalSuccesses = 0;

        int totalDamages = totalSuccesses;

        if (currentWeapon.ReturnRank(totalSuccesses, true) != 99)
            totalDamages += currentWeapon.meleeStageArray[currentWeapon.ReturnRank(totalSuccesses, true)].damages;

        if (totalSuccesses > 0 && GiveDamages(currentTarget, totalDamages))
                StartPositionTaking();
        else
            fw.buttonEndTurn.interactable = true;
    }

    // ATTAQUE EN MELEE : Ramène le 
    public void EndAttackMeleeWithoutKill()
    {
        SetOneBoolAnimTrue("Run");
        isMovingToSlot = 3;
    }

    // PRISE DE POSITION : Crée la fenêtre
    public void StartPositionTaking()
    {
        GameObject ptw = Instantiate(Dealer.instance.positionTakingWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas);
        ptw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.positionTakingWindow.GetComponent<RectTransform>().anchoredPosition;
        ptw.GetComponent<PositionTakingWindow>().chara = this;
        MainSelector.instance.canClick = false;
    }

    // PRISE DE POSITION : Lance la prise de position
    public void EndPositionTaking()
    {
        Camera.main.GetComponent<CombatCamera>().StopActionMode();
        StartMoveCharacter(currentTarget.currentSlot);
    }

    // ATTAQUE A DISTANCE : Crée la fenêtre
    public void StartAttackDistance(Character enemy)
    {
        currentTarget = enemy;
        Dealer.instance.LookAtYAxis(transform, currentTarget.transform.position);
        aw = Instantiate(Dealer.instance.actionWindow, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<NewActionWindow>();
        aw.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.actionWindow.GetComponent<RectTransform>().anchoredPosition;
        aw.chara = this;
        aw.nameText.text = "Attaque à distance";
        aw.skillText.text = skillDistance + " %";
        Camera.main.GetComponent<CombatCamera>().StartActionMode(leftShoulderCamTrans);
        TurnBar.instance.HideAllWorldUIExeptOne(currentTarget);
        currentSlot.HidePossibilities();
        HideRangeCircle();
        ActionBar.instance.UiActionMode();
    }

    // ATTAQUE A DISTANCE : Exécute l'attaque
    public void EndAttackDistance()
    {
        HideRangeCircle();
        currentTarget.targetedBy = this;
        Destroy(aw.gameObject);
        //MainSelector.instance.StopAction(false);
        ChangeCharaAction(0);
        SetOneBoolAnimTrue("Shoot");
        int totalSuccesses = successes - currentTarget.successes - currentTarget.currentSlot.coverValue;

        if (totalSuccesses < 0)
            totalSuccesses = 0;

        int totalDamages = totalSuccesses;

        if (currentWeapon.ReturnRank(totalSuccesses, false) != 99)
            totalDamages += currentWeapon.distanceStageArray[currentWeapon.ReturnRank(totalSuccesses, false)].damages;

        GiveDamages(currentTarget, totalDamages);

        //StartCoroutine(EndAction());
    }

    // COURSE : CREE LE COMPTEUR
    void ApplyRun()
    {
        Destroy(aw.gameObject);

        if (successes <= 0)
            EndRun();
        else
        {
            /*counter = Instantiate(Dealer.instance.counter, Vector3.zero, Quaternion.identity, Dealer.instance.mainCanvas).GetComponent<Counter>();
            counter.GetComponent<RectTransform>().anchoredPosition = Dealer.instance.counter.GetComponent<RectTransform>().anchoredPosition;
            counter.chara = this;
            counter.count = successes;
            counter.CreateCounter(Dealer.instance.symbolRun, "Nombre de <b>positions</b> où votre personnage peut encore se déplacer.");*/
        }
    }

    // COURSE : TERMINE LA COURSE
    public void EndRun()
    {
        //StartCoroutine(EndAction());
    }

    // RECHARGER L'ENERGIE
    public void ReloadEnergy()
    {
        currentLife--;
        currentEnergy = maxEnergy;
        UpdateHealth();
        UpdateEnergy();
    }

    // PASSER SON TOUR
    public void PassTurn()
    {
        StartCoroutine(EndAction(false));
    }

    // TERMINER UNE ACTION
    public IEnumerator EndAction(bool wasActionMode = true)
    {
        currentSlot.HidePossibilities();
        currentAction = Action.Idle;

        yield return new WaitForEndOfFrame();

        if (wasActionMode)
        {
            Camera.main.GetComponent<CombatCamera>().StopActionMode();
            ActionBar.instance.UiSelectionMode();
        }
        SetAllBoolAnimFalse();
        ClearSlots();
        successes = 0;
        if(currentTarget)
            currentTarget.successes = 0;
        currentTarget = null;
        MainSelector.instance.HideSelectedCharacterFeedback();
        MainSelector.instance.isActionStarted = false;
        isActionEnded = true;
        UpdateEnergy();
        TurnBar.instance.ChooseAChara(true);
    }

    // Donne la bonne action au chara
    // Si l'index = 0, tous les bools sont false et le personnage sélectionné Clear ses slots
    public void ChangeCharaAction(Action action)
    {
        if (aw != null)
            Destroy(aw.gameObject);

        currentAction = action;

        if (currentAction == Action.Idle)
        {
            ClearSlots();
        }

        ActionBar.instance.UpdateEnergy(this);
    }

    #endregion


}
