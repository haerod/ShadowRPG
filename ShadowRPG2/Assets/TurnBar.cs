using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    public int currentInitIndex;

    public float distBetweenImages;

    GameObject prefabTurnTile;
    RectTransform rt;
    float size;

    private List<TurnTile> turnTileList = new List<TurnTile>();
    private List<Vector2> pozTurnTile = new List<Vector2>();

	void Start ()
    {
        prefabTurnTile = Dealer.instance.charaTurn;
        rt = GetComponent<RectTransform>();
        size = prefabTurnTile.GetComponent<RectTransform>().rect.height;
	}
	
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.A))
        {
            NextCharaTurn();
        }
	}

    // Crée la Turn Bar
    public void CreateBar()
    {
        Vector2 vecPoz;

        for (int i = 0; i < MainSelector.instance.charaInitList.Count; i++)
        {
            vecPoz = new Vector2(rt.position.x,
                rt.position.y - (size * i) - (distBetweenImages * i));
            pozTurnTile.Add(vecPoz);
            TurnTile instaTurnTile = Instantiate(prefabTurnTile, vecPoz, Quaternion.identity, this.transform).GetComponent<TurnTile>();
            turnTileList.Add(instaTurnTile);
            instaTurnTile.chara = MainSelector.instance.charaInitList[i];
            instaTurnTile.targetPoz = vecPoz;
            instaTurnTile.index = i;
            instaTurnTile.InitTile();
        }
    }

    // Met le nouveau perso en haut et le précédent en bas
    void NextCharaTurn()
    {
        for (int i = 0; i < turnTileList.Count; i++)
        {
            if (turnTileList[i].index == 0)
            {
                turnTileList[i].index = turnTileList.Count - 1;
                turnTileList[i].transform.SetSiblingIndex(0);
            }
            else
                turnTileList[i].index --;

            turnTileList[i].targetPoz = pozTurnTile[turnTileList[i].index];
        }
    }
}
