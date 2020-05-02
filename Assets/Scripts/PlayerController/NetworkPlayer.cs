using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Project_Network;

namespace GameLogic
{
    /// <summary>
    /// Invoke when receive a Network ClickOnHex Event
    /// </summary>
    /// <param name="HexID">The chosen hex ID.</param>
    public delegate void Network_ClickOnHex(int HexID);
    /// <summary>
    /// Invoke when receive a Network ClickOnCard Event
    /// </summary>
    /// <param name="CardName">The chosen card name.</param>
    public delegate void Network_ClickOnCard(string CardName);
    /// <summary>
    /// Invoke when receive a Player Played Card Event
    /// </summary>
    /// <param name="PlayerId">Player Id</param>
    /// <param name="CardIndex">Card Id</param>
    /// <param name="HexIndex">HexTile Id</param>
    public delegate void Network_PlayCard(int PlayerId, int CardIndex, int HexIndex);
    /// <summary>
    /// Invoke when receive a Network PlayerTurnEnd Event
    /// </summary>
    /// <param name="PlayerId">The Index of the comming player</param>
    public delegate void Network_PlayerTurnEnd(int PlayerId);
    /// <summary>
    /// Invoke when receive a Network ChangeToAction Event
    /// </summary>
    public delegate void Network_ChangeToAction();
    /// <summary>
    /// Invoke when receive a Network Click on Button Event
    /// </summary>
    /// <param name="BtnName">The Name of the button</param>
    public delegate void Network_ClickOnBtn(string BtnName);
    /// <summary>
    /// Invoke when receive a Network Click on Monster Event
    /// Need to tranfrom Monster and MonsterId
    /// </summary>
    /// <param name="MonsterId">The uid of the monster</param>
    public delegate void Network_ClickOnMonster(int MonsterId);

    /// <summary>
    /// This Player is replicated from server
    /// Only deal with replicated event, Don't deal with real function
    /// </summary>
    public class NetworkPlayer : EntityEventListener<INetworkPlayer>
    {
        #region Private Reference
        /// <summary>
        /// Private Reference of the playerGameobject
        /// All real function should be call by GameObject
        /// </summary>
        private Player PlayerGameObject;
        /// <summary>
        /// _enableDebugLog = true will print out more information
        /// </summary>
        private bool _enableDebugLog = true;
        #endregion
        #region Event Definition
        /// <summary>
        /// Invoke when receive a Network ClickOnHex Event
        /// </summary>
        public Network_ClickOnCard Network_ClickOnCard;
        /// <summary>
        /// Invoke when receive a Network ClickOnCard Event
        /// </summary>
        public Network_ClickOnHex Network_ClickOnHex;
        /// <summary>
        /// Invoke when receive a Network PlayCard Event
        /// </summary>
        public Network_PlayCard Network_PlayCard;
        /// <summary>
        /// Invoke when receive a Network PlayerTurnEnd Event
        /// </summary>
        public Network_PlayerTurnEnd Network_PlayerTurnEnd;
        /// <summary>
        /// Invoke when receive a Network ChangeToAction Event
        /// </summary>
        public Network_ChangeToAction Network_ChangeToAction;
        /// <summary>
        /// Invoke when receive a Network Click on Button Event
        /// </summary>
        public Network_ClickOnBtn Network_ClickOnBtn;
        /// <summary>
        /// Invoke when receive a Network Click on Monster Event
        /// Need to tranfrom Monster and MonsterId
        /// </summary>
        public Network_ClickOnMonster Network_ClickOnMonster;
        #endregion

        #region Unity Function
        /// <summary>
        /// Just Like Monobehavior.Start()
        /// Invoked when Bolt is aware of this entity and all internal state has been setup
        /// Bind the PlayerGameObject
        /// </summary>
        public override void Attached()
        {
            base.Attached();

            if (entity.IsOwner)
            {
                if(BoltNetwork.IsServer)
                {
                    state.PlayerId = 0;
                }
                else
                {
                    state.PlayerId = 1;
                }
                PlayerGameObject = GameObject.Find("HumanPlayer").GetComponent<HumanPlayer>();
                HumanPlayer human = PlayerGameObject as HumanPlayer;
                human.networkPlayer = this;
                
            }
            else
            {
                PlayerGameObject = GameObject.Find("RemotePlayer").GetComponent<RemotePlayer>();
                RemotePlayer remote = PlayerGameObject as RemotePlayer;
                remote.networkPlayer = this;
                remote.NetworkSetUp();
            }

            state.AddCallback("PlayerId", PlayerIdChange);
        }

        /// <summary>
        /// Unity Start Function
        /// Init the Delegate callback
        /// </summary>
        private void Start()
        {
            Network_ClickOnCard += Network_ClickonCardInvoked;
            Network_ClickOnHex += Network_ClickonHexInvoked;
            Network_PlayCard += Network_PlayCardInvoked;
            Network_PlayerTurnEnd += Network_PlayerTurnEndInvoked;
            Network_ClickOnMonster += Network_ClickOnMonsterInvoked;
            Network_ClickOnBtn += Network_ClickOnBtnInvoked;
            Network_ChangeToAction += Network_ChangeToActionInvoked;
        }

        /// <summary>
        /// Unity Update Function
        /// </summary>
        private void Update()
        {
            PlayerGameObject.PlayerId = state.PlayerId;
        }
        #endregion

