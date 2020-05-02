using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameWorld;
using TerrainScanning;
using CardInfo;
using Monsters;
using UI;

namespace GameLogic
{
    
    public enum GameStates
    {
        Init,
        Wait_For_Mode_Selection,
        Wait_For_Map_Scan,
        Online_Only_Wait_For_Connection,
        Game_Begin,
        ChooseFirstPlayer,
        UpKeepStep,
        Turn_Begin,
        Player_Turn_Begin,
        Wait_For_Player_Turn,
        Battle_Begin,
        Wait_For_Battle, 
        Local_Player_Win,
        Local_Player_Lose,
        Game_Draw,
        Stop,
        Battle_End,
        Turn_End,
        Game_End,
        Error
    };

    public enum GameMode
    {
        Offline_Mode,
        Online_Mode
    }

    public class GameGlobalState
    {
        public int matchCnt;
        public int[] playerWinCnt = new int[2];
        public int lastWinner;
        public void Reset()
        {
            matchCnt = 0;
            playerWinCnt[0] = playerWinCnt[1] = 0;
        }
    }

    public delegate void GameBegin();
    public delegate void DrawCardBegin();
    public delegate void Player0Begin();
    public delegate void Player1Begin();
    public delegate void BattleBegin();

    public class GameManager : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// Store a Pointer to ScanMesh to capture the ScanFinished event
        /// </summary>
        public meshCode ScanMesh;
        public GameObject ScanMeshObject;

        /// <summary>
        /// Store a Pointer to Player0 to send playerstart event and scanfinished event
        /// </summary>
        public Player Player0;
        public Player Player1;

        /// <summary>
        /// The Game World
        /// </summary>
        public World world;

        /// <summary>
        /// The global states(attributes) of a single game, including howmany turns it has passed, howmany times each player wins, e.t.c.
        /// </summary>
        public GameGlobalState gameGlobalState;

        /// <summary>
        /// Limit Player time per turn.
        /// </summary>
        [Tooltip("Limit Player time per turn.")]
        public float PlayerTurnTimeLimited = 30;

        /// <summary>
        /// A link to the Card Database
        /// </summary>
        [Tooltip("A link to the Card Database")]
        public NewCardDB CardDataBase;

        /// <summary>
        /// A Reference to Multiplayer GameManager
        /// </summary>
        public MultiplayerGameManager multiplayerGameManager;

        /// <summary>
        /// Public Interface to get the GameState
        /// </summary>
        public GameStates CurrentState { get { return currentState; } }
        #endregion

        #region Private Variable
        /// <summary>
        /// Store the time when this state began
        /// </summary>
        private float stateBeginTime;

        private List<Player> playerTurnList;
        private int currentPlayerId;

        private GameStates currentState = GameStates.Init;

        private GameMode currentMode;

        //OpponentMovement: all data needed to complete a movement
        //e.g. An attack:
        //OpponentMovement.Attacker
        //OpponentMovement.Target
        //OpponentMovement.Damage
        private object OpponentMovement;

        /// <summary>
        /// Boolean true for Player0 go first, false for Player1 go first
        /// </summary>
        private bool bPlayer0First = true;

        /// <summary>
        /// Counter how many turns
        /// </summary>
        public int TurnCounter = 0;

        [SerializeField]
        private ContentUIManager ContentUIManager;
        #endregion

        private void ChangeState(GameStates dstStates)
        {
            Debug.LogFormat("Game Manager: Now Change to "+dstStates);
            currentState = dstStates;
            stateBeginTime = Time.time;
        } 

        // Start is called before the first frame update
        void Start()
        {
            gameGlobalState = new GameGlobalState();
            playerTurnList = new List<Player>() ;

            ScanMesh.ScanFinished += ScanFinshedUpdate;

            Player0.PlayerEnd += PlayerEndUpdate;
            Player0.PlayedCard += PlayedCardInvoke;
            Player0.PlayerId = 0;
            world.uploadPlayerInWorld(Player0);

            Player1.PlayerEnd += PlayerEndUpdate;
            Player1.PlayedCard += PlayedCardInvoke;
            Player1.PlayerId = 1;
            world.uploadPlayerInWorld(Player1);

            world.BattleEnd += BattleEndUpdate;
            world.World_ResetFinished += World_ResetFinishedUpdate;
            ChangeState(GameStates.Init);
        }

