using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTakingWindow : MonoBehaviour
{
    public Character chara;

    void Start()
    {
        //StartCoroutine(StopPunchAction());
    }

    IEnumerator StopPunchAction()
    {
        yield return new WaitForEndOfFrame();
        chara.SetAllBoolAnimFalse();
    }

    public void ClickOnTakePosition()
    {
        chara.EndPositionTaking();
        MainSelector.instance.canClick = true;
        Destroy(chara.fw.gameObject);
        Destroy(gameObject);
    }

    public void ClickOnStayBack()
    {
        Camera.main.GetComponent<CombatCamera>().StopActionMode();
        MainSelector.instance.canClick = true;
        chara.SetOneBoolAnimTrue("Run");
        chara.isMovingToSlot = 3;
        Destroy(chara.fw.gameObject);
        Destroy(gameObject);
    }
}
