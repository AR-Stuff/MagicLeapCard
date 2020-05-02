using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace Project_Network
{
    /// <summary>
    /// Server Only Class.
    /// Bolt will automatically detect this class and create an instance of it.
    /// This class lives together with Bolt and is destroyed when Bolt is shut down.
    /// </summary>
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class ServerCallback : GlobalEventListener
    {
        #region Static Variable
        /// <summary>
        /// Get the static instant of ServerCallback
        /// </summary>
        public static ServerCallback s_ServerCallback;
        #endregion

        #region Public Variable
        /// <summary>
        /// Public Interface to get server timeStamp
        /// </summary>
        public int TimeStamp { get { return _timeStamp; } }
        #endregion

        #region private Variable
        /// <summary>
        /// How long it will take to increase one time step
        /// </summary>
        private float _timeInterval = 0.5f;
        /// <summary>
        /// The time stamp, increasing every (_timeInterval) seconds
        /// </summary>
        private int _timeStamp;
        /// <summary>
        /// Recording the last update time
        /// </summary>
        private float _lastUpdateTime;
        #endregion

        #region Unity Function
        /// <summary>
        /// Start is called before the first frame update
        /// Init _lastUpdateTime
        /// Init _timeStamp
        /// </summary>
        void Start()
        {
            _lastUpdateTime = Time.time;
            _timeStamp = 0;
            s_ServerCallback = this;
        }
        
        /// <summary>
        /// Update is called once per frame
        /// Doing the timer and send global event(Time Stamp Event)
        /// </summary>
        void Update()
        {
            float nowTime = Time.time;
            if(nowTime-_lastUpdateTime>_timeInterval)
            {
                _timeStamp++;
                _lastUpdateTime = nowTime;
                TimeStampEvent timeStampEvent = TimeStampEvent.Create();
                timeStampEvent.TimeStamp = _timeStamp;
                //timeStampEvent.Send();
            }
        }
        #endregion

        #region Bolt Function
        public override void SceneLoadLocalDone(string scene)
        {
            base.SceneLoadLocalDone(scene);
            BoltNetwork.Instantiate(BoltPrefabs.NetworkGameManager);
        }
        #endregion
    }
}

