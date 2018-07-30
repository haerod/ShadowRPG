using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField] GameObject panelActions; 
    [SerializeField] GameObject panelStats;

    [Space]

    [SerializeField] Text charaNameTxt;
    [SerializeField] Text armorValueTxt; 
    [SerializeField] Text coverValueTxt; 
    [SerializeField] Text stageDesriptionTxt;
    [SerializeField] Text stageDescriptionTextual;
    [SerializeField] RectTransform energyBarRt;
    [SerializeField] Image[] stageImagesArray;

    [Space]

    [SerializeField] Button buttonReloadEnergy;
    
    [Space]

    [SerializeField] private string[] stageDescription;


    Animator anim;

    List<RectTransform> energyList = new List<RectTransform>();

    [HideInInspector]
    public static ActionBar instance;



    void Awake ()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError("Il existe deux instances de ActionBar !");
            Destroy(this);
        }

        anim = GetComponent<Animator>();
    }

    void Start()
    {
        HideActionBar();
    }




    //Affiche la barre d'actions
    public void DisplayActionBar(Character selectedCharacter, bool displayActions)
    {
        anim.gameObject.SetActive(true);
        anim.SetBool("IsOpen", true);

        panelStats.SetActive(true);
        if(displayActions)
            panelActions.SetActive(true);

        charaNameTxt.text = selectedCharacter.charaName;
        UpdateArmorValue(selectedCharacter);
        UpdateCoverValue(selectedCharacter);
        GenerateEnergy(selectedCharacter);

        UpdateStage(selectedCharacter);
        UpdateAndDisplaySelectedPlayerFeedback(selectedCharacter);
        Camera.main.GetComponent<CombatCamera>().isActionBarOpened = true;
    }

    //Masque la barre d'actions
    public void HideActionBar()
    {
        anim.SetBool("IsOpen", false);

        panelActions.SetActive(false);
        panelStats.SetActive(false);
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(false);
        HideEnergy();
        Camera.main.GetComponent<CombatCamera>().isActionBarOpened = false;
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

    // Met à jour l'etat du personnage et bloque l'action reload
    public void UpdateStage(Character selectedCharacter)
    {
        stageDesriptionTxt.text = selectedCharacter.currentStage + "+";
        stageDescriptionTextual.text = stageDescription[selectedCharacter.currentStage];
        for (int i = 0; i < stageImagesArray.Length; i++)
        {
            if (i != (selectedCharacter.currentStage - 2))
                stageImagesArray[i].sprite = Dealer.instance.spriteVoidState;
            else
                stageImagesArray[i].sprite = Dealer.instance.spriteCurrentState;
        }

        if (selectedCharacter.currentStage < 6 && selectedCharacter.currentEnergy != selectedCharacter.maxEnergy)
            buttonReloadEnergy.interactable = true;
        else
            buttonReloadEnergy.interactable = false;
    }

    // Met à jour la position et fait apparaitre le feedback montrant quel PJ a été sélectionné
    public void UpdateAndDisplaySelectedPlayerFeedback(Character selectedCharacter)
    {
        Dealer.instance.selectedCharaFeedback.position = selectedCharacter.currentSlot.transform.position;
        Dealer.instance.selectedCharaFeedback.gameObject.SetActive(true);
    }




    // Instancie les symboles Energie dans l'action bar
    public void GenerateEnergy(Character selectedCharacter)
    {
        RectTransform prefEnergy = Dealer.instance.energyPref.GetComponent<RectTransform>();
        int sideOffset = 20;
        int betweenOffset = 5;

        for (int i = 0; i < selectedCharacter.currentEnergy; i++)
        {
            Vector2 pozEnergy = new Vector2(
                energyBarRt.position.x - (energyBarRt.rect.width / 2) + sideOffset + (prefEnergy.rect.width * i) + (betweenOffset * i),
                energyBarRt.position.y);
            RectTransform instaEnergyRt = Instantiate(Dealer.instance.energyPref, pozEnergy, Quaternion.identity, energyBarRt.transform).GetComponent<RectTransform>();
            energyList.Add(instaEnergyRt);
        }
    }

    // Cache les symboles Energie dans l'action bar
    public void HideEnergy()
    {
        foreach (RectTransform but in energyList)
        {
            Destroy(but.gameObject);
        }
        energyList.Clear();
    }



}
