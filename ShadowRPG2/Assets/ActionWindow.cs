﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWindow : MonoBehaviour
{
    public Character chara;
    public string actionName;

    [Space]

    [SerializeField] Text nameText;
    [SerializeField] Text dicesText;
    [SerializeField] Button buttonRollDices;
    [SerializeField] Button addEnergy;
    [SerializeField] Button removeEnergy;
    [SerializeField] Transform energyBar;

    Image[] energyImagesArray;
    int engagedEnergy;

    // Constructeur
    public void ConstructBar()
    {
        energyImagesArray = new Image[8];
        for (int i = 0; i < energyImagesArray.Length; i++)
        {
            energyImagesArray[i] = energyBar.GetChild(i).GetComponent<Image>();
        }

        nameText.text = actionName;
        dicesText.text = engagedEnergy.ToString();
        if(chara.currentEnergy == 0)
        {
            addEnergy.interactable = false;
        }
        removeEnergy.interactable = false;
        buttonRollDices.interactable = false;
        UpdateEnergy();
        buttonRollDices.onClick.AddListener(delegate { chara.RollSuccesses(engagedEnergy); });
        transform.SetAsFirstSibling();
    }

    // Si le joueur clique sur ADD ENERGY, engage un point d'énergie
    public void ClickOnAddEnergy()
    {
        if(chara.currentEnergy - engagedEnergy > 0)
        {
            engagedEnergy++;
            dicesText.text = engagedEnergy.ToString();
            buttonRollDices.interactable = true;
            UpdateEnergy();
            removeEnergy.interactable = true;
        }
        if (chara.currentEnergy - engagedEnergy <= 0)
            addEnergy.interactable = false;
    }

    // Si le joueur clique sur REMOVE ENERGY, désengage un point d'énergie
    public void ClickOnRemoveEnergy()
    {
        if (engagedEnergy > 0) // Effect
        {
            engagedEnergy--;
            dicesText.text = engagedEnergy.ToString();
            UpdateEnergy();
            addEnergy.interactable = true;
        }
        if (engagedEnergy <= 0) // Disable after effect ?
        {
            removeEnergy.interactable = false;
        }
    }
    
    // Si le joueur clique sur STOP ACTION, ramène le jeu à la normale
    public void ClickStopAction()
    {
        if(chara.currentAction == Character.Action.Melee)
        {
            chara.isMovingToSlot = 4;
            chara.SetOneBoolAnimTrue("Run");
        }
        if (chara.currentAction == Character.Action.Distance)
            chara.HideRangeCircle();
        MainSelector.instance.isActionStarted = false;
        ActionBar.instance.DisplayActionBar(chara, true);
        chara.ChangeCharaAction(0);
        Destroy(this.gameObject);
        chara.linkAnim.ReturnToMainCamMode();
        chara.ChangeCharaAction(Character.Action.Idle);
        Dealer.instance.tooltipUI.gameObject.SetActive(false);
    }

    // Met à jour l'énergie
    void UpdateEnergy()
    {
        for (int i = 0; i < energyImagesArray.Length; i++)
        {
            if (i <= chara.currentEnergy - engagedEnergy - 1)
            {
                energyImagesArray[i].color = Dealer.instance.neutralStar;
            }
            else
            {
                energyImagesArray[i].color = Dealer.instance.greyedStar;
                ;
            }
        }
    }
}
