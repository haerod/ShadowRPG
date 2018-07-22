using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTextLayered : MonoBehaviour
{
    private RectTransform tooltip;
    private float xTooltip;
    private float yTooltip;

    private int layerToIgnore = ~(1 << 8); // Layer du décor

    void Start()
    {
        tooltip = Dealer.instance.tooltipText.transform.parent.gameObject.GetComponent<RectTransform>();
        if (GetComponent<RectTransform>())
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt.position.x < Screen.width / 2)
            {
                xTooltip = rt.position.x + tooltip.rect.width / 2 + rt.rect.width / 2 + Dealer.instance.tooltipHorizontalOffset;
            }
            else
            {
                xTooltip = rt.position.x - tooltip.rect.width / 2 - rt.rect.width / 2 - Dealer.instance.tooltipHorizontalOffset;
            }

            if (rt.position.y > Screen.height / 2)
            {
                yTooltip = rt.position.y - Dealer.instance.tooltipHVerticalOffset;
            }
            else
            {
                yTooltip = rt.position.y + Dealer.instance.tooltipHVerticalOffset;
            }
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, layerToIgnore))
        {
            //Clique sur un perso (sans barre d'action)
            if (hit.transform.GetComponent<TooltipText>())
            {
                DisplayTooltip(hit.transform.GetComponent<TooltipText>().textToDisplay, Camera.main.WorldToScreenPoint(hit.transform.position));
            }
            else if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                HideTooltip();
            }
        }
    }

    void DisplayTooltip(string textToDisplay, Vector3 poz)
    {
        Dealer.instance.tooltipText.text = textToDisplay;
        poz = new Vector3(poz.x + 200f, poz.y - 60f, poz.z);
        tooltip.position = poz;
        tooltip.gameObject.SetActive(true);
    }

    void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
