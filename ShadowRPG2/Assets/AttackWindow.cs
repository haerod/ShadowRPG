using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackWindow : MonoBehaviour
{
    public Character chara;
    public string attackName;

    private int engagedDices;

    [Space]

    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text dicesText;
    [SerializeField]
    private Button buttonRollDices;
    [SerializeField]
    private Button addDices;
    [SerializeField]
    private RectTransform diceBar;

    private List<GameObject> diceList = new List<GameObject>();

    // Constructeur
    public void ConstructBar()
    {
        nameText.text = attackName;
        dicesText.text = engagedDices.ToString();
        if(chara.currentEnergy == 0)
        {
            addDices.interactable = false;
        }
        buttonRollDices.interactable = false;
        GenerateEnergy();
        buttonRollDices.onClick.AddListener(delegate { chara.RollDices(engagedDices); });
    }

    // Si le joueur clique sur ADD DICES, ajoute un dé
    public void ClickOnAddDice()
    {
        if(chara.currentEnergy - engagedDices > 0)
        {
            engagedDices++;
            dicesText.text = engagedDices.ToString();
            buttonRollDices.interactable = true;
            HideEnergy();
            GenerateEnergy();
        }
        if (chara.currentEnergy - engagedDices <= 0)
            addDices.interactable = false;
    }

    // Si le joueur clique sur STOP ACTION, ramène le jeu à la normale
    public void ClickStopAction()
    {
        if(chara.currentAction == "Melee")
        {
            chara.isMovingToSlot = 3;
            chara.SetOneBoolAnimTrue("Run");
        }
        if (chara.currentAction == "Distance")
            chara.HideRangeCircle();
        MainSelector.instance.StopAction();
        MainSelector.instance.ChangeCharaAction(0);
        Camera.main.GetComponent<CombatCamera>().BackToFreeMode();
        Destroy(this.gameObject);
    }

    

    // Instancie les images Energie dans l'action bar
    void GenerateEnergy()
    {
        RectTransform prefEnergy = Dealer.instance.energyPref.GetComponent<RectTransform>();
        int sideOffset = 20;
        int betweenOffset = 5;

        for (int i = 0; i < (chara.currentEnergy-engagedDices); i++)
        {
            Vector2 pozEnergy = new Vector2(
                diceBar.position.x - (diceBar.rect.width / 2) + sideOffset + (prefEnergy.rect.width * i) + (betweenOffset * i),
                diceBar.position.y);
            RectTransform instaEnergyRt = Instantiate(Dealer.instance.energyPref, pozEnergy, Quaternion.identity, diceBar).GetComponent<RectTransform>();
            diceList.Add(instaEnergyRt.gameObject);
        }
    }

    // Détruit les images Energie
    void HideEnergy()
    {
        foreach(GameObject go in diceList)
        {
            Destroy(go);
        }
    }
}
