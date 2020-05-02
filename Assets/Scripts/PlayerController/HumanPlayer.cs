using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CardInfo;
using UnityEngine.XR.MagicLeap;
using InputController;
using UI;
using GameWorld;
using Monsters;

namespace GameLogic
{
    /// <summary>
    /// Using a Finited State Machine to control the game loop in Player Turn
    /// </summary>
    public class HumanPlayer : Player
    {
        #region Private Variable
        /// <summary>
        /// myState record the states of the player
        /// </summary>
        private PlayerStates myState = PlayerStates.Init;

        /// <summary>
        /// Only Init when game start
        /// </summary>
        private bool _bInit = false;

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

        ///<summary>
        /// The num of monster card unchosen
        ///</summary>
        private int MCardUnchosen;

        ///<summary>
        /// The num of terrian card unchosen
        ///</summary>
        private int TCardUnchosen;

        /// <summary>
        /// For Audio Tutorial, When first time player choose hex, play the audio
        /// </summary>
        private bool _isFirstChosenHex = true;
        /// <summary>
        /// For Audio Tutorial, When first time player play card, play the audio
        /// </summary>
        private bool _isFirstPlayCard = true;
        /// <summary>
        /// For Audio Tutorial, When first time player click on a character
        /// </summary>
        private bool _isFirstClickOnCharacter = true;
        /// <summary>
        /// For Audio Tutorial, When first time player move a charcter
        /// </summary>
        private bool _isFirstMove = true;

        /// Const num of available monster card
        /// </summary>
        private int _numOfMonsterCouldUse;

        /// <summary>
        /// Const num of available terrain card
        /// </summary>
        private int _numOfTerrainCouldUse;

        /// <summary>
        /// Store the time when this state began
        /// </summary>
        private float stateBeginTime;

        /// <summary>
        /// Store player's monster when card selection finish
        /// </summary>
        private List<Monster> myMonsters = new List<Monster>();
        #endregion

        #region Public Variable
        /// <summary>
        /// NetworkEvent Sender
        /// </summary>
        public NetworkPlayer networkPlayer = null;

        /// <summary>
        /// Num of available monster card (default 3)
        /// </summary>
        public int numOfMonsterCouldUse = 3;

        /// <summary>
        /// Num of available terrain card
        /// </summary>
        public int numOfTerrainCouldUse = 2;

        /// <summary>
        /// CardInfo Reader
        /// Get all card from database
        /// </summary>
        public NewCardDB myCardDataBase;

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
        /// GameObject of the ContentUIManager
        /// </summary>
        [Tooltip("GameObject of the ContentUIManager")]
        public ContentUIManager ContentUIManager;
        /// <summary>
        /// The Text box in UI show Instructions.
        /// </summary>
        [Tooltip("The Text box in UI show Instructions.")]
        public Text InstructionUI;

        public Monster currMonster;
        public Monster targetMonster;
        #endregion

        // Start is called before the first frame update
        public virtual void Start()
        {
            if (_bInit)
            {
                ChangeState(PlayerStates.Init);
                // startTime = Time.time;
                _bInit = true;
            }
            //Event Init
            if (Event_PlayerTurnStart == null)
            {
                Event_PlayerTurnStart = new UnityEvent();
            }
            Event_PlayerTurnStart.AddListener(PlayerTurnStartInvoke);

            if (Event_ScanFinished == null)
            {
                Event_ScanFinished = new UnityEvent();
            }
            Event_ScanFinished.AddListener(ScanFinishedInvoke);

            PlayedCard += PlayedCardInvoked;
            PlayerEnd += PlayerEndInvoked;
            AttackDelegate += AttackInvoked;
            MoveDelegate += MoveInvoked;

            ControllerManager.ClickOnCard += ClickOnCardInvoked;
            ControllerManager.ClickOnHex += ClickOnHexInvoked;
            ControllerManager.ClickOnMonster += ClickOnMonsterInvoked;
            ControllerManager.ClickOnBtn += ClickOnBtnInvoked;

            MCardUnchosen = numOfMonsterCouldUse;
            TCardUnchosen = numOfTerrainCouldUse;
            _numOfMonsterCouldUse = numOfMonsterCouldUse;
            _numOfTerrainCouldUse = numOfTerrainCouldUse;
        }

