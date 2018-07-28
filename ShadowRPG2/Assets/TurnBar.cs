using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    public float distBetweenImages;

    GameObject prefabCharaTurn;
    RectTransform rt;
    float size;

	void Start ()
    {
        prefabCharaTurn = Dealer.instance.charaTurn;
        rt = GetComponent<RectTransform>();
        size = prefabCharaTurn.GetComponent<RectTransform>().rect.height;

        CreateBar();
	}
	
	void Update ()
    {
		
	}

    // Crée la Turn Bar
    void CreateBar()
    {
        Vector2 vecPoz;

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            vecPoz = new Vector2(rt.position.x,
                rt.position.y - (size * i) - (distBetweenImages * i));
            TurnTile instaCharaTurn = Instantiate(prefabCharaTurn, vecPoz, Quaternion.identity, this.transform).GetComponent<TurnTile>();
            instaCharaTurn.chara = Dealer.instance.allCharacters[i];
            instaCharaTurn.InitTile();
        }
    }
}
