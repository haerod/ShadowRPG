using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackWindow : MonoBehaviour
{
    [HideInInspector] public Character chara;

    public Text attackTxt;
    public Text defenseTxt;
    public Text totalTxt;
    public Button buttonEndTurn;
    
    void Awake ()
    {
        attackTxt.text = "<size=15><b>ATTAQUANT</b></size>\n\n";
        defenseTxt.text = "<size=15><b>DEFENSEUR</b></size>\n\n";
        totalTxt.text = "";
    }

    public void ClickOnEndTurn()
    {
        if (chara.currentAction == Character.Action.Melee)
        {
            Camera.main.GetComponent<CombatCamera>().StopActionMode();
            chara.isMovingToSlot = 3;
        }
        else
        {
            chara.StartCoroutine(chara.EndAction());
        }
        Destroy(this.gameObject);
    }
}
