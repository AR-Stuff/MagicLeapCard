using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Network
{
    /// <summary>
    /// 1. Game start, start as client automately, wait 5 seconds(timer)
    /// 2. If it can find the server, join the server and close the menu, switch to game scene
    /// 3. If the timer passed, shut down the client
    /// 4. Wait 2 seconds between client and server switch.
    /// 5. Start the server and switch to game scene
    /// </summary>
    public class HostJoinSwitcher : MonoBehaviour
    {
        #region Public Variable
        /// <summary>
        /// The total waiting time as a client
        /// </summary>
        public float WaitingTime = 5.0f;
        /// <summary>
        /// The least waiting time between shutdown and start server
        /// </summary>
        public float SwitchTime = 2.0f;
        /// <summary>
        /// A Public Reference to the Bolt Menu
        /// </summary>
        public Menu menu;
        /// <summary>
        /// Init as false, when the client join a session change to true
        /// </summary>
        public bool isJoined;
        #endregion

        #region Private Variable
        /// <summary>
        /// A Timer to countdown the client waiting time
        /// When the timer is over, shutdown the client and start as a server
        /// </summary>
        private float ClientTimer;
        /// <summary>
        /// Init as true, when client shutdown set to true
        /// </summary>
        private bool isClient;
        /// <summary>
        /// A Timer to countdown the server
        /// </summary>
        private float serverTimer;
        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when game start
        /// </summary>
        private void Start()
        {
            isClient = true;
            isJoined = false;
            menu.StartClient();
            AudioManager._instance.Play("LobbyServer");
            ClientTimer = Time.time;
        }
        /// <summary>
        /// Invoke every frame
        /// </summary>
        private void Update()
        {
            if (isClient && !isJoined)
            {
                if (Time.time - ClientTimer > WaitingTime)
                {
                    isClient = false;
                    menu.ClientShutDown();
                    serverTimer = Time.time;
                }
            }
            if (!isClient && !isJoined)
            {
                if (Time.time - serverTimer > SwitchTime)
                {
                    if (!BoltNetwork.IsRunning)
                    {
                        menu.StartServer();
                        AudioManager._instance.Play("LobbyNoServer");
                        isJoined = true;
                    }
                }
            }
        }
        #endregion
    }
}

