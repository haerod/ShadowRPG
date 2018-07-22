using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadManager : MonoBehaviour
{

    [HideInInspector]
    public static SquadManager SquadMInstance;

    [Header("Divers")]
    public bool canClick;
    public List<Character> charaList;
    public List<Character> enemyList;

    [Header("Characters")]
    public Character selectedCharacter;
    public Character selectedEnemy;

    [Header("UI")]
        [Header("   Chara Info")]
    public GameObject panelCharaInfo;
    public Text nameText;
    public Image nameImage;
    public Text energyText;
    public Image energyImage;
    public Text stageText;
    public Image stageImage;
    public Text pctRText;
    public Sprite enemyBg;
    public Sprite characterBg;

    private RectTransform rtPanelCharaInfo;

        [Header("   Actions")]
    public GameObject panelActions;
    public Button buttonRangedAttack;
    public Button buttonContactAttack;

        [Header("   Power")]
    public GameObject panelPower;
    public Slider sliderPower;
    public Text powerText;
    public Image selectedABgImage;
    public Image selectedAMainImage;

    private Color colorSelectedAction;
    private Sprite spriteSelectedAction;

        [Header("   Others")]
    public Button buttonStartTurn;

    [Header("Raycasting")]
    private Ray ray;
    private RaycastHit hit;

    public Button newStageButton;

    // INITIALIZATION / UPDATES
    #region InitializationUpdates
    void Awake ()
    {
        SquadMInstance = this;
        rtPanelCharaInfo = panelCharaInfo.GetComponent<RectTransform>();

    }

    void Start()
    {
        sliderPower.onValueChanged.AddListener(delegate { UpdateSliderPowerValue(); });
        buttonRangedAttack.onClick.AddListener(delegate { ClickOnRangedAttack(buttonRangedAttack.image.color, buttonRangedAttack.transform.GetChild(0).GetComponent<Image>().sprite); });
        buttonContactAttack.onClick.AddListener(delegate { ClickOnContactAttack(buttonContactAttack.image.color, buttonContactAttack.transform.GetChild(0).GetComponent<Image>().sprite); });
        StartCoroutine(CheckOverCharacter());
    }

    void Update ()
    {
		if(canClick)
        {
            CheckInputs();
        }
    }

    IEnumerator CheckOverCharacter()
    {
        yield return new WaitForSeconds(0.1f);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponentInParent<Character>())
            {
                Character chara = hit.transform.GetComponentInParent<Character>();
                if (chara != selectedCharacter)
                {
                    DisplayCharaInfos(chara, false);
                }
            }
            else
            {
                CheckIfIsntCharacterForAutoInfo();
            }
        }
        else
        {
            CheckIfIsntCharacterForAutoInfo();
        }
        StartCoroutine(CheckOverCharacter());
    }

    #endregion

    // DISPLAY / HIDE
    #region DisplayHide

    void DisplayCharaInfos(Character chara, bool isClicked = true)
    {
        if (chara.isEnemy)
        {
            nameImage.sprite = enemyBg;
            energyImage.sprite = enemyBg;
            stageImage.sprite = enemyBg;
        }
        else
        {
            nameImage.sprite = characterBg;
            energyImage.sprite = characterBg;
            stageImage.sprite = characterBg;
        }

        if(!isClicked)
        {
            newStageButton.gameObject.SetActive(false);
            rtPanelCharaInfo.sizeDelta = new Vector2(rtPanelCharaInfo.rect.width, 171f);
        }
        else
        {
            newStageButton.gameObject.SetActive(true);
            rtPanelCharaInfo.sizeDelta = new Vector2(rtPanelCharaInfo.rect.width, 265.5f);
        }
        nameText.text = chara.ID;
        UpdateStageAndEnergy(chara);
        panelCharaInfo.SetActive(true);
    }

    public void HideCharaInfos()
    {
        panelCharaInfo.SetActive(false);
    }

    void DisplayActions()
    {
        UpdatePanelPozOnGameObject(panelActions, selectedEnemy.transform.position);
        panelActions.SetActive(true);
    }

    void HideActions()
    {
        panelActions.SetActive(false);
    }

    void DisplayPower()
    {
        panelPower.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(selectedEnemy.transform.position);
        selectedCharacter.actionPower = 1;
        sliderPower.minValue = selectedCharacter.actionPower;
        sliderPower.maxValue = selectedCharacter.currentEnergy;
        sliderPower.value = selectedCharacter.actionPower;
        powerText.text = selectedCharacter.actionPower.ToString();
        panelPower.GetComponent<Image>().color = colorSelectedAction;
        selectedAMainImage.sprite = spriteSelectedAction;
        panelPower.SetActive(true);
    }

    void HidePower()
    {
        panelPower.SetActive(false);
    }
    #endregion

    // BUTTONS
    #region Buttons

    public void ClickOnNewStage()
    {
        selectedCharacter.ModifyEnergy(selectedCharacter.currentEnergy);
        if(selectedCharacter)
        {
            sliderPower.maxValue = selectedCharacter.currentEnergy;
            sliderPower.value = selectedCharacter.actionPower;
        }
    }

    public void ClickOnRangedAttack(Color color, Sprite sprite)
    {
        selectedCharacter.currentActionName = "RangedAttack";
        colorSelectedAction = color;
        spriteSelectedAction = sprite;
        HideActions();
        DisplayPower();
    }

    public void ClickOnContactAttack(Color color, Sprite sprite)
    {
        selectedCharacter.currentActionName = "ContactAttack";
        colorSelectedAction = color;
        spriteSelectedAction = sprite;
        HideActions();
        DisplayPower();
    }

    public void ClickOnPowerOk()
    {
        panelPower.SetActive(false);

        selectedCharacter.preparedActionImage.color = colorSelectedAction;
        selectedCharacter.preparedActionImage.transform.GetChild(0).GetComponent<Image>().sprite = spriteSelectedAction;
        selectedCharacter.DisplayPreparedAction();
        HideCharaInfos();
        selectedEnemy.HideCharaSlot();
    }

    public void ClickOnStartTurn()
    {
        StartCoroutine(StartTurn());
    }
    #endregion

    // OTHERS
    #region Others

    IEnumerator StartTurn()
    {
        buttonStartTurn.interactable = false;
        canClick = false;
        HideCharaInfos();
        HideActions();
        HidePower();

        if (charaList.Count > 0)
        {
            foreach (Character chara in charaList)
            {
                chara.StartCharaAction();
                chara.HideCharaSlot();
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (enemyList.Count > 0)
        {
            foreach (Character chara in enemyList)
            {
                chara.StartCharaAction();
                chara.HideCharaSlot();
                yield return new WaitForSeconds(0.5f);
            }

            foreach (Character chara in enemyList)
            {
                chara.preparedActionImage.gameObject.SetActive(true);
            }
        }

        buttonStartTurn.interactable = true;
        canClick = true;
    }

    void UpdateSliderPowerValue()
    {
        powerText.text = sliderPower.value.ToString();
        selectedCharacter.actionPower = Mathf.RoundToInt(sliderPower.value);
        //energyText.text = (selectedCharacter.currentEnergy - sliderPower.value) + " / " + selectedCharacter.maxEnergy;
    }

    public void UpdateStageAndEnergy(Character chara)
    {
        energyText.text = chara.currentEnergy + " / " + chara.maxEnergy;
        stageText.text = chara.currentStage + " / " + chara.maxStage;
        pctRText.text = chara.currentPctR + " %";
    }

    void SelectCharacter(Character chara)
    {
        if (selectedCharacter)
        {
            Character previousChara = selectedCharacter;
            if (previousChara != chara)
            {
                HideActions();
                HidePower();
                if (previousChara.selectedEnemy)
                {
                    previousChara.selectedEnemy.HideCharaSlot();
                }
            }
        }

        if (chara.selectedEnemy)
        {
            chara.selectedEnemy.DisplayCharaSlot();
        }

        selectedCharacter = chara;
        DisplayCharaInfos(selectedCharacter);
    }

    void SelectEnemy(Character enemy)
    {
        if (selectedCharacter.selectedEnemy)
        {
            if (selectedCharacter.selectedEnemy != enemy)
            {
                selectedCharacter.selectedEnemy.HideCharaSlot();
            }
        }
        enemy.DisplayCharaSlot();
        selectedEnemy = enemy;
        selectedCharacter.selectedEnemy = enemy;
        DisplayActions();
        HidePower();
    }

    public void UnselectCharacter()
    {
        HideCharaInfos();
        selectedCharacter = null;
    }

    void CheckIfIsntCharacterForAutoInfo()
    {
        if (selectedCharacter)
        {
            DisplayCharaInfos(selectedCharacter);
        }
        else
        {
            HideCharaInfos();
        }
    }

    public void UpdatePanelPozOnGameObject(GameObject panel, Vector3 position)
    {
        panel.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(position);
    }

    void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckRaycastTag() == "Player")
            {
                SelectCharacter(hit.collider.transform.parent.gameObject.GetComponent<Character>());
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (selectedCharacter)
            {
                if (CheckRaycastTag() == "Enemy")
                {
                    SelectEnemy(hit.collider.transform.parent.gameObject.GetComponent<Character>());
                }
            }
        }
    }

    string CheckRaycastTag()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Player")
            {
                return hit.collider.tag;
            }
        }
        return null;
    }
    #endregion
}
