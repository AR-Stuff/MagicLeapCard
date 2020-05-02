using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using GameWorld;
using Monsters;

namespace GameLogic
{
    /// <summary>
    /// PlayerStates are states for the Finited State Machine
    /// </summary>
    public enum PlayerStates
    {
        Init,
        WaitForStart,
        ImageTrackingStart,
        Main_Phase,
        ConfirmSpawnPosition_Phase,
        Spawn_Phase,
        Action_Phase,
        Move_Phase,
        Moved_Phase,
        Attack_Phase,
        ChooseTarget_Phase,
        Double_Attack_1_Phase,
        Double_Attack_2_Phase,
        AOE_Phase,
        Skill_Phase,
        Idle_Phase,
        ImageTrackingStop,
        End,
        Error
    };

    /// <summary>
    /// When this player is end its turn, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="id">
    /// The playerid, should be unique for each Player
    /// </param>
    public delegate void PlayerEnd(int id);

    /// <summary>
    /// When this player is playing a card, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="PlayerId">The Player Id of the card.</param>
    /// <param name="CardIndex">The Card Index of the played card.</param>
    /// <param name="HexIndex">The Hex Index of the player camera.</param>
    public delegate void PlayedCard(int PlayerId, int CardIndex, int HexIndex);

    /// <summary>
    /// When this player click Attack Btn, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="PlayerId">The Player Id of the card.</param>
    /// <param name="CardIndex">The Card Index of the played card.</param>
    /// <param name="HexIndex">The Hex Index of the player camera.</param>
    public delegate void AttackDelegate(int PlayerId, Monster currMonster, Monster targetMonster);

    /// <summary>
    /// When this player want to move monster, it will pop a delegate to Main Logic
    /// </summary>
    /// <param name="destination">The HexTile Id of the destination.</param>
    public delegate void MoveDelegate(int destination);

    /// <summary>
    /// When the play ended its battle turn, this delegate should be activated.
    /// </summary>
    /// 
    public delegate void Battle_PlayerEnd();

    public class Player : MonoBehaviour
    {

        #region Event Define
        ///<summary>
        /// The MainLogic will give a event to the player when scan finished.
        ///</summary>
        public UnityEvent Event_ScanFinished;
        /// <summary>
        /// The MainLogic will give a event to the player when player turn start.
        /// </summary>
        public UnityEvent Event_PlayerTurnStart;
        public UnityEvent Event_Battle_PlayerTurnStart;
        #endregion

        #region Public Delegate
        /// <summary>
        /// Define the interface for delegate PlayerEnd
        /// </summary>
        public PlayerEnd PlayerEnd;

        /// <summary>
        /// Define the interface for delegate Attack
        /// </summary>
        public AttackDelegate AttackDelegate;

        /// <summary>
        /// Define the interface for delegate Move
        /// </summary>
        public MoveDelegate MoveDelegate;

        /// <summary>
        /// Define the interface for delegate Attack
        /// </summary>
        public PlayedCard PlayedCard;

        public Battle_PlayerEnd Battle_PlayerEnd;
        #endregion

        #region Public Property
        /// <summary>
        /// Player Identity, should be unique for each player
        /// </summary>
        public int PlayerId;

        public World world;
        #endregion

        public int GetPlayerId() { return PlayerId; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
