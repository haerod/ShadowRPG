using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTile : MonoBehaviour
{
    public Text nameText;
    public Character chara;
    public Image faceImage;
    public Image backgroundImage;
    public int index;

    public Vector2 targetPoz;

    RectTransform rt;
    bool isEndMovment = false;

    void Awake ()
    {
        backgroundImage = GetComponent<Image>();
        nameText = transform.GetChild(1).GetComponent<Text>();
        faceImage = transform.GetChild(0).GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isEndMovment)
            rt.position += Vector3.right * 1000 * Time.deltaTime;
        else
        {
            if (rt.position.y != targetPoz.y)
                rt.position = Vector2.Lerp(rt.position, targetPoz, .15f);

            if (Mathf.Abs(rt.position.y - targetPoz.y) < 1)
                rt.position = targetPoz;
        }
    }

    // Initialise les différentes propriétés de la tuile (nom, image, etc.)
    public void InitTile()
    {
        nameText.text = chara.charaName;
        faceImage.sprite = chara.charaImage;
        backgroundImage.color = Dealer.instance.teamColors[chara.team];
        chara.turnTile = this;
    }

    // Lance la destruction de la tuile en cas de mort du personnage
    public void DestroyItSelf()
    {
        isEndMovment = true;
        Destroy(this.gameObject, 1f);
    }
}
