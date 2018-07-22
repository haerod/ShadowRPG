using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("General")]
    public bool isEnemy;
    public string ID;
    public int currentR;
    public int actionPower;
    public string currentActionName;
    public Character selectedEnemy;
    public int indexList;

    [Header("Energy")]
    public int currentEnergy;
    public int maxEnergy;
    public int currentStage;
    public int minStage;
    public int maxStage;
    public int currentPctR;
    public List<int> pctRList;

    [Header("Environment")]
    public int currentCover;

    [Header("UI")]
    public Text textName;
    public Color maxStageColor;
    public Color minStageColor;

    public Image preparedActionImage;
    public Image slotImage;
    public Text feedbackText;

    void Awake()
    {
        textName.text = ID;
        UpdateNameColor();
        currentPctR = pctRList[currentStage - 1];
        if(!isEnemy)
        {
            HidePreparedAction();
        }
    }

    void Death()
    {
        for (int i = 0; i < transform.childCount; i ++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if (SquadManager.SquadMInstance.selectedCharacter = this)
        {
            SquadManager.SquadMInstance.UnselectCharacter();
        }
    }

    public void StartCharaAction()
    {
        CalucalteR(actionPower);
        ApplyActionEffects();
        ModifyEnergy(actionPower);
        if (!isEnemy)
        {
            SquadManager.SquadMInstance.UpdateStageAndEnergy(this);
        }

        InitializeChara();
    }

    void InitializeChara()
    {
        currentR = 0;
        HidePreparedAction();
        if (selectedEnemy)
        {
            selectedEnemy = null;
        }
        currentActionName = "";
    }

    public void ApplyActionEffects()
    {
        switch (currentActionName)
        {
            case "":
                break;
            case "RangedAttack":
                selectedEnemy.ApplyDamages(currentR);
                break;
            case "ContactAttack":
                selectedEnemy.ApplyDamages(currentR);
                break;
            default:
                Debug.LogError("Ce type d'action n'existe pas : " + currentActionName);
                break;
        }
    }

    public void ModifyEnergy(int modifier)
    {
        currentEnergy -= modifier;
        if (currentEnergy == 0)
        {
            currentStage--;
            if (currentStage <= 0)
            {
                Death();
                currentPctR = 0;
            }
            else
            {
                currentEnergy = maxEnergy;
                currentPctR = pctRList[currentStage - 1];
                UpdateNameColor();
            }
        }
        if(!isEnemy)
        {
            SquadManager.SquadMInstance.UpdateStageAndEnergy(this);
        }
    }

    void CalucalteR(int power)
    {
        for (int i = 0; i< power; i++)
        {
            if(Random.Range(0,101) <= currentPctR)
            {
                currentR++;
            }
        }
    }

    public void ApplyDamages(int damages)
    {
        currentStage -= damages;
        if (currentStage <= 0)
        {
            Death();
            textName.color = Color.black;
        }
        else
        {
            UpdateNameColor();
            StartCoroutine(DisplayFeedbackText(damages.ToString()));
        }
    }


    public IEnumerator DisplayFeedbackText(string textToDisplay)
    {
        feedbackText.text = textToDisplay;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        feedbackText.gameObject.SetActive(false);
    }

    // UI
    //====================================

    public void DisplayPreparedAction()
    {
        preparedActionImage.gameObject.SetActive(true);
    }

    public void HidePreparedAction()
    {
        preparedActionImage.gameObject.SetActive(false);
    }

    public void DisplayCharaSlot()
    {
        slotImage.gameObject.SetActive(true);
    }

    public void HideCharaSlot()
    {
        slotImage.gameObject.SetActive(false);
    }

    public void UpdateNameColor()
    {
        float cur = currentStage;
        float max = maxStage;
        float pctColor = 1 - ((cur + 1) / (max + 1));
        textName.color = Color.Lerp(maxStageColor, minStageColor, pctColor);
    }
}
