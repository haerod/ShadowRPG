using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponDescriptor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform panelEffect;
    [SerializeField] RectTransform barDistance;
    [SerializeField] RectTransform barMelee;

    RectTransform rt;
    int nbrOfElements;
    Weapon wp;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SizePanel();
        PozPanel();
        FullPanel();
        ShowHidePanelEffects(true);
        PozBars();
    }

    void SizePanel()
    {
        wp = TurnBar.instance.currentChara.currentWeapon;
        nbrOfElements = wp.meleeStageArray.Length + wp.distanceStageArray.Length + 1;

        panelEffect.sizeDelta = new Vector2(
            rt.rect.width,
            Dealer.instance.weaponStageChunck.GetComponent<RectTransform>().rect.height * nbrOfElements);
    }

    void PozPanel()
    {
        Vector2 vecPoz = new Vector2(
            rt.position.x,
            rt.position.y + rt.rect.height / 2 + panelEffect.rect.height / 2 + 5);
        panelEffect.position = vecPoz;
    }

    void FullPanel()
    {
        for (int i = 0; i < nbrOfElements; i++)
        {
            Transform ws = Instantiate(Dealer.instance.weaponStageChunck, Vector3.zero, Quaternion.identity, panelEffect).transform;
            Text successTxt = ws.GetChild(0).GetChild(0).GetComponent<Text>();
            Text descTxt = ws.GetChild(1).GetChild(0).GetComponent<Text>();

            // Distance
            if (i < wp.distanceStageArray.Length)
            {
                successTxt.text = wp.distanceStageArray[i].successValue + "";
                descTxt.text = "+" + wp.distanceStageArray[i].damages + " Dégâts";
                if (i == wp.distanceStageArray.Length - 1)
                    successTxt.text += "+";
            }
            // Séparation
            else if (i == wp.distanceStageArray.Length)
            {
                for (int j = 0; j < ws.childCount; j++)
                {
                    ws.GetChild(j).gameObject.SetActive(false);
                }
            }
            // Melee
            else if (i > wp.distanceStageArray.Length)
            {
                successTxt.text = wp.meleeStageArray[i - wp.distanceStageArray.Length - 1].successValue + "";
                descTxt.text = "+" + wp.meleeStageArray[i - wp.distanceStageArray.Length - 1].damages + " Dégâts";
                if (i == wp.meleeStageArray.Length - 1)
                    successTxt.text += "+";
            }
        }
    }

    void PozBars()
    {
        float yPozBar;

        // DISTANCE
        if (wp.distanceStageArray.Length > 0)
        {
            yPozBar = panelEffect.position.y + panelEffect.rect.height / 2 /*y top panel effects*/ - (panelEffect.rect.height/nbrOfElements * wp.distanceStageArray.Length)/2;
            //print(pozMax.y);

            barDistance.position = new Vector2(
                barDistance.position.x,
                yPozBar);

            barDistance.sizeDelta = new Vector2(
                barDistance.rect.width,
                Dealer.instance.weaponStageChunck.GetComponent<RectTransform>().rect.height * wp.distanceStageArray.Length);
        }
        else
        {
            barDistance.gameObject.SetActive(false);
        }

        // MELEE
        if (wp.meleeStageArray.Length > 0)
        {
            yPozBar = (panelEffect.position.y + panelEffect.rect.height / 2) /*y top panel effects*/ - (panelEffect.rect.height / nbrOfElements * (wp.distanceStageArray.Length + 1)) - ((panelEffect.rect.height / nbrOfElements * wp.meleeStageArray.Length) / 2);
            //print(pozMax.y);

            barMelee.position = new Vector2(
                barMelee.position.x,
                yPozBar);

            barMelee.sizeDelta = new Vector2(
                barMelee.rect.width,
                Dealer.instance.weaponStageChunck.GetComponent<RectTransform>().rect.height * wp.meleeStageArray.Length);
        }
        else
        {
            barMelee.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowHidePanelEffects(false);
        for (int i = 0; i < panelEffect.childCount; i++)
        {
            Destroy(panelEffect.transform.GetChild(i).gameObject);
        }
    }

    public void ShowHidePanelEffects(bool setActive)
    {
        panelEffect.gameObject.SetActive(setActive);
        barDistance.gameObject.SetActive(setActive);
        barMelee.gameObject.SetActive(setActive);
    }
}
