using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    /// <summary>
    /// Event Invoke when Local Player begin
    /// </summary>
    public delegate void LocalPlayerBegin();
    /// <summary>
    /// Event Invoke when Remote Player begin
    /// </summary>
    public delegate void RemotePlayerBegin();
    /// <summary>
    /// Event Invoke when there are 2 Players
    /// And the game can be start
    /// </summary>
    public delegate void FinishInit();
    /// <summary>
    /// Event Invoke when more 2 players finished scanning
    /// The game can be go on
    /// </summary>
    public delegate void AllPlayerFinishScanning();

    public class MultiplayerGameManager : Bolt.EntityEventListener<IGameManagerState>
    {
        #region Public Variable
        /// <summary>
        /// Boolean Variable that define which player go first
        /// </summary>
        public bool isServerFirst { get { return state.isServerFirst; } }
        #endregion

        #region Private Variable
        /// <summary>
        /// Private relevaten to this world Game Manager
        /// </summary>
        [SerializeField]
        private GameManager gameManager;
        #endregion

        #region Delegate Define
        /// <summary>
        /// Event Invoke when there are 2 Players
        /// And the game can be start
        /// </summary>
        public FinishInit FinishInit;
        
        /// <summary>
        /// Event Invoke when Local Player begin
        /// </summary>
        public LocalPlayerBegin ServerPlayerBegin;
        /// <summary>
        /// Event Invoke when Remote Player begin
        /// </summary>
        public RemotePlayerBegin ClientPlayerBegin;
        /// <summary>
        /// Event Invoke when more 2 players finished scanning
        /// The game can be go on
        /// </summary>
        public AllPlayerFinishScanning AllPlayerFinishScanning;
        /// <summary>
        /// Receive Event ScanFinished From GameManager
        /// </summary>
        public UnityEvent ScanFinishedEvent;
        #endregion

        #region Public Function
        /// <summary>
        /// Just Like Monobehavior.Start()
        /// Invoked when Bolt is aware of this entity and all internal state has been setup
        /// Bind the GameManager
        /// Choose the first Player
        /// Calculate the NUmberof Player
        /// </summary>
        public override void Attached()
        {
            base.Attached();
            if(gameManager==null)
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.multiplayerGameManager = this;
                gameManager.SetUpMultiplayerInterface();
            }
            if(entity.IsOwner)
            {
                state.isServerFirst = (Random.value < 0.5);
                state.NumberOfPlayer = 1;
                state.NumberOfPlayerFinishedScan = 0;
            }
            else
            {
                Send_LoginEvent();
            }
        }
        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when this class is init
        /// </summary>
        private void Start()
        {
            if(ScanFinishedEvent==null)
            {
                ScanFinishedEvent = new UnityEvent();
            }
            ScanFinishedEvent.AddListener(ScanFinishedEvent_Invoked);
        }
        // Update is called once per frame
        void Update()
        {
            if(gameManager)
            {
                if(gameManager.CurrentState==GameStates.Init && state.NumberOfPlayer>=2)
                {
                    this.FinishInit();
                }
                if(gameManager.CurrentState==GameStates.Online_Only_Wait_For_Connection && state.NumberOfPlayerFinishedScan>=2)
                {
                    this.AllPlayerFinishScanning();
                }
            }
        }
        #endregion

        #region Public Function
        /// <summary>
        /// Get wether Player0 or Player1 should go first
        /// Player0 is the local player in each device
        /// Player1 is the remote player in each device
        /// This function will respond different result in different device
        /// </summary>
        /// <returns>Return true if Player0 go first</returns>
        public bool GetLocalPlayerGoFirst()
        {
            if(state.isServerFirst && BoltNetwork.IsServer)
            {
                return true;
            }
            if(!state.isServerFirst && BoltNetwork.IsClient)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Send Login event to server
        /// </summary>
        public void Send_LoginEvent()
        {
            var newEvnt = LoginEvent.Create(entity);
            newEvnt.Send();
        }

        /// <summary>
        /// Send FinishScan to Server
        /// </summary>
        public void Send_FinishScan()
        {
            if(entity.IsOwner)
            {
                var newEvnt = FinishScan.Create(entity);
                newEvnt.Send();
            }
        }

        /// <summary>
        /// Send TornadoHex to Server and Client
        /// </summary>
        public void Send_TornadoHex(int MonsterId,int HexId)
        {
            var newEvnt = TornadoHex.Create(entity);
            newEvnt.MonsterID = MonsterId;
            newEvnt.HexID = HexId;
            newEvnt.Send();
        }
        #endregion

        #region Event Handle
        /// <summary>
        /// Update the Number Of Player finished scan when receive this event
        /// </summary>
        private void ScanFinishedEvent_Invoked()
        {
            Send_FinishScan();
        }

        public override void OnEvent(LoginEvent evnt)
        {
            base.OnEvent(evnt);
            if(entity.IsOwner)
            {
                state.NumberOfPlayer++;
            }
        }

        public override void OnEvent(FinishScan evnt)
        {
            base.OnEvent(evnt);
            if(entity.IsOwner)
            {
                state.NumberOfPlayerFinishedScan++;
            }
        }

        public override void OnEvent(TornadoHex evnt)
        {
            base.OnEvent(evnt);
            gameManager.Set_TornadoHex(evnt.MonsterID, evnt.HexID);
        }
        #endregion
    }
}

