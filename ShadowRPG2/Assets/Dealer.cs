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

    [Header("Colors")]
    public Color neutralSlot;
    public Color freeSlot;
    public Color forbiddenSlot;
    public Color attackableSlot;
    public Color greyedStar;
    public Color neutralStar;

    [Header("Tooltip text")]
    public Text tooltipText;
    public RectTransform tooltipUI;
    public float tooltipHorizontalOffset;
    public float tooltipHVerticalOffset;

    [Header("Sprites")]
    public Sprite spriteVoidState;
    public Sprite spriteCurrentState;
    public Sprite fullEnergySprite;
    public Sprite emptyEnergySprite;
    [Space]
    public Sprite symbolRun;

    [Header("Various objects")]
    public Transform mainCanvas;
    public Transform feedbackCanvas;
    public GameObject attackWindow;
    public GameObject charaTurn;
    public GameObject positionTakingWindow;
    public GameObject energyPref;
    public GameObject counter;
    public GameObject lifeSlot;

    [Header("Listes et Array de trucs")]
    public Color[] teamColors;
    public Slot[] allSlotArray;
    [SerializeField] Transform characters;
    public Character[] allCharacters;

    [Header("Trucs divers")]
    public Transform selectedCharaFeedback;
    public GameObject feedbackAction;
    public Sprite imageSuccess;
    public Sprite imageDamages;
    public Camera faceCamera;

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

    // Mélange une List d'objects
    public List<T> ShuffleObjectList<T>(List<T> objectListToShuffle)
    {
        List<T> tempList = new List<T>();
        int x = objectListToShuffle.Count;

        for (int i = 0; i < x; i++)
        {
            int index = Random.Range(0, objectListToShuffle.Count);
            tempList.Add(objectListToShuffle[index]);
            objectListToShuffle.RemoveAt(index);
        }
        return tempList;
    }

    // Donne la proportion (entre 0 et 1) d'une valeur située entre deux valeurs
    public float PercentBetweenTwoFloats(float maxValue, float minValue, float currentValue)
    {
        float result = (currentValue - minValue) / (maxValue - minValue);
        return result;
    }
}
