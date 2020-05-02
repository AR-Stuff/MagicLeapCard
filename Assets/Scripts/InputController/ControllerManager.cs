using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using TerrainScanning;
using GameWorld;
using Monsters;

namespace InputController
{
    #region Delegate Define
    /// <summary>
    /// When bumper click on a hex, this delegate invoked
    /// </summary>
    /// <param name="HexID">The chosen hex ID.</param>
    public delegate void ClickOnHex(int HexID);

    /// <summary>
    /// When bumper click on a card, this delegate invoked
    /// </summary>
    /// <param name="CardName">The chosen card name.</param>
    public delegate void ClickOnCard(string CardName);

    /// <summary>
    /// When bumper click on a monster, this delegate invoked
    /// </summary>
    /// <param name="currMonster">The selected Monster.</param>
    public delegate void ClickOnMonster(Monster currMonster);

    /// <summary>
    /// When bumper click on a button, this delegate invoked
    /// </summary>
    /// <param name="currMonster">The selected Monster.</param>
    public delegate void ClickOnBtn(string btnName);
    #endregion

    public class ControllerManager : MonoBehaviour
    {
        #region Public Variables
        public Vector3 hitPoint;
        public string selected;
        public GameObject selectedGameObject;
        public bool trigger;

        /// <summary>
        /// The Game World
        /// </summary>
        public World world;
        #endregion

        #region Private Variables
        private MLInputController controller;
        #endregion

        #region Event Define
        public ClickOnHex ClickOnHex;
        public ClickOnCard ClickOnCard;
        public ClickOnMonster ClickOnMonster;
        public ClickOnBtn ClickOnBtn;
        #endregion

        #region Unity Methods
        void Start()
        {
            MLInput.Start();
            controller = MLInput.GetController(MLInput.Hand.Left);
            MLInput.OnControllerButtonDown += OnButtonDown;
        }
        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            MLInput.Stop();
            MLInput.OnControllerButtonDown -= OnButtonDown;
        }

        #endregion

        void OnButtonDown(byte controller_id, MLInputControllerButton button)
        {
            //On Bumper Down
            if (button == MLInputControllerButton.Bumper)
            {
                RaycastHit hit;
                if (Physics.Raycast(controller.Position, transform.forward, out hit))
                {
                    selected = hit.transform.gameObject.tag;
                    selectedGameObject = hit.transform.gameObject;
                    hitPoint = hit.point;
                    switch (selected)
                    {
                        case ("Card"):
                            string cardName = selectedGameObject.name.Split('_')[0];
                            ClickOnCard(cardName);
                            break;
                        case ("HexTile"):
                            ClickOnHex(selectedGameObject.transform.parent.gameObject.GetComponent<HexTile>().getID());
                            break;
                        case ("Monster"):
                            ClickOnMonster(selectedGameObject.GetComponent<Monster>());
                            break;
                        case ("ActionBtn"):
                            ClickOnBtn(selectedGameObject.name);
                            break;
                            
                        default:
                            break;
                    }
                }

            }
        }

        void UpdateTriggerInfo()
        {
            if (controller.TriggerValue > 0.8f)
            {
                if (trigger == true)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(controller.Position, transform.forward, out hit))
                    {
                        if (hit.transform.gameObject.tag == "Interactable")
                        {
                            selectedGameObject = hit.transform.gameObject;
                        }
                    }
                    trigger = false;
                }
            }

            if (controller.TriggerValue < 0.2f)
            {
                trigger = true;
                if (selectedGameObject != null)
                {
                    selectedGameObject = null;
                }
            }
        }
    }

}
