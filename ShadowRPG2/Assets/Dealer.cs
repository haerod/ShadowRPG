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
    public Color activeStar;
    public Color protectionStar;

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

    [Header("Prefabs")]
    public GameObject actionWindow;
    public GameObject feedbackWindow;
    public GameObject positionTakingWindow;
    public GameObject energyPref;
    public GameObject counter;
    public GameObject healthChunk;
    public GameObject weaponStageChunck;
    public GameObject feedbackAction;

    [Header("Player UI Elements")]
    public Transform mainCanvas;
    public Transform feedbackCanvas;
    public GameObject newTurnPanel;

    [Header("Listes et Array de trucs")]
    public Color[] teamColors;
    public Slot[] allSlotArray;
    [SerializeField] Transform characters;
    public Character[] allCharacters;

    [Header("Trucs divers")]
    public Transform selectedCharaFeedback;
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

    // Créer une barre de vie en segments
    public void DisplayHealthBar(int currentHealth, int maxHealth, RectTransform healthBarRectTransform, float sizeBetweenTiles, bool isWolrdUI = false)
    {
        for (int i = 0; i < currentHealth; i++)
        {
            RectTransform instaLife = Instantiate(healthChunk, Vector3.zero, healthBarRectTransform.rotation, healthBarRectTransform).GetComponent<RectTransform>();
            instaLife.sizeDelta = new Vector2( // Donne la taille des lifeSlots
                (healthBarRectTransform.rect.width - (maxHealth * sizeBetweenTiles)) / maxHealth,
                healthBarRectTransform.rect.height - sizeBetweenTiles * 2);

            instaLife.gameObject.name = healthChunk.name + "_" + (i + 1);

            if(isWolrdUI) // Positionne les lifeSlots
                // WORLD UI
            {
                instaLife.localPosition = new Vector3( 
                    (- healthBarRectTransform.rect.width / 2 + sizeBetweenTiles * i + instaLife.rect.width * i + instaLife.rect.width / 2),
                    0f,
                    0f);
            }
            else
                // VIEWPORT UI
            {
                instaLife.position = new Vector2( // Positionne les lifeSlots
                    healthBarRectTransform.position.x - healthBarRectTransform.rect.width / 2 + sizeBetweenTiles * i + instaLife.rect.width * i + instaLife.rect.width / 2,
                    healthBarRectTransform.position.y);
            }
        }
    }
}
