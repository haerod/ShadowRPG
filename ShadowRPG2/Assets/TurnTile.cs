using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTile : MonoBehaviour
{
    public Text nameText;
    public Text stateText;
    public Character chara;
    public Image faceImage;
    public Image backgroundImage;
    public int index;

    public Vector2 targetPoz;

    RectTransform rt;

    void Awake ()
    {
        backgroundImage = GetComponent<Image>();
        nameText = transform.GetChild(0).GetComponent<Text>();
        stateText = transform.GetChild(1).GetComponent<Text>();
        faceImage = transform.GetChild(2).GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rt.position.y != targetPoz.y)
            rt.position = Vector2.Lerp(rt.position, targetPoz, .15f);

        if (Mathf.Abs(rt.position.y - targetPoz.y) < 1)
            rt.position = targetPoz;
    }

    public void InitTile()
    {
        nameText.text = chara.charaName;
        stateText.text = chara.currentStage + "+ / " + chara.currentEnergy + " dés";
        faceImage.sprite = chara.charaImage;
        backgroundImage.color = Dealer.instance.teamColors[chara.team];
        chara.turnTile = this;
    }
}
