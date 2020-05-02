/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CardInfo;
using UnityEngine.XR.MagicLeap;
using InputController;
using UI;

namespace GameLogic
{
    /// <summary>
    /// Using a Finited State Machine to control the game loop in Player Turn
    /// </summary>
    public class FakePlayerFSM : Player
    {
        #region Private Variable
        /// <summary>
        /// myState record the states of the player
        /// </summary>
        private PlayerStates myState=PlayerStates.Init;

        /// <summary>
        /// Only Init when game start
        /// </summary>
        private bool _bInit=false;

        /// <summary>
        /// Remember the startTime, wait 5 seconds for MagicLeap to StartUp
        /// </summary>
        private float startTime;

        /// <summary>
        /// The Card Player played
        /// </summary>
        private string PlayedCardName;

        /// <summary>
        /// This is a pointer to the Monster in UI
        /// </summary>
        private GameObject UIMonsterpreview;

        ///<summary>
        ///This is a string recording the monster name
        ///</summary>
        private string UIMonsterName;

        ///<summary>
        ///This is Raycast hit point position
        ///</summary>
        private Vector3 RayHitPosition;

        ///<summary>
        /// The ID of one hex to place new monster
        ///</summary>
        private int targetHexId;
        #endregion

        #region Public Variable
        
        /// <summary>
        /// Preview GameObject under this gameobject
        /// </summary>
        public GameObject MonsterPreviewField;

        /// <summary>
        /// CardInfo Reader
        /// </summary>
        public GetCardInstruction myCardDataBase;

        /// <summary>
        /// The Text box in UI show how many card you are tracking.
        /// </summary>
        [Tooltip("The Text box in UI show how many card you are tracking.")]
        public Text NumberOfCard;

        /// <summary>
        /// The Text box in UI show which card you are tracking.
        /// </summary>
        [Tooltip("The Text box in UI show which card you are tracking.")]
        public Text CardInfo;

        /// <summary>
        /// Start Hand Tracker when game Launch
        /// </summary>
        public GameObject myHandTrackerObject;
        ///<summary>
        /// A link to HandTracker
        ///</summary>
        public HandTracker myHandTracker;
        /// <summary>
        /// The Text box in UI show Instructions based on states.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions based on states.")]
        public Text Instructions;

        /// <summary>
        /// The Array of HexMap
        /// </summary>
        public Transform HexMap;

        /// <summary>
        /// GameObject of MainCamera
        /// </summary>
        public GameObject MainCamera;

        /// <summary>
        /// GameObject of Controller Manager
        /// </summary>
        public ControllerManager ControllerManager;

        /// <summary>
        /// GameObject of the CardUIManager
        /// </summary>
        [Tooltip("GameObject of the CardUIManager")]
        public ContentUIManager ContentUIManager;
        #endregion

        // Start is called before the first frame update
        public virtual void Start()
        {
            if(_bInit)
            {
                myState = PlayerStates.Init;
                startTime = Time.time;
                _bInit = true;
            }
            //Event Init
            if(Event_PlayerTurnStart == null)
            {
                Event_PlayerTurnStart = new UnityEvent();
            }
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);
            if(Event_ScanFinished==null)
            {
                Event_ScanFinished = new UnityEvent();
            }
            Event_ScanFinished.AddListener(ScanFinishedInvoke);

            PlayedCard += PlayedCardInvoked;

            ControllerManager.ClickOnCard += ClickOnCardInvoked;
            ControllerManager.ClickOnHex += ClickOnHexInvoked;

        }

