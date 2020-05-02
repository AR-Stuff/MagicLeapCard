//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.XR.MagicLeap;

//namespace GameLogic
//{
//    public class RaycastHead : MonoBehaviour
//    {

//        #region Public Variable
//        [SerializeField, Tooltip("Raycast from headpose.")]
//        public WorldRaycastHead _raycastHead = null;
//        public Transform camTransform;
//        #endregion

//        #region Private Variable
//        private float startTime;
//        private MLWorldRays.QueryParams _raycastParams = new MLWorldRays.QueryParams();
//        #endregion

//        /// <summary>
//        /// Validate all required components and sets event handlers.
//        /// </summary>
//        void Start()
//        {
//            startTime = Time.time;
//            //MLWorldRays.Start();
//        }

//        void Update()
//        {
//            float nowTime = Time.time;
//            if (nowTime - startTime > 5.0f)
//            {
//                if (_raycastHead != null)
//                {
//                    _raycastHead.gameObject.SetActive(true);
//                }
//            }
//            // Update the orientation data in the raycast parameters.
//            _raycastParams.Position = camTransform.position;
//            _raycastParams.Direction = camTransform.forward;
//            _raycastParams.UpVector = camTransform.up;

//            // Make a raycast request using the raycast parameters 
//            //MLWorldRays.GetWorldRays(_raycastParams, HandleOnReceiveRaycast);
//        }

//        /// <summary>
//        /// Cleans up the component.
//        /// </summary>
//        void OnDestroy()
//        {
//            MLWorldRays.Stop();
//        }

//        #region Event Handlers
//        /// <summary>
//        /// Callback handler called when raycast has a result.
//        /// Updates the confidence value to the new confidence value.
//        /// </summary>
//        /// <param name="state"> The state of the raycast result.</param>
//        /// <param name="result">The hit results (point, normal, distance).</param>
//        /// <param name="confidence">Confidence value of hit. 0 no hit, 1 sure hit.</param>
//        public void OnRaycastHit(MLWorldRays.MLWorldRaycastResultState state, RaycastHit result, float confidence)
//        {
//            Debug.Log("hit");
//        }

//        void HandleOnReceiveRaycast(MLWorldRays.MLWorldRaycastResultState state,
//                                UnityEngine.Vector3 point, Vector3 normal,
//                                float confidence)
//        {
//            Debug.Log("Receive Ray");
//        }
//        #endregion
//    }
//}