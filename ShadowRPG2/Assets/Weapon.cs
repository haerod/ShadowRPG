using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Sprite illustration;

    // Nom des effets
    public enum Effect { None, Poison, Stun };

    // Les effets des différentes étapes
    [System.Serializable] public struct Stage
    {
        public int successValue;
        public int damages;
        public Effect currentEffect;
    }

    [SerializeField] public Stage[] meleeStageArray;

    [Space]

    [SerializeField] public Stage[] distanceStageArray;




    // Retourne l'index du tableau de Stages correspondant au nombre de réussites
    // Si le nombre de réussites est entre deux valeurs du tableau, cela retourne la valeur inférieure la plus proche (ex : si l'arme a des effets à 2 et à 5 et que le joueur fait 3 succes, retourne l'index correspondant à 2)
    // ATTENTION : Si le nombre de succès est trop faible, retourne 99
    public int ReturnRank(int succeses, bool isMelee) // si  isMelee est false, c'est une attauqe à distance
    {
        if (isMelee)
        {
            for (int i = 0; i < meleeStageArray.Length; i++)
            {
                if (meleeStageArray[i].successValue == succeses) // Si le nbr de succès est égal à celui de ce rang
                {
                    return i;
                }
                else if (meleeStageArray[i].successValue < succeses) // Si le nbr de succès est supérieur à celui de ce rang
                {
                    if (i + 1 < meleeStageArray.Length) // S'il y a un rang supérieur
                    {
                        if (meleeStageArray[i + 1].successValue > succeses) // Si le nbr de succès est inférieur à celui du rang supérieur 
                        {
                            return i;
                        }
                    }
                    else
                    {
                        return i;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < distanceStageArray.Length; i++)
            {
                if (distanceStageArray[i].successValue == succeses)
                {
                    return i;
                }
                else if (distanceStageArray[i].successValue < succeses) // Si le nbr de succès est supérieur à celui de ce rang
                {
                    if (i + 1 < distanceStageArray.Length) // S'il y a un rang supérieur
                    {
                        if (distanceStageArray[i + 1].successValue > succeses) // Si le nbr de succès est inférieur à celui du rang supérieur 
                        {
                            return i;
                        }
                    }
                    else
                    {
                        return i;
                    }
                }

            }
        }

        return 99;
    }
}
