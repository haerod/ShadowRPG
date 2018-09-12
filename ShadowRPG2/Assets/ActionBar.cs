using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField] GameObject panelActions; 
    [SerializeField] GameObject panelStats;
    [SerializeField] GameObject panelPassAndReload;

    [Space]

    [SerializeField] Text charaNameTxt;
    [SerializeField] Text armorValueTxt; 
    [SerializeField] Text coverValueTxt; 
    [SerializeField] Text stageDesriptionTxt;
    [SerializeField] Text healthText;
    public RectTransform energyBarRt;
    [SerializeField] RectTransform healthBarRt;

    [Space]

    [SerializeField] Button buttonReloadEnergy;

    [Space]

    [SerializeField] Button[] buttonsAction; // /!\ Correspond à l'ordre de l'enum Action

    [Space]

    [SerializeField] Sprite fullEnergySprite;
    [SerializeField] Sprite emptyEnergySprite;

    List<Image> energyImagesList = new List<Image>();

    [HideInInspector] public static ActionBar instance;




    void Awake ()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError("Il existe deux instances de ActionBar !");
            Destroy(this);
        }
    }



    //Affiche la barre d'actions
    public void DisplayActionBar(Character selectedCharacter, bool displayActions)
    {
        panelActions.SetActive(false);
        panelStats.SetActive(false);

        panelStats.SetActive(true);
        if (displayActions) // Si demandé, affiche les actions et les actions sous la Chara Tile principale
        {
            panelActions.SetActive(true);
            panelPassAndReload.SetActive(true);
        }
        else
        {
            HideUndertileActions();
        }

        charaNameTxt.text = selectedCharacter.charaName;
        UpdateArmorValue(selectedCharacter);
        UpdateCoverValue(selectedCharacter);
        GenerateEnergy(selectedCharacter);
        DestroyHealth(); // Enlève les symboles life
        UpdateHealth(selectedCharacter);
        UpdateSkillsValue(selectedCharacter); // Met à jour les % skill
        MainSelector.instance.DisplaySelectedPlayerFeedback(selectedCharacter);
    }

    //Masque la barre d'actions
    public void HideActionBar()
    {
        panelActions.SetActive(false);
        panelStats.SetActive(false);
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(false);
        DestroyEnergy();
    }

    // Met à jour le text de % de compétence d'un chara
    void UpdateSkillsValue(Character chara)
    {
        for (int i = 0; i < buttonsAction.Length; i++)
        {
            Text textToModify = buttonsAction[i].GetComponentInChildren<Text>();
            switch (i)
            {
                case 0:
                    textToModify.text = chara.skillMelee.ToString();
                    break;
                case 1:
                    textToModify.text = chara.skillDistance.ToString();
                    break;
                default:
                    Debug.LogError("Le programme tente de mettre une valeur dans un bouton qui ne correspond pas : " + buttonsAction[i].name);
                    break;
            }

            textToModify.text += " %";
        }
    }

    // Met à jour le text de valeur d'armure du selected character
    void UpdateArmorValue(Character selectedCharacter)
    {
        armorValueTxt.text = selectedCharacter.armor.ToString();
    }

    // Met à jour le text de valeur de couverture du selected character
    public void UpdateCoverValue(Character selectedCharacter)
    {
        coverValueTxt.text = selectedCharacter.currentSlot.coverValue.ToString();
    }
    
    // Masque les actions situées sous la Chara Tile (terminer le tour, recharger énergie)
    public void HideUndertileActions()
    {
        panelPassAndReload.SetActive(false);
    }

    // SANTE
    // Met à jour l'etat du personnage et bloque l'action reload
    public void UpdateHealth(Character chara)
    {
        DestroyHealth();

        if (chara.currentLife > 1 && chara.currentEnergy != chara.maxEnergy)
            buttonReloadEnergy.interactable = true;
        else
            buttonReloadEnergy.interactable = false;

        int sizeBetweenTiles = 2;

        for (int i = 0; i < chara.currentLife; i++)
        {
            RectTransform instaLife = Instantiate(Dealer.instance.lifeSlot, Vector3.zero, Quaternion.identity, healthBarRt).GetComponent<RectTransform>();
            instaLife.sizeDelta = new Vector2( // Donne la taille des lifeSlots
                (healthBarRt.rect.width - (chara.maxLife * sizeBetweenTiles)) / chara.maxLife,
                healthBarRt.rect.height - sizeBetweenTiles * 2);

            instaLife.gameObject.name = Dealer.instance.lifeSlot.name + "_" + (i+1);

            instaLife.position = new Vector2( // Positionne les lifeSlots
                healthBarRt.position.x - healthBarRt.rect.width / 2 + sizeBetweenTiles * i + instaLife.rect.width * i + instaLife.rect.width / 2,
                healthBarRt.position.y);
        }

        healthText.text = chara.currentLife.ToString();
    }

    //Détruit les symboles énergie dans l'action bar
    public void DestroyHealth()
    {
        for (int i = 0; i < healthBarRt.transform.childCount; i++)
        {
            Destroy(healthBarRt.transform.GetChild(i).gameObject);
        }
    }
    //=========

    // ENERGY
    //Crée les PE dans l'action bar
    void GenerateEnergy(Character chara)
    {
        DestroyEnergy();

        int maxAngle = 180;
        int radius = 60;
        int currentAngle;

        for (int i = 0; i < chara.maxEnergy; i++)
        {
            currentAngle = maxAngle / (chara.maxEnergy + 1); // Calcule la taille d'une portion d'angle
            currentAngle = currentAngle * (i + 1); // Calcule l'angle lié au PE

            Vector2 pozEnergy = new Vector2( // Calcule les coordonnées du PE;
                radius * Mathf.Cos((currentAngle+90) * Mathf.Deg2Rad),
                radius * Mathf.Sin((currentAngle+90) * Mathf.Deg2Rad));

            RectTransform pe = Instantiate(Dealer.instance.energyPref, Vector3.zero, Quaternion.identity, energyBarRt).GetComponent<RectTransform>(); // Crée le PE
            pe.transform.localPosition = pozEnergy; // Assigne la position
            energyImagesList.Add(pe.GetComponent<Image>()); // Ajoute le PE à la liste
        }

        UpdateEnergy(chara);
    }

    // Met à jour les symboles énergie (plein / vide)
    public void UpdateEnergy(Character selectedCharacter)
    {
        for (int i = 0; i < energyImagesList.Count; i++)
        { 
            if(i <= selectedCharacter.currentEnergy - 1)
            {
                energyImagesList[i].color = Dealer.instance.neutralStar;
            }
            else
            {
                energyImagesList[i].color = Dealer.instance.greyedStar;
            }
        }
    }

    // Cache les symboles Energie dans l'action bar
    public void DestroyEnergy()
    {
        for (int i = 0; i < energyImagesList.Count; i++)
        {
            Destroy(energyImagesList[i].gameObject);
        }
        energyImagesList.Clear();
    }
    //=========


    #region ACTIONS

    // Si le joueur clique sur le bouton MOUVEMENT
    public void ClickOnMovement()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.currentSlot.ShowMovementPossibilities();
        TurnBar.instance.currentChara.ChangeCharaAction(Character.Action.Move);
    }

    // Si le joueur clique sur le bouton MELEE
    public void ClickOnMelee()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.currentSlot.ShowMeleePossibilities();
        TurnBar.instance.currentChara.ChangeCharaAction(Character.Action.Melee);
    }

    // Si le joueur clique sur le bouton DISTANCE
    public void ClickOnDistance()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.currentSlot.ShowDistancePossibilities();
        TurnBar.instance.currentChara.DisplayRangeCircle();
        TurnBar.instance.currentChara.ChangeCharaAction(Character.Action.Distance);
    }

    // Si le joueur clique sur le bouton RECHARGER ENERGIE
    public void ClickOnReloadState()
    {
        if (TurnBar.instance.currentChara.currentLife > 1)
        {
            TurnBar.instance.currentChara.ReloadEnergy();
        }
    }

    // Si le joueur clique sur le bouton PASSER SON TOUR
    public void ClickOnPassTurn()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.PassTurn();
    }

    // Si le joueur clique sur le bouton RUN
    public void ClickOnRun()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.currentSlot.ShowRunPossibilities();
        MainSelector.instance.isActionStarted = true;
    }

    #endregion
}
