using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CardInfo
{
    /// <summary>
    /// Cards is a class to store data of a card
    /// </summary>
    [Serializable]
    public class NewCard
    {
        public int id;
        public bool isMonster; //True is monster, False is terrain
        public string CardName;
        public int Attack;
        public int HP;
        public int Speed;
        public string SpecialEffect;
        public GameObject CardPrefab;
    }

    public class NewCardDB : MonoBehaviour
    {
        public NewCard[] cards;


        /// <summary>
        /// Input A cardName return a class that include all information about card
        /// </summary>
        /// <param name="CardName">
        /// The name of requesting card
        /// </param>
        /// <returns>
        /// A instance of Class Cards that include all the information about card.
        /// </returns>
        public NewCard GetCard(string CardName)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].CardName == CardName)
                {
                    return cards[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Input A Card Index return a class that include all information about card
        /// </summary>
        /// <param name="CardIndex">The Card Index</param>
        /// <returns>A class that include all information of that card</returns>
        public NewCard GetCardByIndex(int CardIndex)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if(cards[i].id==CardIndex)
                    return cards[i];
            }
            return null;
        }

        ///<summary>
        /// Get a random Card from the database
        ///</summary>
        ///<returns>
        ///A instance of Class Cards that include all the information about the random card.
        ///</returns>
        public NewCard GetRandomCard()
        {
            int n = cards.Length;
            int k = (int)(UnityEngine.Random.value * n);
            while (k == n)
            {
                k = (int)(UnityEngine.Random.value * n);
            }
            return cards[k];
        }
    }
}