        #region Event Callback
        /// <summary>
        /// Set the Real gameobject Player Id, 0 is server, 1 is client
        /// </summary>
        private void PlayerIdChange()
        {
            PlayerGameObject.PlayerId = state.PlayerId;
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnCard evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnCard(evnt.CardName);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnHex evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnHex(evnt.HexTileID);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(PlayCard evnt)
        {
            base.OnEvent(evnt);
            if (evnt.PlayerId != state.PlayerId) return;
            this.Network_PlayCard(evnt.PlayerId, evnt.CardIndex, evnt.HexIndex);
        }
        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(PlayerTurnEnd evnt)
        {
            base.OnEvent(evnt);
            if (evnt.PlayerId != state.PlayerId) return;
            this.Network_PlayerTurnEnd(evnt.PlayerId);
        }

        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ChangeToAction evnt)
        {
            base.OnEvent(evnt);
            this.Network_ChangeToAction();
        }

        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnBtn evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnBtn(evnt.BtnName);
        }

        /// <summary>
        /// Receive Network event from remote player and sent to local player
        /// </summary>
        /// <param name="evnt">Event Class</param>
        public override void OnEvent(ClickOnMonster evnt)
        {
            base.OnEvent(evnt);
            this.Network_ClickOnMonster(evnt.MonsterId);
        }
        #endregion

        #region Event Sender
        /// <summary>
        /// Send ClickOnCard Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="CardName">The Name of Card</param>
        public void Send_ClickOnCard(string CardName)
        {
            if(entity.IsOwner)
            {
                var newEvent = ClickOnCard.Create(entity);
                newEvent.CardName = CardName;
                newEvent.Send();
            }
        }

        /// <summary>
        /// Send ClickOnHex Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="HexTileID">HexTile Index</param>
        public void Send_ClickOnHex(int HexTileID)
        {
            if(entity.IsOwner)
            {
                var newEvent = ClickOnHex.Create(entity);
                newEvent.HexTileID = HexTileID;
                newEvent.Send();
            }
        }

        /// <summary>
        /// Send PlayCard Event to other player
        /// Only Owner allow to send this event
        /// </summary>
        /// <param name="PlayerId">Player Index, Integer</param>
        /// <param name="CardIndex">Card Index, Interger</param>
        /// <param name="HexIndex">Hex Index, Interger</param>
        public void Send_PlayCard(int PlayerId, int CardIndex, int HexIndex)
        {
            if(entity.IsOwner)
            {
                var newEvent = PlayCard.Create(entity);
                newEvent.PlayerId = PlayerId;
                newEvent.CardIndex = CardIndex;
                newEvent.HexIndex = HexIndex;
                newEvent.Send();
            }
        }

        /// <summary>
        /// Send PlayerTurnEnd Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="PlayerId"></param>
        public void Send_PlayerTurnEnd(int PlayerId)
        {
            if(entity.IsOwner)
            {
                var newEvent = PlayerTurnEnd.Create(entity);
                newEvent.PlayerId = PlayerId;
                newEvent.Send();
            }
        }

        /// <summary>
        /// Send ChangeToAction to other player
        /// Only Owner allow to send event
        /// </summary>
        public void Send_ChangeToAction()
        {
            if (entity.IsOwner)
            {
                var newEvnt = ChangeToAction.Create(entity);
                newEvnt.Send();
            }
        }

        /// <summary>
        /// Send Click On Monster to other player
        /// Only Owner allow to send event
        /// Need to transform Monster to MonsterId before send
        /// </summary>
        /// <param name="MonsterId">Monster UID</param>
        public void Send_ClickOnMonster(int MonsterId)
        {
            if (entity.IsOwner)
            {
                var newEvnt = ClickOnMonster.Create(entity);
                newEvnt.MonsterId = MonsterId;
                newEvnt.Send();
            }
        }

        /// <summary>
        /// Send Click On Button Event to other player
        /// Only Owner allow to send event
        /// </summary>
        /// <param name="BtnName">Button Name</param>
        public void Send_ClickOnBtn(string BtnName)
        {
            if (entity.IsOwner)
            {
                var newEvnt = ClickOnBtn.Create(entity);
                newEvnt.BtnName = BtnName;
                newEvnt.Send();
            }
        }
        #endregion

        #region Event Handle
        /// <summary>
        /// Private Handle the ClickOnCard Event
        /// </summary>
        private void Network_ClickonCardInvoked(string CardName)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Click On Card Invoked, CardName={0}", CardName);
            }
        }

        /// <summary>
        /// Private Handle the ClickOnHex Event
        /// </summary>
        private void Network_ClickonHexInvoked(int HexIndex)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Click On Hex Invoked, HexIndex={0}", HexIndex);
            }
        }
        /// <summary>
        /// Private Handle the PlayCard Event
        /// </summary>
        private void Network_PlayCardInvoked(int PlayerId, int CardIndex, int HexIndex)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Play Card Invoked, PlayerId={0}, CardIndex={1}, HexIndex={2}", PlayerId, CardIndex, HexIndex);
            }
        }

        /// <summary>
        /// Private Handle the PlayerTurnEnd Event
        /// </summary>
        private void Network_PlayerTurnEndInvoked(int PlayerId)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Player Turn End Invoked, PlayerId={0}", PlayerId);
            }
        }

        /// <summary>
        /// Private Handle the ClickOnMonster Event
        /// </summary>
        private void Network_ClickOnMonsterInvoked(int MonsterId)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Player click on Monster, UID={0}", MonsterId);
            }
        }

        /// <summary>
        /// Private Handle the ClickOnBtn Event
        /// </summary>
        private void Network_ClickOnBtnInvoked(string BtnName)
        {
            if (_enableDebugLog)
            {
                Debug.LogFormat("NetworkPlayer: Player click on button, Button Name={0}", BtnName);
            }
        }
        /// <summary>
        /// Private Handle the ChangeToAction Event
        /// </summary>
        private void Network_ChangeToActionInvoked()
        {
            if (_enableDebugLog)
            {
                Debug.Log("NetworkPlayer: Now Change to Action Phase");
            }
        }
        #endregion
    }
}


