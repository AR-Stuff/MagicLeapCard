//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using CardInfo;
//using UnityEngine.UI;

//namespace UnityEngine.XR.MagicLeap
//{
//    /// <summary>
//    /// CardTracker should be disable in the editor and enable after game start.
//    /// Add Card Images and Names to the array.
//    /// When Card been tracked, it will change status in private variable.
//    /// Using GetStates and GetTrackingName to know which card has been used.
//    /// </summary>
//    [AddComponentMenu("MagicLeapCard/CardTracker")]
//    public class CardTracker : MonoBehaviour
//    {
//        #region Pulbic Variable

//        /// <summary>
//        /// Image that needs to be tracked.
//        /// Do not resize the image, the aspect ratio of the image provided here
//        /// and the printed image should be the same. Set the "Non Power of 2"
//        /// property of Texture2D to none.
//        /// </summary>
//        [Tooltip("Texture2D  of image that needs to be tracked. Do not change the aspect ratio of the image. Set the \"Non Power of 2\" property of Texture2D to \"none\".")]
//        public Texture2D[] CardImageList;

//        /// <summary>
//        /// Card Name
//        /// The Length Of the Array should be the same as @CardImageList
//        /// </summary>
//        public string[] CardNameList;

//        /// <summary>
//        /// Longer dimension of the printed image target in scene units.
//        /// If width is greater than height, it is the width, height otherwise.
//        /// </summary>
//        [Tooltip("Longer dimension of the printed image target in scene units. If width is greater than height, it is the width, height otherwise.")]
//        public float LongerDimensionInSceneUnits;

//        /// <summary>
//        /// CardInfo Reader
//        /// </summary>
//        public GetCardInstruction myCardDataBase;
//        #endregion

//        #region Private Variable
//        private MLImageTarget[] myCardTarget;
//        private MLImageTargetResult[] myCardResult;
//        #endregion

//        #region UnityFunction
//        /// <summary>
//        /// Start is called before the first frame update
//        /// </summary>
//        void Awake()
//        {
//            if(CardImageList.Length!=CardNameList.Length)
//            {
//                Debug.LogErrorFormat("CardTracker failed because the Image List doesn't match Name List");
//                return;
//            }
//            else
//            {
//                myCardTarget = new MLImageTarget[CardImageList.Length];
//                myCardResult = new MLImageTargetResult[CardImageList.Length];
//            }
//            if(MLImageTracker.IsStarted) MLImageTracker.Stop();
//            MLResult result = MLImageTracker.Start();
//            if (!result.IsOk)
//            {
//                Debug.LogErrorFormat("MLImageTrackerBehavior failed to start image tracker. Reason: {0}", result);
//                return;
//            }

//            for(int i=0;i<CardImageList.Length;i++)
//            {
//                myCardTarget[i] = MLImageTracker.AddTarget(CardNameList[i], CardImageList[i], LongerDimensionInSceneUnits, CardEvent, false);
//                if (myCardTarget[i] == null)
//                {
//                    Debug.LogErrorFormat("MLImageTrackerBehavior failed to add target {0} to the image tracker.", gameObject.name);
//                }
//                myCardResult[i].Status = MLImageTargetTrackingStatus.NotTracked;
//            }
            
//        }

//        // Update is called once per frame
//        void Update()
//        {
            
//        }

//        private void OnDestroy()
//        {
//            if (!MLImageTracker.IsStarted)
//                return;
//            for (int i=0;i<CardNameList.Length;i++)
//            {
//                MLImageTracker.RemoveTarget(CardNameList[i]);
//            }
//            MLImageTracker.Stop();
//        }

//        private void OnDisable()
//        {
//            if (!MLImageTracker.IsStarted)
//                return;
//            for (int i = 0; i < CardNameList.Length; i++)
//            {
//                MLImageTracker.RemoveTarget(CardNameList[i]);
//            }
//            MLImageTracker.Stop();
//        }
//        #endregion

//        #region EventHandlers
//        private void CardEvent(MLImageTarget imageTarget, MLImageTargetResult newResult)
//        {
//            int index = -1;
//            for(int i=0;i<myCardTarget.Length;i++)
//            {
//                if(imageTarget==myCardTarget[i])
//                {
//                    index = i;
//                    break;
//                }
//            }
//            if(index==-1)
//            {
//                Debug.LogErrorFormat("CardTracker failed. Reason: Find an Unknown Card.");
//                return;
//            }
//            if (newResult.Status != myCardResult[index].Status)
//            {
//                if (newResult.Status == MLImageTargetTrackingStatus.NotTracked)
//                    Debug.Log(("Card {0} Missed.", CardNameList[index]));
//                if (newResult.Status == MLImageTargetTrackingStatus.Tracked)
//                    Debug.Log(("Card {0} Find.", CardNameList[index]));
//                myCardResult[index] = newResult;
//            }
//        }
//        #endregion

//        #region Public Function
//        /// <summary>
//        /// Get number of Image been tracking
//        /// </summary>
//        /// <returns>
//        /// 0: No Card been Tracking
//        /// 1: 1 Card been Tracking, Well to go to next step
//        /// >1: More than 1 cards been tracking, Cannot go to next step
//        /// </returns>
//        public int GetStates()
//        {
//            int num = 0;
//            for(int i=0;i<myCardResult.Length;i++)
//            {
//                if(myCardResult[i].Status==MLImageTargetTrackingStatus.Tracked)
//                {
//                    num++;
//                }
//            }
//            return num;
//        }

//        /// <summary>
//        /// Get the name of Tracking Card, If more than one card been tracking return "MORE", If no card been tracking return "NON"
//        /// </summary>
//        /// <returns>
//        /// "NON", No card been tracking, cannot go to next step
//        /// "MORE", more than 1 card been tracking, cannot go to next step
//        /// "$CardName", return card name, can go to next step
//        /// "ERROR", Something Wrong!
//        /// </returns>
//        public string GetTrackingName()
//        {
//            int num = GetStates();
//            if(num==0)
//            {
//                return "NON";
//            }
//            else if(num>1)
//            {
//                return "MORE";
//            }
//            for(int i=0;i<myCardResult.Length;i++)
//            {
//                if(myCardResult[i].Status==MLImageTargetTrackingStatus.Tracked)
//                {
//                    return CardNameList[i];
//                }
//            }
//            return "ERROR";
//        }
//        #endregion
//    }
//}
