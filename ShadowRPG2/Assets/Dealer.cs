using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dealer : MonoBehaviour
{
    [HideInInspector]
    public static Dealer instance;

    [Header("Materials")]
    public Material connectedSlotsLR;

    [Header("Materials")]
    public Color neutralSlot;
    public Color freeSlot;
    public Color forbiddenSlot;
    public Color attackableSlot;

    [Header("Action bar")]
    public Animator actionBarAnim;
    public GameObject panelStats;
    [Space]
    public GameObject panelActions;
        public Button buttonReloadEnergy;
    [Space]
    public Text charaName;
    public Text stageDesription;
    public Text stageDescriptionTextual;
    public Image[] stageImagesArray;
    public Sprite spriteVoidState;
    public Sprite spriteCurrentState;
    public Text armorValue;
    public Text coverValue;
    [Space]
    public Text tooltipText;
    public float tooltipHorizontalOffset;
    public float tooltipHVerticalOffset;
    [Space]
    public GameObject energyPref;
    public Transform energyBar;

    [Header("Turn bar")]
    public RectTransform turnBar;
    public GameObject charaTurn;

    [Header("Various objects")]
    public Transform mainCanvas;
    public GameObject attackWindow;
    public GameObject positionTakingWindow;

    [Header("Listes et Array de trucs")]
    public Slot[] allSlotArray;
    [SerializeField] Transform characters;
    public Character[] allCharacters;

    [Header("Feedbacks divers")]
    public Transform selectedCharaFeedback;
    public GameObject feedbackAction;
    public Sprite imageSuccess;
    public Sprite imageDamages;

    void Awake()
    {
        if(!instance)
            instance = this;
        else
            Debug.LogError("2 dealers !!!!!");

        allSlotArray = (Slot[])FindObjectsOfType(typeof(Slot));
        allCharacters = new Character[characters.childCount];

        for (int i = 0; i < characters.childCount; i++)
        {
            allCharacters[i] = characters.GetChild(i).GetComponent<Character>();
        }
    }

    // Oriente le Transform sur l'axe Yaw (Y)
    public void LookAtYAxis(Transform thisTrans, Vector3 targetPoz)
    {
        thisTrans.LookAt(new Vector3(targetPoz.x,
                                       thisTrans.position.y,
                                       targetPoz.z));
    }

    // Change la valeur Y d'un Vector3;
    public Vector3 SetVectorY (Vector3 vectorToModify, float y)
    {
        vectorToModify = new Vector3(
            vectorToModify.x,
            y,
            vectorToModify.z);
        return vectorToModify;
    }
}
