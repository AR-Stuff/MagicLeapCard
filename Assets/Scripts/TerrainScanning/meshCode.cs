using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using InputController;

namespace TerrainScanning
{
    public delegate void ScanFinished();
    public class meshCode : MonoBehaviour
    {
        public StaticHexTileMapGenerator hx;
        public Material BlackMaterial;
        public MLSpatialMapper Mapper;
        public GameObject PlaneGenerator;
        public GameObject TileGenerator;
        public GameObject MLSpatialMapper;
        public GameObject ray;
        /// <summary>
        /// Define the object for delegate
        /// </summary>
        public ScanFinished ScanFinished;

        #region Private Variable
        private MLInputController _controller;
        /// <summary>
        /// A bool variable to control the system only generate once
        /// </summary>
        private bool AlreadyGenerate = false;
        #endregion

        void Start()
        {
            //Start receiving input by the Control
            MLInput.Start();
            MLInput.OnControllerButtonDown += OnButtonDown;
            _controller = MLInput.GetController(MLInput.Hand.Left);
        }
        void OnDestroy()
        {
            //Stop receiving input by the Control
            MLInput.Stop();
        }
        void Update()
        {
        }

        void OnButtonDown(byte controller_id, MLInputControllerButton button)
        {
            if (button == MLInputControllerButton.Bumper)
            {
                if (AlreadyGenerate) return;
                Debug.Log("yacha yacha");
                for (int i = 0; i < transform.childCount; i++)
                {
                    // Get the child gameObject
                    GameObject gameObject = transform.GetChild(i).gameObject;
                    // Get the meshRenderer component
                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                    // Get the assigned material
                    Material material = meshRenderer.sharedMaterial;
                    if (material != BlackMaterial)
                    {
                        meshRenderer.material = BlackMaterial;
                    }
                }

                hx.createHex(ray.GetComponent<DynamicBeam>().hitPoint);
                AlreadyGenerate = true;
                ScanFinished();
                Destroy(PlaneGenerator);
                Destroy(TileGenerator);
                Destroy(MLSpatialMapper);


                // Destroy(gameObject);
            }
        }
    }
}
