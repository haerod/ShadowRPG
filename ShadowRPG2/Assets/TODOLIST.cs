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

    [Tooltip("Les armes ont des effets différents en fonction du nombre de réussites du joueur")]
    public bool EffetsArme;
    [Tooltip("Un personnage peut avoir plusieurs armes")]
    public bool PlusieursArmes = false;
    [Tooltip("Une fonction permet de centrer automatiquement la caméra sur un perso")]
    public bool CentrerCamera = false;
    [Tooltip("Les ennemis ont une IA qui peut se déplacer et se battre")]
    public bool IAEnnemi = false;
    [Tooltip("Unifier les feedbacks entre : le déplacement, les slots à portée mais interdits sont gris, hors de portée restent bleus et le tir tous les slots non autorisés sont gris.")]
    public bool UnifierFeedbacks = false;
    [Tooltip("Les tooltips de l'UI ne fonctionnent plus.")]
    public bool TooltipsUI = false;
    [Tooltip("La portée d'attaque à distance et la distance de shoot ne correspondent pas.")]
    public bool RangeShoot = false;
    [Tooltip("Lorsque le joueur clique sur un personnage dont ce n'est pas le tour, les boutons EndTurn et Reload Energy doivent être cachés.")]
    public bool HideEndTurnAndReload = false;
    [Tooltip("Les feedbacks de dégâts/réussites doivent être intanciés dans le monde plutôt que sur le canvas UI de manière à rester à leur place")]
    public bool WorldFeedback = false;

    [Space]

    [Tooltip("La vie doit s'afficher et se mettre à jour dans les turn tiles")]
    public bool UpdateTurnTiles = false;
    [Tooltip("Les turnTiles doivent se déplacer au nouvelles positions en cas de changement de tour")]
    public bool PositionTurnTiles = false;
    [Tooltip("Les turnTiles doivent se déplacer au nouvelles positions si un personnage est tué")]
    public bool PositionTurnTilesDeath = false;

    [Space]

    [Tooltip("Les compétences doivent afficher le % correspondant")]
    public bool ActionPct = false;
    [Tooltip("La valeur de cover (et armor ? à vérifier si ça marche) doit se mettre à jour")]
    public bool MAJCover = false;

    [Header("Features Actions")]

    [Tooltip("ACTION : Le personnage peut se déplacer sur plusieurs slots, au coût d'1 PE / slot (le premier étant gratuit)")]
    public bool Course = false;

    [Header ("Polish")]
    [Tooltip("Ajouter une balle avec une traînée lors du tir")]
    public bool FeedbackShoot = false;

    [Header("Notes et liens")]

    [Tooltip("Lien vers Visages")]
    public string LiensVersFaces = "https://icon-icons.com/fr/pack/Face-Avatars-Icons/108";
}
