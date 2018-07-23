using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODOLIST : MonoBehaviour
{
    [Header("Bugs")]
    [Tooltip("Seul le toit OU les murs fonctionnent")]
    public bool TransparenceDesBâtimentsMurToit = false;
    [Tooltip("Le material n'est pas complètement transparent")]
    public bool TransparenceDesBâtimentsMaterial = false;
    [Tooltip("Clic sur les slots vides pour l'action Tir à distance")]
    public bool ClicSurCibleVide = false;
    [Tooltip("Les tooltips sont détectés à travers l'UI de la barre d'action")]
    public bool ToolitpATraversUI = false;
}
