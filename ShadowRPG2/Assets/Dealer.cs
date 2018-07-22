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

    [Header("Various objects")]
    public Transform mainCanvas;
    public GameObject attackWindow;
    public GameObject positionTakingWindow;

    [Header("Listes et Array de trucs")]
    public Slot[] allSlotArray;

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
    }

    void Start()
    {
        allSlotArray = (Slot[]) FindObjectsOfType(typeof(Slot));
    }

    public void LookAtYAxis(Transform thisTrans, Vector3 targetPoz)
    {
        thisTrans.LookAt(new Vector3(targetPoz.x,
                                       thisTrans.position.y,
                                       targetPoz.z));
    }
}
