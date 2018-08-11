using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    public int currentInitIndex;
    public float distBetweenImages;
    [SerializeField] RectTransform currentCharaSquare;

    GameObject prefabTurnTile;
    RectTransform rt;
    float size;

    List<TurnTile> turnTileList = new List<TurnTile>();
    List<Vector2> pozTurnTile = new List<Vector2>();
    List<Character> charaInitList = new List<Character>();
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

        CreateTurnList();
	}

    // Crée la turn list
    public void CreateTurnList()
    {
        int min = 0;
        int max = 0;
        List<Character> tempList = new List<Character>();

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++) // Calcule init min
        {
            if (Dealer.instance.allCharacters[i].init < min)
                min = Dealer.instance.allCharacters[i].init;
        }

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++) // Calcule init max
        {
            if (Dealer.instance.allCharacters[i].init > max)
                max = Dealer.instance.allCharacters[i].init;
        }

        for (int i = min; i < max + 1; i++) // Organise les persos dans l'ordre
        {
            tempList.Clear();
            for (int j = 0; j < Dealer.instance.allCharacters.Length; j++)
            // Pour chaque valeur d'init possible, récupère tous les persos ayant cette init
            {

                if (Dealer.instance.allCharacters[j].init == i)
                    tempList.Add(Dealer.instance.allCharacters[j]);
            }
            if (tempList.Count > 0)
            //Prend les persos possibles et les ajoute à la liste au hasard
            {
                tempList = Dealer.instance.ShuffleObjectList(tempList);
                foreach (Character chara in tempList)
                {
                    charaInitList.Add(chara);
                }
            }
        }

        charaInitList.Reverse(); // Retourne la liste
        CreateBar();
    }

    // Crée la Turn Bar
    public void CreateBar()
    {
        Vector2 vecPoz;

        for (int i = 0; i < charaInitList.Count; i++)
        {
            vecPoz = new Vector2(rt.position.x,
                (rt.position.y + rt.rect.height / 2 - size) // position de base
                - (size * i) - (distBetweenImages * i)); // position des tuiles entre elles 
            pozTurnTile.Add(vecPoz);
            TurnTile instaTurnTile = Instantiate(prefabTurnTile, vecPoz, Quaternion.identity, this.transform).GetComponent<TurnTile>();
            turnTileList.Add(instaTurnTile);
            instaTurnTile.chara = charaInitList[i];
            instaTurnTile.targetPoz = vecPoz;
            instaTurnTile.index = i;
            instaTurnTile.gameObject.name = "TurnTile(" + instaTurnTile.chara.charaName + ")";
            instaTurnTile.InitTile();

            if (i == 0) // positionne currentCharaSquare
                currentCharaSquare.position = vecPoz;
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
    public Character GetCurrentCharacter()
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
