using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    public int currentTeam;

    [HideInInspector] public Character currentChara;
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
        ChooseAChara(true);
        StartCoroutine(ShowNewTurnPanel("VOTRE TOUR"));
    }

    // Regarde la liste des persos et prend le premier à être de la bonne team et à ne pas avoir joué, sinon passe à l'équipe suivante
    public void ChooseAChara(bool canPassToTheNextTeam)
    {
        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            if (Dealer.instance.allCharacters[i].team == currentTeam && !Dealer.instance.allCharacters[i].isActionEnded && !Dealer.instance.allCharacters[i].isDead)
            {
                SelectChara(Dealer.instance.allCharacters[i]);
                return;
                /////////////////////
            }
        }
        if(canPassToTheNextTeam)
            ChangeTeam(); // Si aucun chara ne peut jouer
    }

    // Sélectionne un chara
    public void SelectChara(Character chara)
    {
        currentChara = chara; // Donne le current chara
        ActionBar.instance.UpdateActionBar(currentChara, true); // Ajourne l'action bar
        chara.UpdateEnergy(); // Met à jour l'énergie en World UI
        Dealer.instance.faceCamera.transform.parent = currentChara.transform; // Assigne la face cam au nouveau perso
        Dealer.instance.faceCamera.transform.position = currentChara.faceCamTrans.position;
        Dealer.instance.faceCamera.transform.rotation = currentChara.faceCamTrans.rotation;
        PlayNewCharaTrack(currentChara);  // Lance la musique du perso
        MainSelector.instance.DisplaySelectedPlayerFeedback(currentChara); // Met le feedback sur le nouveau perso.
    }

    // Change d'équipe
    public void ChangeTeam()
    {
        if (currentTeam == 0)
        {
            currentTeam = 1;
            if (IsAnEnemyAlive())
                StartCoroutine(ShowNewTurnPanel("TOUR DES ADVERSAIRES"));
        }
        else
        {
            currentTeam = 0;
            if(IsAnEnemyAlive())
                StartCoroutine(ShowNewTurnPanel("VOTRE TOUR"));
        }

        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            Character chara = Dealer.instance.allCharacters[i];
            if (chara.team == currentTeam && !Dealer.instance.allCharacters[i].isDead)
            {
                chara.isActionEnded = false;
                chara.UpdateEnergy();
                if (chara.currentLife > 1 && chara.currentEnergy <= 0)
                    chara.ReloadEnergy();
            }
        }
        ChooseAChara(true);
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

    // Masque toutes les World UI sauf celle du chara en question
    public void HideAllWorldUIExeptOne(Character chara)
    {
        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            if(Dealer.instance.allCharacters[i] != chara)
                Dealer.instance.allCharacters[i].panelHealthEnergy.SetActive(false);
        }
    }

    // Affiche toutes les World UI
    public void DispalyAllWorldUI()
    {
        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            if(!Dealer.instance.allCharacters[i].isDead)
                Dealer.instance.allCharacters[i].panelHealthEnergy.SetActive(true);
        }
    }

    // Affiche la transition entre deux équipes
    IEnumerator ShowNewTurnPanel(string teamName)
    {
        GameObject panel = Dealer.instance.newTurnPanel;
        panel.SetActive(false);
        Text teamTxt = panel.transform.GetComponentInChildren<Text>();
        teamTxt.text = teamName;
        panel.SetActive(true);
        Dealer.instance.tooltipUI.gameObject.SetActive(false); // Désactive le tooltip pour éviter qu'il ne passe au-dessus.
        yield return new WaitForSeconds(1.5f); // AJUSTER A LA DUREE DE L'ANIMATION
        panel.SetActive(false);
    }

    bool IsAnEnemyAlive()
    {
        for (int i = 0; i < Dealer.instance.allCharacters.Length; i++)
        {
            if(Dealer.instance.allCharacters[i].team != 0 && !Dealer.instance.allCharacters[i].isDead)
            {
                return true;
            }
        }
        return false;
    }
}