        // Update is called once per frame
        void Update()
        {
            float nowTime = Time.time;
            int winner = -1;
            switch (currentState)
            {
                case GameStates.Init: //Wait Until 2Players Join the game
                    //ChangeState(GameStates.Wait_For_Mode_Selection);
                    break;
                case GameStates.Wait_For_Mode_Selection: // TODO: let user select mode.
                    currentMode = GameMode.Online_Mode;
                    ScanMeshObject.SetActive(true);
                    ScanMesh.ScanFinished += ScanFinshedUpdate;
                    ChangeState(GameStates.Wait_For_Map_Scan);
                    break;
                case GameStates.Wait_For_Map_Scan:
                    break;
                case GameStates.Online_Only_Wait_For_Connection: // TODO: add online wait for connection
                    //Debug.LogError("Error! Online mode hasn't implemented yet.");
                    //ChangeState(GameStates.Error);
                    break;
                case GameStates.Game_Begin:
                    gameGlobalState.Reset();
                    ChangeState(GameStates.ChooseFirstPlayer);
                    break;
                case GameStates.ChooseFirstPlayer:
                    bPlayer0First = multiplayerGameManager.GetLocalPlayerGoFirst();
                    if(bPlayer0First)
                    {
                        AudioManager._instance.Play("FirstPlayer");
                    }
                    else
                    {
                        AudioManager._instance.Play("SecondPlayer");
                    }
                    ChangeState(GameStates.UpKeepStep);
                    break;
                case GameStates.UpKeepStep:
                    if(nowTime-stateBeginTime>0.5)
                    {
                        world.Event_ResetStart.Invoke();
                    }
                    break;
                case GameStates.Turn_Begin:
                    WinningCheck();
                    if (currentState != GameStates.Turn_Begin)
                        break;
                    if (nowTime - stateBeginTime > 0.5)
                    {
                        if(bPlayer0First==true)
                        {
                            Debug.Log("Player0 Go First.");
                            playerTurnList.Add(Player0);
                            playerTurnList.Add(Player1);
                            //bPlayer0First = false;  //Switch Player go first next turn
                            //For manual control: Do not Switch Player go first next turn
                        }
                        else
                        {
                            Debug.Log("Player1 Go First.");
                            playerTurnList.Add(Player1);
                            playerTurnList.Add(Player0);
                            // bPlayer0First = true;   //Switch Player go first next turn
                        }
                        TurnCounter++;
                        if(TurnCounter==6)
                        {
                            if (bPlayer0First) AudioManager._instance.Play("BattleBegin_First");
                            else AudioManager._instance.Play("BattleBegin_Second");
                        }
                        ChangeState(GameStates.Player_Turn_Begin);
                    }
                    break;
                case GameStates.Player_Turn_Begin:
                    WinningCheck();
                    if (currentState != GameStates.Player_Turn_Begin)
                        break;

                    if (playerTurnList.Count == 0) // all players' turn is over. 
                        //skip auto battle
                        //ChangeState(GameStates.Battle_Begin);
                        ChangeState(GameStates.Turn_Begin);
                    if (nowTime - stateBeginTime > 1)
                    {
                        Player currentPlayer = playerTurnList[0];
                        if(TurnCounter==6 && bPlayer0First==false && currentPlayer==Player0)
                        {
                            AudioManager._instance.Play("SecondPlayerFirstBattle");
                        }
                        playerTurnList.Remove(currentPlayer);
                        currentPlayerId = currentPlayer.PlayerId;
                        currentPlayer.Event_PlayerTurnStart.Invoke();

                        ChangeState(GameStates.Wait_For_Player_Turn);
                    }
                    break ;
                case GameStates.Wait_For_Player_Turn:
                    // Do Manual Control in this state
                    if (nowTime - stateBeginTime > PlayerTurnTimeLimited) // Setting a time limit for each player. TODO: when this is triggered, a delegent should be sent to the Player.
                    {
                        Debug.LogWarningFormat("Player {0}'s time out. The system will consider it hasn't done anything.", currentPlayerId);
                        ChangeState(GameStates.Player_Turn_Begin);
                    }
                    break;
                //case GameStates.Battle_Begin:
                //    if (nowTime - stateBeginTime > 2)
                //    {
                //        world.Event_BattleBegin.Invoke();
                //        ChangeState(GameStates.Wait_For_Battle);
                //    }
                //    break;
                //case GameStates.Battle_End:
                //    gameGlobalState.matchCnt++;
                //    winner = gameGlobalState.lastWinner;
                //    if (winner == -1)
                //        Debug.LogFormat("Round {0}: Draw! Current Score: {1}:{2}", gameGlobalState.matchCnt, gameGlobalState.playerWinCnt[0], gameGlobalState.playerWinCnt[1]);
                //    else
                //    {
                //        gameGlobalState.playerWinCnt[winner]++;
                //        Debug.LogFormat("Round {0}: Player {1} wins! Current Score: {2}:{3}", gameGlobalState.matchCnt, winner, gameGlobalState.playerWinCnt[0], gameGlobalState.playerWinCnt[1]);
                //    }
                //    if ((winner >= 0 && gameGlobalState.playerWinCnt[winner] > 2) || gameGlobalState.matchCnt == 5)
                //        ChangeState(GameStates.Game_End);
                //    else
                //        ChangeState(GameStates.UpKeepStep);
                //    break;
                case GameStates.Local_Player_Win:
                    if(AudioManager._instance)
                    {
                        AudioManager._instance.StopAll();
                        AudioManager._instance.Play("Victory");
                    }
                    if(ContentUIManager)
                    {
                        if(ContentUIManager.InstructionUI)
                            ContentUIManager.InstructionUI.text = "You WIN!!!!";
                    }
                    ChangeState(GameStates.Stop);
                    break;
                case GameStates.Local_Player_Lose:
                    if (AudioManager._instance)
                    {
                        AudioManager._instance.StopAll();
                        AudioManager._instance.Play("Defeat");
                    }
                    if (ContentUIManager)
                    {
                        if (ContentUIManager.InstructionUI)
                            ContentUIManager.InstructionUI.text = "You LOSE!!!!";
                    }
                    ChangeState(GameStates.Stop);
                    break;
                case GameStates.Game_Draw:
                    if (AudioManager._instance)
                    {
                        AudioManager._instance.StopAll();
                        AudioManager._instance.Play("Victory");
                    }
                    if (ContentUIManager)
                    {
                        if (ContentUIManager.InstructionUI)
                            ContentUIManager.InstructionUI.text = "DRAW!!!!";
                    }
                    ChangeState(GameStates.Stop);
                    break;
                case GameStates.Stop:
                    break;
                case GameStates.Game_End:
                    if (gameGlobalState.playerWinCnt[0] != gameGlobalState.playerWinCnt[1])
                        winner = (gameGlobalState.playerWinCnt[0] < gameGlobalState.playerWinCnt[1] ? 1 : 0);
                    else winner = -1;
                    if (winner == -1)
                        Debug.Log("Game over. Draw.");
                    else
                        Debug.LogFormat("Game over. Player {0} wins. Congrat to him!!!", winner);
                    ChangeState(GameStates.Game_Begin);// Todo: player can choose to restart a game or 
                    break;
                case GameStates.Error:
                    // TODO: some operations to get back to init.
                    if(nowTime - stateBeginTime > 10)
                    {
                        ChangeState(GameStates.Init);
                    }
                    break;
            }
        }

