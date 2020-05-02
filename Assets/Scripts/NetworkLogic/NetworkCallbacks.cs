using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Network
{
    /// <summary>
    /// Bolt will automatically detect this class and create an instance of it.
    /// This class lives together with Bolt and is destroyed when Bolt is shut down.
    /// </summary>
    [BoltGlobalBehaviour]
    public class NetworkCallbacks : Bolt.GlobalEventListener
    {
        #region Static Variable
        /// <summary>
        /// Public Interface to get the NetworkCallback Class
        /// </summary>
        public static NetworkCallbacks s_NetworkCallbacks;
        #endregion

        #region Public Variable
        /// <summary>
        /// Public Interface to get all Log messages
        /// </summary>
        public List<KeyValuePair<int, string>> LogMessages { get { return _logMessage; } }
        /// <summary>
        /// Public Interface to get Time Stamp
        /// </summary>
        public int TimeStamp { get { return _timeStamp; } }
        #endregion

        #region Private Variable
        /// <summary>
        /// Store all the log messages
        /// </summary>
        private List<KeyValuePair<int, string>> _logMessage = new List<KeyValuePair<int, string>>();
        /// <summary>
        /// Store the sync timestamp from server
        /// should be >=0 to be valid,-1 when init
        /// </summary>
        private int _timeStamp = -1;
        #endregion

        #region Bolt Global Event Handle
        /// <summary>
        /// When Scene Loading
        /// Init new Player
        /// Set the static variable s_NetworkCallbacks
        /// </summary>
        /// <param name="scene">The loading scene Name</param>
        public override void SceneLoadLocalDone(string scene)
        {
            base.SceneLoadLocalDone(scene);
            s_NetworkCallbacks = this;

            if(BoltNetwork.IsServer)
                AudioManager._instance.Play("WaitForPlayer");

            BoltNetwork.Instantiate(BoltPrefabs.Controller_Replicated_, new Vector3(0,0,0), Quaternion.identity);
            BoltNetwork.Instantiate(BoltPrefabs.NetworkPlayer, new Vector3(0, 0, 0), Quaternion.identity);
        }

        /// <summary>
        /// When receive a LogMessage
        /// </summary>
        /// <param name="evnt">The input Evnt</param>
        public override void OnEvent(MagicCardEvent evnt)
        {
            base.OnEvent(evnt);
            _logMessage.Insert(0, new KeyValuePair<int, string>(evnt.TimeStamp,evnt.Message));
        }

        /// <summary>
        /// When receive time synchronize event from Server
        /// </summary>
        /// <param name="evnt">The time synchronize event</param>
        public override void OnEvent(TimeStampEvent evnt)
        {
            base.OnEvent(evnt);
            _timeStamp = evnt.TimeStamp;
        }
        #endregion
    }

}
