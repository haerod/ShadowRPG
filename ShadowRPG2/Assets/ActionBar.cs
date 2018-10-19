using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] GameObject panelActions; 
    [SerializeField] GameObject panelStats;
    [SerializeField] GameObject panelPassAndReload;
    [SerializeField] GameObject panelWeapon;
    [SerializeField] GameObject panelDefenses;

    [Space]

    [SerializeField] Text charaNameTxt;
    [SerializeField] Text armorValueTxt; 
    [SerializeField] Text coverValueTxt; 
    [SerializeField] Text healthText;
    [SerializeField] Image weaponImage;
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

    #endregion



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



    //Met à jour la barre d'actions
    public void UpdateActionBar(Character chara, bool displayActions)
    {
        panelStats.SetActive(true);
        panelDefenses.SetActive(true);

        if (displayActions) // Si demandé, affiche les actions et les actions sous la Chara Tile principale
        {
            panelActions.SetActive(true);
            panelWeapon.SetActive(true);
            panelPassAndReload.SetActive(true);
            for (int i = 0; i < buttonsAction.Length; i++)
            {
                buttonsAction[i].interactable = !chara.isActionEnded;
            }
        }
        else
        {
            panelActions.SetActive(false);
            panelWeapon.SetActive(false);
            panelPassAndReload.SetActive(false);
        }

        charaNameTxt.text = chara.charaName;
        UpdateArmorValue(chara);
        UpdateCoverValue(chara);
        UpdateEnergy(chara);
        UpdateHealth(chara);
        UpdateSkillsValue(chara); // Met à jour les % skill
        UpdateWeaponImage(chara);
        MainSelector.instance.DisplaySelectedPlayerFeedback(chara);
    }

    //Masque la barre d'actions
    public void HideActionBar()
    {
        panelActions.SetActive(false);
        panelStats.SetActive(false);
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(false);
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

    // Lance l'UI pour l'Action mode
    public void UiActionMode()
    {
        panelActions.SetActive(false);
        panelStats.SetActive(false);
    }

    // Lance l'UI pour le mode Sélection (mode de base)
    public void UiSelectionMode()
    {
        panelActions.SetActive(true);
        panelStats.SetActive(true);
    }

    // Met à jour la santé du personnage et bloque l'action reload
    public void UpdateHealth(Character chara)
    {
        for (int i = 0; i < healthBarRt.transform.childCount; i++)
        {
            Destroy(healthBarRt.transform.GetChild(i).gameObject);
        }

        if (chara.currentLife > 1 && chara.currentEnergy != chara.maxEnergy)
            buttonReloadEnergy.interactable = true;
        else
            buttonReloadEnergy.interactable = false;

        Dealer.instance.DisplayHealthBar(chara.currentLife, chara.maxLife, healthBarRt, 2);

        healthText.text = chara.currentLife.ToString();
        chara.UpdateHealth();
    }

    // Met à jour les PE du personnage
    public void UpdateEnergy(Character chara)
    {
        // Destroy energy
        for (int i = 0; i < energyImagesList.Count; i++)
        {
            Destroy(energyImagesList[i].gameObject);
        }
        energyImagesList.Clear();

        // Generate energy
        int maxAngle = 180;
        int radius = 60;
        int currentAngle;

        for (int i = 0; i < chara.maxEnergy; i++)
        {
            currentAngle = maxAngle / (chara.maxEnergy + 1); // Calcule la taille d'une portion d'angle
            currentAngle = currentAngle * (i + 1); // Calcule l'angle lié au PE

            Vector2 pozEnergy = new Vector2( // Calcule les coordonnées du PE;
                radius * Mathf.Cos((currentAngle + 90) * Mathf.Deg2Rad),
                radius * Mathf.Sin((currentAngle + 90) * Mathf.Deg2Rad));

            RectTransform pe = Instantiate(Dealer.instance.energyPref, Vector3.zero, Quaternion.identity, energyBarRt).GetComponent<RectTransform>(); // Crée le PE
            pe.transform.localPosition = pozEnergy; // Assigne la position
            energyImagesList.Add(pe.GetComponent<Image>()); // Ajoute le PE à la liste

            chara.UpdateEnergy();
        }

        // Update symbols
        for (int i = 0; i < energyImagesList.Count; i++)
        {
            if (chara.isActionEnded)
            {
                if (i <= chara.currentEnergy - 1)
                {
                    energyImagesList[energyImagesList.Count - i - 1].color = Dealer.instance.protectionStar;
                    // juste mettre list[i] pour les étoiles dans l'autre sens
                }
                else
                {
                    energyImagesList[energyImagesList.Count - i - 1].color = Dealer.instance.greyedStar;
                }
            }
            else
            {
                if (i <= chara.currentEnergy - 1)
                {
                    energyImagesList[energyImagesList.Count - i - 1].color = Dealer.instance.activeStar;
                    // juste mettre list[i] pour les étoiles dans l'autre sens
                }
                else
                {
                    energyImagesList[energyImagesList.Count - i - 1].color = Dealer.instance.greyedStar;
                }
            }
        }
    }
    
    // Met à jour l'image d'arme du personnage
    void UpdateWeaponImage(Character chara)
    {
        weaponImage.sprite = chara.currentWeapon.illustration;
    }
    
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
            UpdateHealth(TurnBar.instance.currentChara);
            UpdateEnergy(TurnBar.instance.currentChara);
        }
    }

    // Si le joueur clique sur le bouton PASSER SON TOUR
    public void ClickOnPassTurn()
    {
        TurnBar.instance.currentChara.ClearSlots();
        TurnBar.instance.currentChara.PassTurn();
    }

    #endregion
}
