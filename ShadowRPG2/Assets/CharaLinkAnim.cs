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

    public void AnimPunchEnded()
    {
        linkedChara.EndAttackMelee2();
        linkedChara.SetAllBoolAnimFalse();
    }

    // Lorsque l'anim Punch ou Shoot au fusil TOUCHE L'ADVERSAIRE
    public void AnimAttack()
    {
        linkedChara.currentTarget.SetOneBoolAnimTrue("Hit");
    }

    public void AnimHitStarted()
    {
        linkedChara.SetAllBoolAnimFalse();
    }
}
