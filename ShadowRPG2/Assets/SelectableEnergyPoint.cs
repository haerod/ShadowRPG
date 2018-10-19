using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableEnergyPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int energyIndex; // La place de ce PE sur la barre (le 1er, le 2nd, ..?)
    [HideInInspector] public NewActionWindow aw;
    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(delegate { aw.chara.RollSuccesses(energyIndex +1); aw.DestroyItself();  });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        aw.currentEnergy = energyIndex;
        if (!GetComponent<Outline>() && energyIndex < aw.chara.currentEnergy)
        {
            gameObject.AddComponent<Outline>();
            GetComponent<Outline>().effectColor = Color.white;
            GetComponent<Outline>().effectDistance = Vector2.one*2;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Outline>())
        {
            Destroy(GetComponent<Outline>());
        }
        if(energyIndex == 0)
        {
            aw.currentEnergy = -1;
        }
    }

    public void SetColor(Color col)
    {
        img.color = col;
    }
}
