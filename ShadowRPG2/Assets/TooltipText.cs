using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string textToDisplay;

    private RectTransform tooltip;
    private float xTooltip;
    private float yTooltip;

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


    public void OnPointerEnter(PointerEventData eventData)
    {
        Dealer.instance.tooltipText.text = textToDisplay;
        Dealer.instance.tooltipUI.sizeDelta = new Vector2(
            Dealer.instance.tooltipUI.rect.width, 
            Dealer.instance.tooltipText.preferredHeight + 20);
        tooltip.position = new Vector2(xTooltip, yTooltip);
        tooltip.gameObject.SetActive(true);
        //Instantiate(Dealer.instance.energyPref, tooltip.position, tooltip.rotation, Dealer.instance.mainCanvas);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}