        ///<summary>
        ///Update is called once per frame
        ///</summary> 
        public virtual void Update()
        {
            //Raycast hit point position
            RayHitPosition = ControllerManager.hitPoint;

            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    Debug.Log("PlayerStates=Init");
                    //Instructions.text = "Now is Init States. \n Please scanning the room around you. \n Press trigger to finish scanning.";
                    // float nowTime = Time.time;
                    // if(nowTime-startTime>5.0f)
                    // {
                    //     if(myHandTrackerObject)
                    //         myHandTrackerObject.SetActive(true);
                    //     if(myHandTracker)
                    //     {
                    //         myHandTracker.FindPose += HandGesture;
                    //     }
                    // }
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    Debug.Log("PlayerStates=WaitForStart");
                    //Instructions.text = "Now is not your turn. Please wait for the game going.";
                    break;
                case (PlayerStates.ImageTrackingStart):
                    myState = PlayerStates.Main_Phase;
                    // PlayedCardName=myCardDataBase.GetRandomCard().CardName;
                    break;
                case (PlayerStates.Main_Phase): // Wait For Hand Event From Hand Tracker
                    Debug.Log("PlayerStates=Main_Phase");
                    //Instructions.text = "Now is your turn, Main-Phase.\n Please choose a card. \n And shape your right hand to OK pose to confirm.";
                    Debug.Log("Now is your turn, Main-Phase.Please choose a card.");

                    // if (NumberOfCard)
                    // {
                    //     NumberOfCard.text = "Now you are tracking 1 Card.";
                    // }
                    if (myCardDataBase)
                    {
                        ContentUIManager.ShowUICanvas();
                        
                        //Cards myCard = myCardDataBase.GetCard(PlayedCardName);
                        // CardInfo.text = "CardName:" + myCard.CardName + "\n"
                        //     + "Attack:" + myCard.Attack.ToString() + "\n"
                        //     + "HP:" + myCard.HP.ToString() + "\n"
                        //     + "Speed:" + myCard.Speed.ToString() + "\n"
                        //     + "Special Effect:\n"
                        //     + myCard.SpecialEffect;

                        // if (UIMonsterName != myCard.CardName)
                        // {
                        //     Destroy(UIMonsterpreview);
                        //     UIMonsterName = myCard.CardName;
                        //     Debug.Log(myCard.PrefabPath);
                        //     UIMonsterpreview = Object.Instantiate(Resources.Load<GameObject>(myCard.PrefabPath), MonsterPreviewField.transform);
                        // }
                    }
                    break;
                case (PlayerStates.Confirm_Phase): // Wait For Hand (OpenHand or Fist) From Hand Tracker
                    //Debug.Log("PlayerStates=Confirm_Phase");
                    //Instructions.text = "Now is your turn, Confirm-Phase.\n Please use Open-Hand pose to comfirm. \n Or use Fist pose to go back.";
                    if (NumberOfCard)
                    {
                        NumberOfCard.text = "Now you want to use " + PlayedCardName + ".";
                    }
                    if (myCardDataBase)
                    {

                        // CardInfo.text = "CardName:" + myCard.CardName + "\n"
                        //     + "Attack:" + myCard.Attack.ToString() + "\n"
                        //     + "HP:" + myCard.HP.ToString() + "\n"
                        //     + "Speed:" + myCard.Speed.ToString() + "\n"
                        //     + "Special Effect:\n"
                        //     + myCard.SpecialEffect;

                        // if (UIMonsterName != myCard.CardName)
                        // {
                        //     Destroy(UIMonsterpreview);
                        //     Debug.Log(myCard.PrefabPath);
                        //     UIMonsterpreview = Object.Instantiate(Resources.Load<GameObject>(myCard.PrefabPath), MonsterPreviewField.transform);
                        //     UIMonsterName = myCard.CardName;
                        // }
                    }
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    //Spawn Actor Here
                    //Instructions.text = "Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.";
                    //Debug.Log("Now is your turn, Spawn-Phase.\n Your monster is spawning into battlefield.");
                    // CardInfo.text = "";
                    // NumberOfCard.text = "";
                    Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        Cards myCard = myCardDataBase.GetCard(PlayedCardName);
                        // float myDis = 9999.9f;
                        // int index = 0;
                        // for(int i=0;i<HexMap.childCount;i++)
                        // {
                        //     Transform hex = HexMap.GetChild(i);
                        //     if((MainCamera.transform.position - hex.position).magnitude<myDis)
                        //     {
                        //         myDis = (MainCamera.transform.position - hex.position).magnitude;
                        //         index = i;
                        //     }
                        // }

                        PlayedCard(PlayerId, myCard.id, targetHexId);
                    }
                    //myState = PlayerStates.ImageTrackingStop;
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.ImageTrackingStop):
                    myState = PlayerStates.End;
                    break;
                case (PlayerStates.End):
                    //Return the game control loop to main logic
                    //Debug.Log("PlayerStates=End");
                    //Instructions.text = "Now is your turn, End-Phase. Switch to next turn.";
                    PlayerEnd(PlayerId);
                    myState = PlayerStates.WaitForStart;
                    break;
                default:
                    break;
            }
        }

        #region Event Handler
        /// <summary>
        /// Call this function when Event_PlayerTurnStart invoke
        /// Change User state from WaitForStart to Main_Phase
        /// </summary>
        void PlayerTurnStartInvoke()
        {
             if(myState==PlayerStates.WaitForStart)
            {
                myState = PlayerStates.ImageTrackingStart;
                //myHandTracker.Event_enableOKtracked.Invoke();
            }
        }

        /// <summary>
        /// Call this function when Event_ScanFished invoke
        /// Change User state from Init to WaitForStart
        /// </summary>
        void ScanFinishedInvoke()
        {
            if(myState==PlayerStates.Init)
            {
                myState = PlayerStates.WaitForStart;
            }
        }

        /// <summary>
        /// This function will call when click on a Card
        /// </summary>
        /// <param name="CardName">The Chosen Card Name</param>
        private void ClickOnCardInvoked(string CardName)
        {
            if(myState==PlayerStates.Main_Phase)
            {
                PlayedCardName=myCardDataBase.GetRandomCard().CardName;
                ContentUIManager.HideUICanvas();
                Debug.Log("Now Player want to use " + PlayedCardName);
                myState = PlayerStates.Confirm_Phase;
                //Debug.LogFormat("Now Player want to use {0}, ATK: {1}, HP: {2}, SPEED: {3}, SPECIAL EFFECT: {4}", PlayedCardName.CardName, PlayedCardName.Attack, PlayedCardName.HP, PlayedCardName.Speed, PlayedCardName.SpecialEffect);
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a Hex
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if(myState == PlayerStates.Confirm_Phase)
            {
                targetHexId = HexTileID;
                myState = PlayerStates.Spawn_Phase;
            }
            return;
        }

        #endregion

        #region Delegate Handler
        /// <summary>
        /// Invoke when receive delegate from HandTracker
        /// </summary>
        /// <param name="PoseId">
        /// Id=0, OK Pose
        /// Id=1, OpenHand Pose
        /// id=2, Fist Pose
        /// </param>

        // private void HandGesture(int PoseId)
        // {
        //     Debug.Log("HandGestrue here!");
        //     if(PoseId==0)
        //     {
        //         if(myState==PlayerStates.Main_Phase)
        //         {
        //             myHandTracker.Event_enableFistTracked.Invoke();
        //             myHandTracker.Event_enableOpenHandtracked.Invoke();
        //             myState = PlayerStates.Confirm_Phase;
        //         }
        //     }
        //     if(PoseId==1)
        //     {
        //         if(myState==PlayerStates.Confirm_Phase)
        //         {
        //             myHandTracker.Event_disableAllTracked.Invoke();
        //             myState = PlayerStates.Spawn_Phase;
        //         }
        //     }
        //     if(PoseId==2)
        //     {
        //         if(myState==PlayerStates.Confirm_Phase)
        //         {
        //             PlayedCardName = "NON";
        //             myState = PlayerStates.Main_Phase;
        //         }
        //     }
        // }

        /// <summary>
        /// Invoked when the player is playing card
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="CardIndex">The played card index</param>
        /// <param name="HexIndex">The hex that card has been place on</param>
        private void PlayedCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            Debug.LogFormat("Player {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            return;
        }
        #endregion
    }
}
*/
