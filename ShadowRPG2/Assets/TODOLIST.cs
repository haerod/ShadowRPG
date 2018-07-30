﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODOLIST : MonoBehaviour
{
    [Header("Bugs")]

    [Tooltip("Seul le toit OU les murs fonctionnent")]
    public bool TransparenceDesBâtimentsMurToit = false;
    [Tooltip("Le material des bâtiments n'est pas complètement transparent")]
    public bool TransparenceDesBâtimentsMaterial = false;
    [Tooltip("Clic sur les slots vides pour l'action Tir à distance")]
    public bool ClicSurCibleVide = false;
    [Tooltip("Les tooltips sont détectés à travers l'UI de la barre d'action")]
    public bool ToolitpATraversUI = false;
    [Tooltip("En cas d'attaque à ditance, un ennemi caché par un obstacle est quand même détecté")]
    public bool TirDetectionEnnemi = false;
    [Tooltip("Lorsqu'on clique sur un slot pour sélectionner un perso, ce dernier n'est pas sélecionné")]
    public bool ClicSurSlotSelection = false;
    [Tooltip("Lorsqu'on attaque à distance, le nombre de réussites affiche 0")]
    public bool ReussitesADistance = false;

    [Header("Features Générales")]

    [Tooltip("Virer l'aspect JDR pour rendre le jeu bitable")]
    public bool MAJTermes = false;
    [Tooltip("La caméra doit pouvoir se déplacer à la souris")]
    public bool CameraALaSouris = false;
    [Tooltip("Les personnages peuvent jouer les uns après les autres")]
    public bool ToursDeJeu = false;
    [Tooltip("Les ennemis ont une IA qui peut se déplacer et se battre")]
    public bool IAEnnemi = false;
    [Tooltip("Un personnage peut avoir plusieurs armes")]
    public bool PlusieursArmes = false;
    [Tooltip("Le joueur doit comprendre comment fonctionne l'attack window grâce à des tooltips")]
    public bool TooltipsAttackWindow = false;
    [Tooltip("Le joueur peut enlever des dés misés dans l'Attack window")]
    public bool BoutonMoinsAttackWindow = false;

    [Header("Features Actions")]

    [Tooltip("ACTION : Le personnage peut courir sur plusieurs slots")]
    public bool Course = false;
    [Tooltip("ACTION : Les personnages peuvent retarder leur initiative")]
    public bool RetarderIntitative = false;

    [Header("Notes et liens")]

    [Tooltip("Lien vers Visages")]
    public string LiensVersFaces = "https://icon-icons.com/fr/pack/Face-Avatars-Icons/108";
}
