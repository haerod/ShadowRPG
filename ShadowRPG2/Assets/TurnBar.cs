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
    [HideInInspector] public static TurnBar instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Il y a deux instances de TurnBar");
            Destroy(this.gameObject);
        }
    }

	void Start ()
    {
        prefabTurnTile = Dealer.instance.charaTurn;
        rt = GetComponent<RectTransform>();
        size = prefabTurnTile.GetComponent<RectTransform>().rect.height;
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
            instaTurnTile.gameObject.name = "TurnTile(" + instaTurnTile.chara.charaName + ")";
            instaTurnTile.InitTile();
        }
    }

    // Met le nouveau perso en haut et le précédent en bas
    public void NextCharaTurn()
    {
        for (int i = 0; i < turnTileList.Count; i++)
        {
            if (turnTileList[i].index == 0)
            {
                turnTileList[i].index = turnTileList.Count - 1;
                turnTileList[i].transform.SetSiblingIndex(0); // Lui donne l'index 0 auprès de son parent
            }
            else
                turnTileList[i].index --;

            turnTileList[i].targetPoz = pozTurnTile[turnTileList[i].index];
        }
    }

    // Enlève une tuile de la barre à un index donné
    public void RemoveTileAt(int index)
    {
        int newIndex = 0;

        for (int i = 0; i < turnTileList.Count; i++) // Cherche l'index de la tile à modifier dans la liste
        {
            if (turnTileList[i].index == index)
            {
                newIndex = i;
                break;
            }
        }

        turnTileList[newIndex].DestroyItSelf(); // Destruction
        turnTileList.RemoveAt(newIndex);
        pozTurnTile.RemoveAt(pozTurnTile.Count - 1);

        for (int i = 0; i < turnTileList.Count; i++) // Modifie l'index de tous les objets inférieurs
        {
            if (turnTileList[i].index > index)
            {
                turnTileList[i].index--;
                turnTileList[i].targetPoz = pozTurnTile[turnTileList[i].index];
            }
        }
    }

    // Donne le chara dont c'est le tour
    public Character ReturnCurrentCharacter()
    {
        for (int i = 0; i < turnTileList.Count; i++) // Cherche le chara ayant le bon index
        {
            if (turnTileList[i].index == 0)
            {
                return turnTileList[i].chara;
            }
        }

        return null;
    }
}
