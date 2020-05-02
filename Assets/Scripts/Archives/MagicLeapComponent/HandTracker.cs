//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.XR.MagicLeap;
//using UnityEngine.Events;
//using UnityEngine;

//namespace GameLogic
//{
//    /// <summary>
//    /// Return Tracked Pose
//    /// </summary>
//    /// <param name="PoseId">
//    /// Id=0, OK Pose
//    /// Id=1, OpenHand Pose
//    /// Id=2, Fist Pose
//    /// </param>
//    public delegate void FindPose(int PoseId);

//    /// <summary>
//    /// HandTracker should be disable in the editor and enable after game start.
//    /// Using GetHandPose To get Hand Pose Event
//    /// Only Enable Once otherwise it will cause bug
//    /// </summary>
//    [AddComponentMenu("MagicLeapCard/HandTracker")]
//    public class HandTracker : MonoBehaviour
//    {
//        #region Private Variable
//        /// <summary>
//        /// Check Init
//        /// </summary>
//        private bool _bInit = false;
//        ///<summary>
//        /// bOKtracked=true and the hand gesture is OK, return in delegate, id=0, otherwise not return
//        ///</summary>
//        private bool bOKtracked = false;
//        ///<summary>
//        /// bOpenHandtracked=true and the hand gesture is OpenHand, return in delegate, id=1, otherwise not return
//        ///</summary>
//        private bool bOpenHandtracked = false;
//        ///<summary>
//        /// bFisttracked=true and the hand gesture is Fist, return in delegate, id=2, otherwise not return
//        ///</summary>
//        private bool bFisttracked = false;
//        #endregion

//        #region Public Event
//        /// <summary>
//        /// Event to change bOKtracked
//        /// </summary>
//        public UnityEvent Event_enableOKtracked;
//        /// <summary>
//        /// Event to change bOpenHandtracked
//        /// </summary>
//        public UnityEvent Event_enableOpenHandtracked;
//        /// <summary>
//        /// Event to change bFisttracked
//        /// </summary>
//        public UnityEvent Event_enableFistTracked;
//        /// <summary>
//        /// Event to disable all tracked
//        /// </summary>
//        public UnityEvent Event_disableAllTracked;
//        #endregion

//        #region Public Variable
//        ///<summary>
//        ///PointFilterLevel
//        ///</summary>
//        public MLKeyPointFilterLevel _keyPointFilterLevel = MLKeyPointFilterLevel.ExtraSmoothed;

//        ///<summary>
//        ///PostFilterLevel
//        ///</summary>
//        public MLPoseFilterLevel _PoseFilterLevel = MLPoseFilterLevel.ExtraRobust;

//        /// <summary>
//        /// bRightHand=true, if the player is right-handed.
//        /// bRightHand=false, if the player is left-handed.
//        /// </summary>
//        public bool bRightHand=true;

//        /// <summary>
//        /// 
//        /// </summary>
//        public FindPose FindPose;
//        #endregion

//        #region Unity Function
//        /// <summary>
//        /// Start is called before the first frame update
//        /// </summary>
//        void Start()
//        {
//            if(!_bInit)
//            {
//                InitializeAPI();
//                InitializeEvent();
//            }
            
//        }

//        /// <summary>
//        /// On Enable
//        /// </summary>
//        private void OnEnable()
//        {
//            if(!_bInit)
//            {
//                InitializeAPI();
//                InitializeEvent();
//            }
//        }
        
//        /// <summary>
//        /// Update is called once per frame
//        /// </summary>
//        void Update()
//        {
//            if(!_bInit)
//            {
//                return;
//            }

//            MLHand trackedHand;
//            if(bRightHand)
//            {
//                trackedHand = MLHands.Right;
//            }
//            else
//            {
//                trackedHand = MLHands.Left;
//            }

//            //Debug.Log(trackedHand.KeyPose.ToString());

