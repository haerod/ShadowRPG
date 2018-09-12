using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaLinkAnim : MonoBehaviour
{
    Character linkedChara;

    void Awake()
    {
        linkedChara = GetComponentInParent<Character>();
    }

    // A la fin de l'anim PUNCH
    public void AnimPunchEnded()
    {
        linkedChara.EndAttackMelee2();
        linkedChara.SetAllBoolAnimFalse();
    }

    // Lorsque l'anim PUNCH ou SHOOT TOUCHE L'ADVERSAIRE
    public void AnimAttack()
    {
        linkedChara.currentTarget.SetOneBoolAnimTrue("Hit");
    }

    // Au début de l'anim HIT
    public void AnimHitStarted()
    {
        linkedChara.SetAllBoolAnimFalse();
    }

    // A la fin de l'anim HIT
    public void ReturnToMainCamMode()
    {
        Camera.main.GetComponent<CombatCamera>().StopCinematicMode();
    }
}