        ///<summary>
        ///Update is called once per frame
        ///</summary> 
        public virtual void Update()
        {
            float nowTime = Time.time;
            //Raycast hit point position
            RayHitPosition = ControllerManager.hitPoint;

            //StateMachine Related
            switch (myState)
            {
                case (PlayerStates.Init):
                    //Debug.Log("PlayerStates=Init");
                    ContentUIManager.ShowUICanvas();
                    InstructionUI.text = "Place BattleFiled";
                    break;
                case (PlayerStates.WaitForStart): //Wait Event From Main Logic
                    InstructionUI.text = "Wait For Start";
                    break;
                case (PlayerStates.Main_Phase):
                    if (numOfMonsterCouldUse <= 0)
                    {
                        ChangeState(PlayerStates.End);
                        break;
                    }
                    //Card selection finished
                    if (MCardUnchosen <= 0 && TCardUnchosen <= 0)
                    {
                        //init myMonster List
                        while (myMonsters.Count < _numOfMonsterCouldUse)
                        {
                            foreach (KeyValuePair<int, Monster> monsterPair in world.monsters)
                            {
                                Monster thisMonster = monsterPair.Value;
                                if (thisMonster.monsterOwner.GetPlayerId() == PlayerId)
                                {
                                    myMonsters.Add(thisMonster);
                                }
                            }
                        }
                        InstructionUI.text = "Player " + PlayerId + " Turn";
                        if (networkPlayer)
                        {
                            networkPlayer.Send_ChangeToAction();
                        }
                        world.UnHighLightAll();
                        ChangeState(PlayerStates.Action_Phase);
                        break;
                    }
                    else
                    {
                        InstructionUI.text = "Choose A Card";
                        if (myCardDataBase)
                        {
                            ContentUIManager.DisplayCard();
                        }
                    }
                    break;
                case (PlayerStates.Action_Phase):
                    // InstructionUI.text = "Your Turn";
                    break;
                case (PlayerStates.Move_Phase):
                    break;
                case (PlayerStates.Moved_Phase):
                    break;
                case (PlayerStates.Attack_Phase):
                    ChangeState(PlayerStates.ChooseTarget_Phase);
                    break;
                case (PlayerStates.ChooseTarget_Phase):
                    break;
                case (PlayerStates.ConfirmSpawnPosition_Phase): // Wait For Click on Hex
                    //Debug.Log("PlayerStates=ConfirmSpawnPosition_Phase");
                    InstructionUI.text = "Choose Spawn Position";
                    break;
                case (PlayerStates.Spawn_Phase):
                    //Debug.Log("PlayerStates=Spawn_Phase");
                    InstructionUI.text = PlayedCardName + ":" + targetHexId.ToString();
                    Debug.Log("Spawn A Charcter Name:" + PlayedCardName);
                    if (myCardDataBase)
                    {
                        NewCard myCard = myCardDataBase.GetCard(PlayedCardName);
                        PlayedCard(PlayerId, myCard.id, targetHexId);
                        if (myCard.id >= 6)
                        {
                            TCardUnchosen--;
                        }
                        else
                        {
                            MCardUnchosen--;
                        }
                    }
                    ChangeState(PlayerStates.End);
                    break;
                case (PlayerStates.Idle_Phase):
                    ChangeState(PlayerStates.Main_Phase);
                    break;
                case (PlayerStates.End):
                    //Return the game control loop to main logic
                    //Debug.Log("PlayerStates=End");
                    if (nowTime - stateBeginTime > 1)
                    {
                        PlayerEnd(PlayerId);
                        ChangeState(PlayerStates.WaitForStart);
                        InstructionUI.text = "Wait For Your Turn";
                    }
                    break;
                default:
                    break;
            }
        }

        private void ChangeState(PlayerStates dstStates)
        {
            //Debug.LogFormat("PlayerStates: "+dstStates);
            myState = dstStates;
            stateBeginTime = Time.time;
        }

        #region Event Handler
        /// <summary>
        /// Call this function when Event_PlayerTurnStart invoke
        /// Change User state from WaitForStart to Main_Phase
        /// </summary>
        void PlayerTurnStartInvoke()
        {
            if (myState == PlayerStates.WaitForStart)
            {
                numOfMonsterCouldUse = _numOfMonsterCouldUse;
                numOfTerrainCouldUse = _numOfTerrainCouldUse;
                if (myMonsters.Count == _numOfMonsterCouldUse)
                {
                    foreach (Monster m in myMonsters)
                    {
                        m.isIdle = false;
                        if (!m.isAlive || m.isExiled)
                        {
                            numOfMonsterCouldUse--;
                        }
                    }
                }
                ChangeState(PlayerStates.Main_Phase);
            }
        }

        /// <summary>
        /// Call this function when Event_ScanFished invoke
        /// Change User state from Init to WaitForStart
        /// </summary>
        void ScanFinishedInvoke()
        {
            if (myState == PlayerStates.Init)
            {
                ChangeState(PlayerStates.WaitForStart);
                InstructionUI.text = "Wait For Your Turn";
            }
        }

