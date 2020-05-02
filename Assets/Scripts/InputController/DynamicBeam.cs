using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using HighlightingSystem;

namespace InputController
{
    public class DynamicBeam : MonoBehaviour
    {
        #region Public Variables
        public GameObject controller;
        public Vector3 hitPoint;
        // Hover color
        public Color hoverColor = Color.red;
        public Highlighter highlighter;
        public string selectedTag;
        public GameObject selectedGameObject;
        public GameObject prevSelected;
        #endregion

        #region Private Variables
        [SerializeField, Tooltip("The LineRenderer to show the line from the input to the hit point.")]
        private LineRenderer beamLine;
        #endregion

        #region Unity Methods
        // Update is called once per frame
        void Update()
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                beamLine.useWorldSpace = true;
                beamLine.SetPosition(0, controller.transform.position);
                beamLine.SetPosition(1, hit.point);
                hitPoint = hit.point;
                
                //Render collider border on hover 
                selectedTag = hit.transform.gameObject.tag;

                GameObject hitObject = hit.transform.gameObject;
                Debug.Log(selectedTag);
                if(selectedTag=="HexTile")
                {

                    hitObject = hitObject.transform.parent.gameObject;
                }


                if(hitObject!=selectedGameObject)
                {
                    prevSelected = selectedGameObject;
                    if(prevSelected!=null)
                    {
                        if(prevSelected.GetComponent<Clickable>())
                        {
                            prevSelected.GetComponent<Clickable>().unHighlighted();
                        }
                    }
                    selectedGameObject = hitObject;
                    if(selectedGameObject!=null)
                    {
                        if(selectedGameObject.GetComponent<Clickable>())
                        {
                            selectedGameObject.GetComponent<Clickable>().Highlighted();
                        }
                    }
                }

            }

        }
        #endregion

        #region Public Methods
        // To use Highlighter Plugin, trigger this method via onHover event
        public void OnHover(RaycastHit hitInfo)
        {
            Transform tr = hitInfo.collider.transform;
            if (tr == null) { return; }

            highlighter = tr.GetComponentInParent<Highlighter>();
            if (highlighter == null) { return; }

            // Highlight by hoverColor | Cannot work on OpenGL4.5
            highlighter.Hover(hoverColor);
        }
        #endregion
    }
}
