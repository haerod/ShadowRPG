using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTile : MonoBehaviour
{
    public Text nameText;
    //public Image faceImage;
    public int index;
    public RectTransform energyBarRt;
    [SerializeField] RectTransform healthBarRt;

    List<Image> energyImagesList = new List<Image>();
    [HideInInspector] public Vector2 targetPoz;
    [HideInInspector] public Character chara;
    RectTransform rt;
    bool isEndMovment = false;

    void Awake ()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isEndMovment)
            rt.position -= Vector3.right * 1000 * Time.deltaTime;
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
        chara.turnTile = this;
        GenerateEnergy();
        UpdateHealth();
    }

    // SANTE
    // Met à jour l'etat du personnage et bloque l'action reload
    public void UpdateHealth()
    {
        DestroyHealth();

        int sizeBetweenTiles = 2;

        for (int i = 0; i < chara.currentLife; i++)
        {
            RectTransform instaLife = Instantiate(Dealer.instance.lifeSlot, Vector3.zero, Quaternion.identity, healthBarRt).GetComponent<RectTransform>();
            instaLife.sizeDelta = new Vector2( // Donne la taille des lifeSlots
                (healthBarRt.rect.width - (chara.maxLife * sizeBetweenTiles)) / chara.maxLife,
                healthBarRt.rect.height - sizeBetweenTiles * 2);

            instaLife.gameObject.name = Dealer.instance.lifeSlot.name + "_" + (i + 1);

            instaLife.position = new Vector2( // Positionne les lifeSlots
                healthBarRt.position.x - healthBarRt.rect.width / 2 + sizeBetweenTiles * i + instaLife.rect.width * i + instaLife.rect.width / 2,
                healthBarRt.position.y);
        }
    }

    //Détruit les symboles énergie dans l'action bar
    public void DestroyHealth()
    {
        for (int i = 0; i < healthBarRt.transform.childCount; i++)
        {
            Destroy(healthBarRt.transform.GetChild(i).gameObject);
        }
    }
    //=========

    // ENERGY
    //Crée les PE dans l'action bar
    void GenerateEnergy()
    {
        DestroyEnergy();

        int maxAngle = 180;
        int radius = 45;
        int currentAngle;

        for (int i = 0; i < chara.maxEnergy; i++)
        {
            currentAngle = maxAngle / (chara.maxEnergy + 1); // Calcule la taille d'une portion d'angle
            currentAngle = currentAngle * (i + 1); // Calcule l'angle lié au PE

            Vector2 pozEnergy = new Vector2( // Calcule les coordonnées du PE;
                radius * Mathf.Cos((currentAngle+90) * Mathf.Deg2Rad),
                radius * Mathf.Sin((currentAngle +90) * Mathf.Deg2Rad));

            RectTransform pe = Instantiate(Dealer.instance.energyPref, Vector3.zero, Quaternion.identity, energyBarRt).GetComponent<RectTransform>(); // Crée le PE
            pe.sizeDelta /= 1.3f;
            pe.transform.localPosition = pozEnergy; // Assigne la position
            energyImagesList.Add(pe.GetComponent<Image>()); // Ajoute le PE à la liste
        }

        UpdateEnergy();
    }

    // Met à jour les symboles énergie (plein / vide)
    public void UpdateEnergy()
    {
        for (int i = 0; i < energyImagesList.Count; i++)
        {
            if (i <= chara.currentEnergy - 1)
            {
                energyImagesList[i].color = Dealer.instance.neutralStar;
            }
            else
            {
                energyImagesList[i].color = Dealer.instance.greyedStar;
            }
        }
    }

    // Cache les symboles Energie dans l'action bar
    public void DestroyEnergy()
    {
        for (int i = 0; i < energyImagesList.Count; i++)
        {
            Destroy(energyImagesList[i].gameObject);
        }
        energyImagesList.Clear();
    }
    //=========

    // Lance la destruction de la tuile en cas de mort du personnage
    public void DestroyItSelf()
    {
        isEndMovment = true;
        Destroy(this.gameObject, 1f);
    }
}