        void ScanFinshedUpdate()
        {
            Debug.Log("ScanFinished");
            List<int> path = new List<int>();
            Player0.Event_ScanFinished.Invoke();
            multiplayerGameManager.ScanFinishedEvent.Invoke();
            AudioManager._instance.Play("PlaceDoneFirst");

            if(currentState == GameStates.Wait_For_Map_Scan)
            {
                if (currentMode == GameMode.Offline_Mode)
                {
                    Player1.Event_ScanFinished.Invoke();
                    ChangeState(GameStates.Game_Begin);
                }
                else
                    ChangeState(GameStates.Online_Only_Wait_For_Connection);
            }

            return;
        }

        #region Public Function
        /// <summary>
        /// Set up multiplayer game manager delegate interface
        /// Invoke by MultiplayerGameManager
        /// </summary>
        public void SetUpMultiplayerInterface()
        {
            multiplayerGameManager.FinishInit += MultiplayerGameManager_FinishInitInvoked;
            multiplayerGameManager.AllPlayerFinishScanning += MultiplayerGameManager_AllPlayerFinishScanning;

        }

        /// <summary>
        /// Send Tornado Hex to server
        /// </summary>
        public void Send_TornadoHex(int MonsterID,int HexID)
        { 
            multiplayerGameManager.Send_TornadoHex(MonsterID, HexID);
        }

        public void Set_TornadoHex(int MonsterID,int HexID)
        {
            var itr=world.getMonsterByID(MonsterID);
            itr.StateUpdate("TornadoMove", HexID);
        }

        #endregion

        #region Delegate Handler
        /// <summary>
        /// When receive the FinishInit Delegate from MultiplayerGameManager
        /// Game State goto Game_Begin
        /// </summary>
        private void MultiplayerGameManager_FinishInitInvoked()
        {
            if(currentState==GameStates.Init)
            {
                ChangeState(GameStates.Wait_For_Mode_Selection);
                AudioManager._instance.Play("ScanAndPlace");
            }
            return;
        }

        /// <summary>
        /// When receive the FinishInit Delegate from MultiplayerGameManager
        /// Game State goto Game_Begin
        /// </summary>
        private void MultiplayerGameManager_AllPlayerFinishScanning()
        {
            Debug.Log("Receive all PlayerFinished");
            if (currentState == GameStates.Online_Only_Wait_For_Connection)
            {
                ChangeState(GameStates.Game_Begin);
            }
            return;
        }

