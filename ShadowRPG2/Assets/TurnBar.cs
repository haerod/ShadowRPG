using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    public int currentInitIndex;
    public float distBetweenTiles;
    public float bigTileDist;

    [HideInInspector] public Character currentChara;
    GameObject prefabTurnTile;
    float sizeTile;

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
        sizeTile = prefabTurnTile.GetComponent<RectTransform>().rect.height;

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
        currentChara = charaInitList[0];
        PlayNewCharaTrack(currentChara);
        CreateBar();
    }

    // Crée la Turn Bar
    public void CreateBar()
    {
        Vector2 vecPoz;

        for (int i = 0; i < charaInitList.Count; i++)
        {
                vecPoz = new Vector2(
                                    ActionBar.instance.energyBarRt.position.x - ActionBar.instance.energyBarRt.rect.width / 2 + prefabTurnTile.GetComponent<TurnTile>().energyBarRt.rect.width / 2,
                                    (ActionBar.instance.energyBarRt.position.y + ActionBar.instance.energyBarRt.rect.height / 2 + sizeTile / 2 + distBetweenTiles + bigTileDist) // position de base
                                    + (sizeTile * i) + (distBetweenTiles * i)); // position des tuiles entre elles 
                pozTurnTile.Add(vecPoz);
                TurnTile instaTurnTile = Instantiate(prefabTurnTile, vecPoz, Quaternion.identity, this.transform).GetComponent<TurnTile>();
                turnTileList.Add(instaTurnTile);
                instaTurnTile.chara = charaInitList[i];
                instaTurnTile.targetPoz = vecPoz;
                instaTurnTile.index = i;
                instaTurnTile.gameObject.name = "TurnTile(" + instaTurnTile.chara.charaName + ")";
                instaTurnTile.InitTile();
        }

        currentChara = charaInitList[0];
        ActionBar.instance.DisplayActionBar(currentChara, true);
        Dealer.instance.faceCamera.transform.parent = currentChara.transform; // Assigne la face cam au nouveau perso
        Dealer.instance.faceCamera.transform.position = currentChara.faceCamTrans.position;
        Dealer.instance.faceCamera.transform.rotation = currentChara.faceCamTrans.rotation;
    }

    // Met le nouveau perso en haut et le précédent en bas, puis lance le tour
    public void NextCharaTurn()
    {
        for (int i = 0; i < turnTileList.Count; i++) // Inverse les persos
        {
            if (turnTileList[i].index == 0)
            {
                turnTileList[i].index = turnTileList.Count - 1;
            }
            else
                turnTileList[i].index--;

            if(turnTileList[i].index == 0) 
            {
                currentChara = turnTileList[i].chara;
            }

            //turnTileList[i].targetPoz = pozTurnTile[turnTileList[i].index];
        }

        PlayNewCharaTrack(currentChara);  // Lance la musique du perso
        ActionBar.instance.DisplayActionBar(currentChara, true); // Ajourne l'action bar
        MainSelector.instance.DisplaySelectedPlayerFeedback(currentChara); // Met le feedback sur le nouveau perso.
        Dealer.instance.faceCamera.transform.parent = currentChara.transform; // Assigne la face cam au nouveau perso
        Dealer.instance.faceCamera.transform.position = currentChara.faceCamTrans.position; 
        Dealer.instance.faceCamera.transform.rotation = currentChara.faceCamTrans.rotation;
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

    // Joue la musique du nouveau personnage
    void PlayNewCharaTrack(Character chara)
    {
        AudioSource audioS = Camera.main.transform.GetChild(0).GetComponent<AudioSource>();
        if(audioS.clip != chara.charaTrack)
        {
            audioS.clip = chara.charaTrack;
            audioS.Play();
        }
    }
}
