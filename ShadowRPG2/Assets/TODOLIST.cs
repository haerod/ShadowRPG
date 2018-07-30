using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODOLIST : MonoBehaviour
{
    [Header("Bugs")]

    [Tooltip("En cas d'attaque à ditance, un ennemi caché par un obstacle est quand même détecté")]
    public bool TirDetectionEnnemi = false;
    [Tooltip("Le material des bâtiments n'est pas complètement transparent")]
    public bool TransparenceDesBâtimentsMaterial = false;

    [Header("Features Générales")]

    [Tooltip("Virer l'aspect JDR pour rendre le jeu bitable")]
    public bool MAJTermes = false;
    [Tooltip("Le joueur doit comprendre comment fonctionne l'attack window grâce à des tooltips")]
    public bool TooltipsAttackWindow = false;
    [Tooltip("Le joueur peut enlever des dés misés dans l'Attack window")]
    public bool BoutonMoinsAttackWindow = false;
    [Tooltip("Un personnage peut avoir plusieurs armes")]
    public bool PlusieursArmes = false;
    [Tooltip("Une fonction permet de centrer automatiquement la caméra sur un perso")]
    public bool CentrerCamera = false;
    [Tooltip("Les ennemis ont une IA qui peut se déplacer et se battre")]
    public bool IAEnnemi = false;

    [Header("Features Actions")]

    [Tooltip("ACTION : Le personnage peut courir sur plusieurs slots")]
    public bool Course = false;
    [Tooltip("ACTION : Les personnages peuvent retarder leur initiative")]
    public bool RetarderIntitative = false;

    [Header("Notes et liens")]

    [Tooltip("Lien vers Visages")]
    public string LiensVersFaces = "https://icon-icons.com/fr/pack/Face-Avatars-Icons/108";
}
