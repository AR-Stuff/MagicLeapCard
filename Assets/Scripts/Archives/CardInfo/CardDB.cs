//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;

//namespace CardInfo
//{
//    /// <summary>
//    /// Cards is a class to store data of a card
//    /// </summary>
//    public class Card : MonoBehaviour
//    {
//        /// <summary>
//        /// Card Index, Integer
//        /// </summary>
//        public int id;
//        /// <summary>
//        /// Card Name, String
//        /// </summary>
//        public string CardName;
//        /// <summary>
//        /// Card Attack, Integer
//        /// </summary>
//        public int Attack;
//        /// <summary>
//        /// Card Health, Integer
//        /// </summary>
//        public int HP;
//        /// <summary>
//        /// Card Speed, Integer
//        /// </summary>
//        public int Speed;
//        /// <summary>
//        /// Card SpecialEffect, String
//        /// </summary>
//        public string SpecialEffect;
//        /// <summary>
//        /// A string discribe the path to the prefabs used
//        /// </summary>
//        public string PrefabPath;
//    }

//    /// <summary>
//    /// CardDB read filePath(Default is "CardData.csv") to retrieve the card data when game started
//    /// Using GetCard(CardName) return a Cards
//    /// TODO: Using Database to replace the local File
//    /// </summary>
//    public class CardDB : MonoBehaviour
//    {
//        #region Private Variable
//        /// <summary>
//        /// 2D Array to storage Card Infomation
//        /// </summary>
//        private string[][] CardInfo;
//        /// <summary>
//        /// Array to storage the Attributes Name of the Card.
//        /// </summary>
//        private string[] keyName;
//        /// <summary>
//        /// File Path for data 
//        /// </summary>
//        [Tooltip("The File Path to the Card Data File.")]
//        public string filePath = "Assets/Project/Data/CardData.csv";
//        #endregion

//        #region Unity Function
//        // Start is called before the first frame update
//        /// <summary>
//        /// Init The CardInfo and keyName
//        /// </summary>
//        void Start()
//        {
//            string[] fileData = File.ReadAllLines(filePath);
//            keyName = fileData[0].Split(',');

//            CardInfo = new string[fileData.Length - 1][];

//            for (int i = 1; i < fileData.Length; i++)
//            {
//                string[] tempArr = fileData[i].Split(',');
//                CardInfo[i - 1] = new string[tempArr.Length];
//                for (int j = 0; j < tempArr.Length; j++)
//                {
//                    CardInfo[i - 1][j] = tempArr[j];
//                }
//            }
//        }
//        #endregion

//        #region Public Function
//        /// <summary>
//        /// Input A cardName return a class that include all information about card
//        /// </summary>
//        /// <param name="CardName">
//        /// The name of requesting card
//        /// </param>
//        /// <returns>
//        /// A instance of Class Cards that include all the information about card.
//        /// </returns>
//        public Card GetCard(string CardName)
//        {
//            Card newCard = new Card();
//            for (int i = 0; i < CardInfo.Length; i++)
//            {
//                if (CardInfo[i][1] == CardName)
//                {
//                    newCard.id = int.Parse(CardInfo[i][0]);
//                    newCard.CardName = CardInfo[i][1];
//                    newCard.Attack = int.Parse(CardInfo[i][2]);
//                    newCard.HP = int.Parse(CardInfo[i][3]);
//                    newCard.Speed = int.Parse(CardInfo[i][4]);
//                    newCard.SpecialEffect = CardInfo[i][5];
//                    newCard.PrefabPath = CardInfo[i][6];
//                }
//            }
//            return newCard;
//        }

//        /// <summary>
//        /// Input A Card Index return a class that include all information about card
//        /// </summary>
//        /// <param name="CardIndex">The Card Index</param>
//        /// <returns>A class that include all information of that card</returns>
//        public Card GetCardByIndex(int CardIndex)
//        {
//            Card newCard = new Card();
//            for (int i = 0; i < CardInfo.Length; i++)
//            {
//                if (CardInfo[i][0] == CardIndex.ToString())
//                {
//                    newCard.id = int.Parse(CardInfo[i][0]);
//                    newCard.CardName = CardInfo[i][1];
//                    newCard.Attack = int.Parse(CardInfo[i][2]);
//                    newCard.HP = int.Parse(CardInfo[i][3]);
//                    newCard.Speed = int.Parse(CardInfo[i][4]);
//                    newCard.SpecialEffect = CardInfo[i][5];
//                    newCard.PrefabPath = CardInfo[i][6];
//                    break;
//                }
//            }
//            return newCard;
//        }

//        ///<summary>
//        /// Get a random Card from the database
//        ///</summary>
//        ///<returns>
//        ///A instance of Class Cards that include all the information about the random card.
//        ///</returns>
//        public Card GetRandomCard()
//        {
//            string randomCard = "";
//            int n = CardInfo.Length;
//            int k = (int)(Random.value * n);
//            while (k == n)
//            {
//                k = (int)(Random.value * n);
//            }
//            randomCard = CardInfo[k][1];
//            return GetCard(randomCard);
//        }
//        #endregion
//    }
//}


