using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewActionWindow : MonoBehaviour
{
    public RectTransform energyBar;
    public Text nameText;
    public Text skillText;
    public GameObject selectablePE;

    [HideInInspector] public Character chara;
    [HideInInspector] public int currentEnergy = -1;
    SelectableEnergyPoint[] energyArray;
    int engagedEnergy;

    // Initialisation
    void Start()
    {
        currentEnergy = -1;
        energyArray = new SelectableEnergyPoint[chara.maxEnergy];
        transform.SetAsFirstSibling();
        DisplayEnergyBar();
        energyBar.sizeDelta = new Vector2(
            selectablePE.GetComponent<RectTransform>().rect.width * chara.maxEnergy + chara.maxEnergy * energyBar.GetComponent<HorizontalLayoutGroup>().spacing + 5,
            energyBar.GetComponent<RectTransform>().rect.height);
        UpdateEnergy();
    }

    // MAJ symboles energy bar
    void Update()
    {
        UpdateEnergy();
        //print(currentEnergy);
    }

    void DisplayEnergyBar()
    {
        for (int i = 0; i < chara.maxEnergy; i++)
        {
            SelectableEnergyPoint pe = Instantiate(selectablePE, Vector3.zero, Quaternion.identity, energyBar).GetComponent<SelectableEnergyPoint>();
            pe.aw = this;
            pe.energyIndex = i;
            energyArray[i] = pe;
            if(i > chara.currentEnergy -1)
            {
                Destroy(pe.GetComponent<Button>());
            }
        }
    }

    // Si le joueur clique sur STOP ACTION, ramène le jeu à la normale
    public void ClickStopAction()
    {
        if (chara.currentAction == Character.Action.Melee)
        {
            chara.isMovingToSlot = 4;
            chara.SetOneBoolAnimTrue("Run");
        }
        if (chara.currentAction == Character.Action.Distance)
            chara.HideRangeCircle();
        MainSelector.instance.isActionStarted = false;
        ActionBar.instance.UpdateActionBar(chara, true);
        chara.ChangeCharaAction(0);
        Destroy(this.gameObject);
        Camera.main.GetComponent<CombatCamera>().StopActionMode();
        ActionBar.instance.UiSelectionMode();
        chara.ChangeCharaAction(Character.Action.Idle);
        Dealer.instance.tooltipUI.gameObject.SetActive(false);
    }

    // Met à jour l'énergie
    public void UpdateEnergy()
    {
        for (int i = 0; i < energyArray.Length; i++)
        {
            if(energyArray[i].energyIndex <= currentEnergy && currentEnergy < chara.currentEnergy)
            {
                energyArray[i].SetColor(Dealer.instance.activeStar); // JAUNE
            }
            else if(energyArray[i].energyIndex > currentEnergy && energyArray[i].energyIndex < chara.currentEnergy)
            {
                energyArray[i].SetColor(Dealer.instance.protectionStar); // BLEUE
            }
            else if(energyArray[i].energyIndex > currentEnergy && energyArray[i].energyIndex > chara.currentEnergy - 1)
            {
                energyArray[i].SetColor(Dealer.instance.greyedStar); // GRISE
            }
            else
            {
                //print(i);
            }
        }
    }

    public void DestroyItself()
    {
        Destroy(this.gameObject);
    }
}
