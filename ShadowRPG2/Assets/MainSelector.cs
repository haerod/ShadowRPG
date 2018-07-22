using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSelector : MonoBehaviour
{
    public bool canClick = true;

    [HideInInspector]
    public static MainSelector instance;

    [HideInInspector]
    public Character selectedCharacter;
    private List<RectTransform> energyList = new List<RectTransform>();

    private bool isActionStarted;
    private bool isShortcutAction;
    private int layerToIgnore = ~(1 << 8); // Layer du décor


    void Awake()
    {
        if (!instance)
            instance = this;

    }

    void Update()
    {
        if(canClick)
        {
            CheckClick();
            if (isShortcutAction)
                CheckShortcutsAction();
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
                //Clique sur un perso (sans barre d'action)
                if (hit.transform.GetComponent<Character>() && (selectedCharacter == null || selectedCharacter.currentAction == "Idle"))
                {
                    Character perso = hit.transform.GetComponent<Character>();
                    // PJ et en vie
                    if (perso.team == 0 && !perso.isDead)
                    {
                        HideActionBar();
                        selectedCharacter = perso;
                        DisplayActionBar();
                        return;
                    }
                }
                //(IDEM) Clique sur le slot d'un perso (sans barre d'action)
                else if (hit.transform.tag == "Slot" && (selectedCharacter == null || selectedCharacter.currentAction == "Idle"))
                {
                    if (hit.transform.GetComponent<Slot>().currentChara != null)
                    {
                        Character perso = hit.transform.GetComponent<Slot>().currentChara;
                        // PJ et en vie
                        if (perso.team == 0 && !perso.isDead)
                        {
                            HideActionBar();
                            selectedCharacter = perso;
                            DisplayActionBar();
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
                        HideActionBar();
                        UpdateAndDisplaySelectedPlayerFeedback();
                        UpdateCoverValue();
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
                        HideActionBar();
                        isActionStarted = true;
                        return;
                        //////////////
                    }

                    if (selectedCharacter.currentAction == "Distance")
                    {
                        selectedCharacter.StartAttackDistance(tempSlot.currentChara);
                        HideActionBar();
                        isActionStarted = true;
                        return;
                        //////////////
                    }
                }

                // Enlève l'UI en cliquant dans le vide
                else if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && selectedCharacter)
                {
                    HideActionBar();
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




    // Check les raccourics clavier - ACTION BAR
    void CheckShortcutsAction()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ClickOnMovement();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ClickOnMelee();
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





    //Affiche la barre d'actions
    void DisplayActionBar()
    {
        Dealer.instance.actionBarAnim.gameObject.SetActive(true);
        Dealer.instance.actionBarAnim.SetBool("IsOpen", true);
        Dealer.instance.panelActions.SetActive(true);
        Dealer.instance.charaName.text = selectedCharacter.charaName;
        Dealer.instance.panelStats.SetActive(true);
        UpdateArmorValue();
        UpdateCoverValue();
        selectedCharacter.UpdateStage();
        isShortcutAction = true;
        GenerateEnergy();
        UpdateAndDisplaySelectedPlayerFeedback();
    }

    //Masque la barre d'actions
    void HideActionBar()
    {
        if(Dealer.instance.actionBarAnim.gameObject.activeInHierarchy)
            Dealer.instance.actionBarAnim.SetBool("IsOpen", false);
        Dealer.instance.panelActions.SetActive(false);
        Dealer.instance.panelStats.SetActive(false);
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(false);
        isShortcutAction = false;
        HideEnergy();
    }

    // Met à jour la position et fait apparaitre le feedback montrant quel PJ a été sélectionné
    public void UpdateAndDisplaySelectedPlayerFeedback()
    {
        Dealer.instance.selectedCharaFeedback.position = selectedCharacter.currentSlot.transform.position;
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(true);
    }

    // Met à jour le text de valeur d'armure du selected character
    void UpdateArmorValue()
    {
        Dealer.instance.armorValue.text = selectedCharacter.armor.ToString();
    }

    // Met à jour le text de valeur de couverture du selected character
    void UpdateCoverValue()
    {
        Dealer.instance.coverValue.text = selectedCharacter.currentSlot.coverValue.ToString();
    }


    // Donne la bonne action au chara
    // Si l'index = 0, tous les bools sont false et le personnage sélectionné Clear ses slots
    public void ChangeCharaAction(int boolIndex)
    {

        //selectedCharacter.DistibuteEnergy();
        HideEnergy();

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

        GenerateEnergy();
    }

    // Instancie les symboles Energie dans l'action bar
    public void GenerateEnergy()
    {
        RectTransform prefEnergy = Dealer.instance.energyPref.GetComponent<RectTransform>();
        RectTransform pozEnergyBar = Dealer.instance.energyBar.GetComponent<RectTransform>();
        int sideOffset = 20;
        int betweenOffset = 5;

        for (int i = 0; i < selectedCharacter.currentEnergy; i++)
        {
            Vector2 pozEnergy = new Vector2(
                pozEnergyBar.position.x - (pozEnergyBar.rect.width/2) + sideOffset + (prefEnergy.rect.width* i) + (betweenOffset * i),
                pozEnergyBar.position.y);
            RectTransform instaEnergyRt = Instantiate(Dealer.instance.energyPref, pozEnergy, Quaternion.identity, Dealer.instance.energyBar).GetComponent<RectTransform>();
            energyList.Add(instaEnergyRt);
        }
    }

    // Cache les symboles Energie dans l'action bar
    void HideEnergy()
    {
        foreach(RectTransform but in energyList)
        {
            Destroy(but.gameObject);
        }
        energyList.Clear();
    }

    //Arrête l'action lancée et retourne à la barre d'action (optionnel, if true)
    public void StopAction(bool displayActionBar = true)
    {
        isActionStarted = false;
        if(displayActionBar)
            DisplayActionBar();
    }

}
