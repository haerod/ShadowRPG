using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTakingWindow : MonoBehaviour
{
    public Character linkedChara;

    void Start()
    {
        //StartCoroutine(StopPunchAction());
    }

    IEnumerator StopPunchAction()
    {
        yield return new WaitForEndOfFrame();
        linkedChara.SetAllBoolAnimFalse();
    }

    public void ClickOnTakePosition()
    {
        linkedChara.EndPositionTaking();
        MainSelector.instance.canClick = true;
        Destroy(this.gameObject);
    }

    public void ClickOnStayBack()
    {
        MainSelector.instance.canClick = true;
        linkedChara.SetOneBoolAnimTrue("Run");
        linkedChara.isMovingToSlot = 3;
        Destroy(this.gameObject);
    }
}
