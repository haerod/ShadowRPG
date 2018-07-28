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

	void Awake ()
    {
        faceImage = GetComponent<Image>();
        nameText = transform.GetChild(0).GetComponent<Text>();
        stateText = transform.GetChild(1).GetComponent<Text>();
    }
	
    public void InitTile()
    {
        nameText.text = chara.charaName;
        stateText.text = chara.currentStage + "+";
        faceImage.sprite = chara.charaImage;
    }
}
