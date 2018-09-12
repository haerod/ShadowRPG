﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTextLayered : MonoBehaviour
{
    private RectTransform tooltip;
    private int layerToIgnore = ~(1 << 8); // Layer du décor
    CombatCamera cam;

    void Awake()
    {
        cam = GetComponent<CombatCamera>();
    }

    void Start()
    {
        tooltip = Dealer.instance.tooltipText.transform.parent.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!cam.isCinematicMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, layerToIgnore))
            {
                if (hit.transform.GetComponent<TooltipText>() && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    DisplayTooltip(hit.transform.GetComponent<TooltipText>().textToDisplay, Camera.main.WorldToScreenPoint(hit.transform.position));
                else if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    HideTooltip();
            }
        }
    }

    void DisplayTooltip(string textToDisplay, Vector3 poz)
    {
        Dealer.instance.tooltipText.text = textToDisplay;
        poz = new Vector3(poz.x + 200f, poz.y - 60f, poz.z);
        tooltip.position = poz;
        Dealer.instance.tooltipUI.sizeDelta = new Vector2(
            Dealer.instance.tooltipUI.rect.width,
            Dealer.instance.tooltipText.preferredHeight + 20);
        tooltip.gameObject.SetActive(true);
    }

    void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