//            if(bOKtracked)
//            {
//                if(trackedHand.KeyPose==MLHandKeyPose.Ok)
//                {
//                    Debug.Log("Send Delegate");
//                    FindPose(0);
//                }
//            }
//            if(bOpenHandtracked)
//            {
//                if (trackedHand.KeyPose == MLHandKeyPose.OpenHand)
//                {
//                    FindPose(1);
//                }
//            }
//            if (bFisttracked)
//            {
//                if (trackedHand.KeyPose == MLHandKeyPose.Fist)
//                {
//                    FindPose(2);
//                }
//            }

//        }

//        /// <summary>
//        /// Stops the communication to the MLHands API and unregisters required events.
//        /// </summary>
//        void OnDisable()
//        {
//            if (MLHands.IsStarted && _bInit)
//            {
//                MLHands.Stop();
//                _bInit = false;
//            }
//        }
//        #endregion

//        #region Private Function

//        /// <summary>
//        /// API initialize
//        /// </summary>
//        private void InitializeAPI()
//        {
//            if (!MLHands.IsStarted)
//            {
//                MLResult result = MLHands.Start();

//                if (!result.IsOk)
//                {
//                    Debug.LogErrorFormat("Error: HandTracking failed starting MLHands, disabling script. Reason: {0}", result);
//                    enabled = false;
//                    return;
//                }
//            }
//            _bInit = true;
//        }

//        ///<summary>
//        /// Event Initialize
//        ///</summary>
//        private void InitializeEvent()
//        {
//            if(Event_disableAllTracked==null)
//            {
//                Event_disableAllTracked = new UnityEvent();
//            }
//            Event_disableAllTracked.AddListener(DisableAllTracked);
//            if (Event_enableOKtracked==null)
//            {
//                Event_enableOKtracked = new UnityEvent();
//            }
//            Event_enableOKtracked.AddListener(EnableOKTracked);
//            if (Event_enableFistTracked==null)
//            {
//                Event_enableFistTracked = new UnityEvent();  
//            }
//            Event_enableFistTracked.AddListener(EnableFistTracked);
//            if (Event_enableOpenHandtracked==null)
//            {
//                Event_enableOpenHandtracked = new UnityEvent();
//            }
//            Event_enableOpenHandtracked.AddListener(EnableOpenHandTracked);
//        }
//        #endregion

//        #region Public Function
//        /// <summary>
//        /// API for Get hand pose based on User's perferance
//        /// </summary>
//        /// <returns>
//        /// Return a HandPose
//        /// </returns>
//        public MLHandKeyPose GetHandPose()
//        {
//            if(bRightHand)
//            {
//                return MLHands.Right.KeyPose;
//            }
//            else
//            {
//                return MLHands.Left.KeyPose;
//            }
//        }

//        /// <summary>
//        /// API to set wether the player is left-handed or right-handed
//        /// </summary>
//        /// <param name="_bRight">
//        /// _bRight=true if the player is right-handed
//        /// </param>
//        public void Left_Right_Switch(bool _bRight)
//        {
//            bRightHand = _bRight;
//        }
//        #endregion

//        #region Event handler
//        /// <summary>
//        /// Invoke when Event_enableOKtracked received
//        /// </summary>
//        public void EnableOKTracked()
//        {
//            Debug.Log("Go into the event");
//            bOKtracked = true;
//        }

//        /// <summary>
//        /// Invoke when Event_enableFisttracked received
//        /// </summary>
//        public void EnableFistTracked()
//        {
//            bFisttracked = true;
//        }

//        /// <summary>
//        /// Invoke when Event_enableOpenHandtracked received
//        /// </summary>
//        public void EnableOpenHandTracked()
//        {
//            bOpenHandtracked = true;
//        }

//        /// <summary>
//        /// Invoke when Event_DisableAllTracked received
//        /// </summary>
//        public void DisableAllTracked()
//        {
//            bOKtracked = false;
//            bFisttracked = false;
//            bOpenHandtracked = false;
//        }
//        #endregion
//    }
//}


