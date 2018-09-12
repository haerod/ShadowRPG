using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [HideInInspector] public TooltipText tooltip;
    [HideInInspector] public int count;
    [HideInInspector] public Character chara;
    [HideInInspector] public Image symbol;
    [HideInInspector] public Text txt;

    void Awake ()
    {
        Transform tooltipChild = transform.GetChild(0);
        tooltip = tooltipChild.GetComponent<TooltipText>();
        txt = tooltipChild.GetComponentInChildren<Text>();
        symbol = tooltipChild.GetChild(0).GetComponent<Image>();
	}

    // Crée le compteur
    public void CreateCounter(Sprite newSymbol, string newTooltip)
    {
        symbol.sprite = newSymbol;
        tooltip.textToDisplay = newTooltip;
        UpdateCounter();
    }

    // Décrémente 1
    public void Decrement()
    {
        count--;
        UpdateCounter();
    }

    // Met à jour le texte du compteur
    void UpdateCounter()
    {
        txt.text = count.ToString();
    }

    // Si le joueur clique sur le bouton STOP ACTION
    public void ClickOnStopAction()
    {
        chara.EndRun();
        Destroy(this);
    }
}