        /// <summary>
        /// Invoke when Player Turn Ended.
        /// </summary>
        /// <param name="PlayerId">
        /// Input the player id.
        /// </param>
        void PlayerEndUpdate(int PlayerId)
        {
            if (currentState != GameStates.Wait_For_Player_Turn || currentPlayerId != PlayerId)
            {
                Debug.LogWarningFormat("Sorry player {0}, this is not your turn yet.", PlayerId);
                return;
            }
            else
            {
                Debug.LogFormat("Player {0} has finished it's turn.", PlayerId);
                ChangeState(GameStates.Player_Turn_Begin);
            }
        }
        /// <summary>
        /// post-process of a battle.
        /// </summary>
        /// <param name="winner">
        /// the winner of this battle. winner = -1 if there's a draw. 
        /// </param>
        void BattleEndUpdate(int winner)
        {
            gameGlobalState.lastWinner = winner;
            ChangeState(GameStates.Battle_End);
        }
        /// <summary>
        /// Update game states after world is finished
        /// </summary>
        private void World_ResetFinishedUpdate()
        {
            Debug.Log("WorldResetFinished");
            ChangeState(GameStates.Turn_Begin);
        }

        /// <summary>
        /// Deal the monster spwan when player played card.
        /// Also attach Monster class to the spawn monster
        /// </summary>
        /// <param name="PlayerId">The player id of the played card.</param>
        /// <param name="CardIndex">The played card index.</param>
        /// <param name="HexIndex">The hex index of that played card.</param>
        private void PlayedCardInvoke(int PlayerId,int CardIndex,int HexIndex)
        {
            Debug.LogFormat("AIPlayer {0} playing card {1} in hex {2}", PlayerId, CardIndex, HexIndex);
            NewCard thisCard = CardDataBase.GetCardByIndex(CardIndex);
            HexTile TargetHex = world.tileMap.getHexTileByIndex(HexIndex);
            if (thisCard.isMonster)
            {
                GameObject newMonster = Instantiate(thisCard.CardPrefab, TargetHex.transform);
                if (newMonster.GetComponent<Monster>() == null)
                    newMonster.AddComponent<Monster>();
                newMonster.GetComponent<Monster>().world = world;
                newMonster.GetComponent<Monster>().MonsterInit(thisCard.CardName, thisCard.HP, thisCard.Attack, thisCard.Speed, (PlayerId == Player0.PlayerId ? Player0 : Player1), HexIndex);
                world.uploadMonsterInWorld(newMonster.GetComponent<Monster>());
            }
            else
            {
                GameObject newTerrain = Instantiate(thisCard.CardPrefab, TargetHex.transform);
                if(newTerrain.GetComponent<InteractiveTerrain>()==null)
                {
                    newTerrain.AddComponent<InteractiveTerrain>();
                }
                newTerrain.GetComponent<InteractiveTerrain>().World = world;
                List<int> Affective = new List<int>();
                Affective.Add(TargetHex.getID());
                if(thisCard.CardName!="Portal")
                {
                    var temp=world.tileMap.GetAllSurroundHexIndex(HexIndex);
                    foreach(var itr in temp)
                    {
                        Affective.Add(itr);
                    }
                }
                else
                {
                     int CentralIndex = 91;
                     Affective.Add(CentralIndex);
                }
                newTerrain.GetComponent<InteractiveTerrain>().TerrainCardInit(thisCard.CardName,Affective);
                world.uploadTerrainInWorld(newTerrain.GetComponent<InteractiveTerrain>());
            }

            //GameObject monsterClass = Resources.Load<GameObject>(thisCard.PrefabPath);
            
        }

        /// <summary>
        /// Check Winning, will change state if one side win
        /// </summary>
        private void WinningCheck()
        {
            if(TurnCounter <= 6) return;
            int Player0Count = 0;
            int Player1Count = 0;
            foreach (var itr in world.monsters)
            {
                if(itr.Value.isAlive)
                {
                    if(itr.Value.monsterOwner.PlayerId==Player0.PlayerId)
                    {
                        Player0Count++;
                    }
                    else
                    {
                        Player1Count++;
                    }
                }
            }
            
            if(Player0Count>0 && Player1Count>0)
            {//ON GOING
                return;
            }
            else if(Player0Count<=0 && Player1Count<=0)
            {//DRAW
                ChangeState(GameStates.Game_Draw);
                return;
            }
            else if(Player0Count<=0)
            {
                ChangeState(GameStates.Local_Player_Lose);
                return;
            }
            else
            {
                ChangeState(GameStates.Local_Player_Win);
                return;
            }
            return;
        }
        #endregion
    }

}