        /// <summary>
        /// This function will call when click on a Card
        /// </summary>
        /// <param name="CardName">The Chosen Card Name</param>
        private void ClickOnCardInvoked(string CardName)
        {
            if (networkPlayer)
            {
                networkPlayer.Send_ClickOnCard(CardName);
            }

            if (myState == PlayerStates.Main_Phase)
            {
                PlayedCardName = CardName;
                ContentUIManager.ClearContentUI();
                InstructionUI.text = "Place it!";
                if (_isFirstChosenHex)
                {
                    AudioManager._instance.Play("ChooseHex");
                    _isFirstChosenHex = false;
                }
                Debug.Log("Now Player want to use " + PlayedCardName);
                ChangeState(PlayerStates.ConfirmSpawnPosition_Phase);
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a Hex
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnHexInvoked(int HexTileID)
        {
            if (networkPlayer)
            {
                networkPlayer.Send_ClickOnHex(HexTileID);
            }

            if (myState == PlayerStates.ConfirmSpawnPosition_Phase)
            {
                targetHexId = HexTileID;
                ChangeState(PlayerStates.Spawn_Phase);
            }

            if (myState == PlayerStates.Move_Phase)
            {
                if(_isFirstMove)
                {
                    AudioManager._instance.Play("AfterMoving");
                    _isFirstMove = false;
                }

                if (currMonster.CanReach(HexTileID))
                {
                    MoveDelegate(HexTileID);
                    ChangeState(PlayerStates.Moved_Phase);
                }
                else
                {

                }
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a monster
        /// </summary>
        /// <param name="HexTileID">The Chosen Hex ID</param>
        private void ClickOnMonsterInvoked(Monster clickedMonster)
        {
            if (networkPlayer)
            {
                //Transform Monster To MonsterUID
                networkPlayer.Send_ClickOnMonster(clickedMonster.GetUId());
            }

            //choose an action
            if (myState == PlayerStates.Action_Phase)
            {
                if(_isFirstClickOnCharacter)
                {
                    AudioManager._instance.Play("AfterClickCharacter");
                    _isFirstClickOnCharacter = false;
                }

                if(currMonster)
                    if(currMonster.gameObject.GetComponent<ClickableCharacter>())
                        currMonster.gameObject.GetComponent<ClickableCharacter>().Highlighted(ClickableCharacter.CharHightLightStatus.DEFAULT);

                currMonster = clickedMonster;

                if(currMonster)
                    if(currMonster.gameObject.GetComponent<ClickableCharacter>())
                        currMonster.gameObject.GetComponent<ClickableCharacter>().Highlighted(ClickableCharacter.CharHightLightStatus.SELECTED);

                if ((!currMonster.isIdle) && currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    ContentUIManager.ShowActionBtn();
                    InstructionUI.text = currMonster.monsterName;
                    world.HighLightMovementZone(currMonster);
                    currMonster.OnSelected();
                    ChangeState(PlayerStates.Move_Phase);
                }
                else
                {
                    InstructionUI.text = currMonster.monsterName;
                    ContentUIManager.HideActionBtn();
                }
            }

            //confirm your attack target
            if (myState == PlayerStates.ChooseTarget_Phase)
            {
                targetMonster = clickedMonster;
                //TODO check attack range
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    AttackDelegate(PlayerId, currMonster, targetMonster);
                }
                else
                {
                    return;
                }
                //Should go to next monster
                world.UnHighLightAll();
                ChangeState(PlayerStates.Idle_Phase);
            }
            else if(myState==PlayerStates.Double_Attack_1_Phase)
            {
                targetMonster = clickedMonster;
                //TODO check attack range
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    AttackDelegate(PlayerId, currMonster, targetMonster);
                }
                else
                {
                    return;
                }
                ChangeState(PlayerStates.Double_Attack_2_Phase);
            }
            else if (myState == PlayerStates.Double_Attack_2_Phase)
            {
                targetMonster = clickedMonster;
                //TODO check attack range
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    AttackDelegate(PlayerId, currMonster, targetMonster);
                }
                else
                {
                    return;
                }
                world.UnHighLightAll();
                ChangeState(PlayerStates.Idle_Phase);
            }
            else if(myState==PlayerStates.AOE_Phase)
            {
                targetMonster = clickedMonster;
                if (targetMonster.isAlive && targetMonster.monsterOwner.GetPlayerId() != PlayerId)
                {
                    currMonster.MonsterStateUpdate.Invoke("AOE", targetMonster.GetUId());
                }
                else
                {
                    return;
                }
                world.UnHighLightAll();
                ChangeState(PlayerStates.Idle_Phase);
            }
            return;
        }

        /// <summary>
        /// This function will call when click on a button
        /// </summary>
        /// <param name="btnName"></param>
        private void ClickOnBtnInvoked(string btnName)
        {
            if (networkPlayer)
            {
                networkPlayer.Send_ClickOnBtn(btnName);
            }

            if (myState == PlayerStates.Move_Phase || myState == PlayerStates.Moved_Phase)
            {
                if (currMonster.isAlive && currMonster.monsterOwner.GetPlayerId() == PlayerId)
                {
                    switch (btnName)
                    {
                        case ("AttackBtn"):
                            InstructionUI.text = "Choose Attack Target";
                            numOfMonsterCouldUse--;
                            currMonster.isIdle = true;
                            ContentUIManager.HideActionBtn();
                            world.HighLightAttackZone(currMonster);
                            ChangeState(PlayerStates.Attack_Phase);
                            break;
                        case ("SkillBtn"):
                            if (currMonster.canSkill == false)
                                break;//Still Cool Down
                            if(currMonster is Maria_Brute)
                            {
                                //var currMaria_Brute = (Maria_Brute)currMonster;
                                currMonster.MonsterStateUpdate.Invoke("Defense", 0);
                                numOfMonsterCouldUse--;
                                currMonster.isIdle = true;
                                ContentUIManager.HideActionBtn();
                                ChangeState(PlayerStates.Idle_Phase);
                            }
                            else if(currMonster is Erica_Surviver) //Double_Attack
                            {
                                InstructionUI.text = "Choose 2 Attack Targets";
                                currMonster.MonsterStateUpdate.Invoke("DoubleAttack", 0);
                                numOfMonsterCouldUse--;
                                currMonster.isIdle = true;
                                ContentUIManager.HideActionBtn();
                                world.HighLightAttackZone(currMonster);
                                ChangeState(PlayerStates.Double_Attack_1_Phase);
                                break;
                            }
                            else if(currMonster is Jolleen_Knight)//AOE
                            {
                                InstructionUI.text = "Choose Skill Target";
                                numOfMonsterCouldUse--;
                                currMonster.isIdle = true;
                                ContentUIManager.HideActionBtn();
                                world.HighLightAttackZone(currMonster);
                                ChangeState(PlayerStates.AOE_Phase);
                                break;
                            }
                            // InstructionUI.text = "Choose Skill Target";
                            // numOfMonsterCouldUse--;
                            // currMonster.isIdle = true;
                            // ContentUIManager.HideActionBtn();
                            break;
                        case ("IdleBtn"):

                            numOfMonsterCouldUse--;
                            currMonster.isIdle = true;
                            ContentUIManager.HideActionBtn();
                            ChangeState(PlayerStates.Idle_Phase);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.Log("Current Monster doesn't match UI!");
                }
            }
            return;
        }
        /// <summary>
        /// Invoke when Player turn End
        /// </summary>
        /// <param name="PlayerId">The Player Index</param>
        private void PlayerEndInvoked(int PlayerId)
        {
            if(networkPlayer)
            {
                networkPlayer.Send_PlayerTurnEnd(PlayerId);
            }
            return;
        }

        #endregion

        #region Delegate Handler
        /// <summary>
        /// Invoked when the player is playing card
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="CardIndex">The played card index</param>
        /// <param name="HexIndex">The hex that card has been place on</param>
        private void PlayedCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            Debug.LogFormat("Player {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            if(_isFirstPlayCard)
            {
                AudioManager._instance.Play("FirstPlayCard");
                _isFirstPlayCard = false;
            }
            
            InstructionUI.text = "PlayedCard";
            if(networkPlayer)
            {
                networkPlayer.Send_PlayCard(PlayerId, CardIndex, HexIndex);
            }
            return;
        }

        /// <summary>
        /// Invoked when the player is attacking
        /// </summary>
        /// <param name="PlayerId">The Player Id of the played card.</param>
        /// <param name="currMonster">Controlled Monster</param>
        /// <param name="targetMonster">Target Monster</param>
        private void AttackInvoked(int PlayerId, Monster currMonster, Monster targetMonster)
        {
            Debug.LogFormat("Player {0} use {1} attack {2}", PlayerId, currMonster, targetMonster);
            currMonster.Attack(targetMonster, currMonster.ATK);
            return;
        }

        /// <summary>
        /// Invoked when move
        /// </summary>
        /// <param name="destination">The HexTile Id of destination.</param>
        private void MoveInvoked(int destination)
        {
            List<int> path = new List<int>();
            path = world.tileMap.getShortestPath(currMonster.HexIndex,destination);
            currMonster.Move(path);
            return;
        }
        #endregion
    }
}


