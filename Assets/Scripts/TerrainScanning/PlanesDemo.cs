using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace TerrainScanning
{
    public class PlanesDemo : MonoBehaviour
    {
        public Transform BBoxTransform;
        public Vector3 BBoxExtents;

        private float timeout = 5f;
        private float timeSinceLastRequest = 0f;

        private MLWorldPlanesQueryParams _queryParams = new MLWorldPlanesQueryParams();
        public MLWorldPlanesQueryFlags QueryFlags;

        public GameObject PlaneGameObject;
        private List<GameObject> _planeCache = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            MLWorldPlanes.Start();
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastRequest += Time.deltaTime;
            if (timeSinceLastRequest > timeout)
            {
                timeSinceLastRequest = 0f;
                RequestPlanes();
            }
        }

        void RequestPlanes()
        {
            // We will fill this in shortly
            _queryParams.Flags = QueryFlags;
            _queryParams.MaxResults = 100;
            _queryParams.BoundsCenter = BBoxTransform.position;
            _queryParams.BoundsCenter.y -= 1;
            _queryParams.BoundsRotation = BBoxTransform.rotation;
            _queryParams.BoundsExtents = BBoxExtents;

            MLWorldPlanes.GetPlanes(_queryParams, HandleOnReceivedPlanes);

        }

        private void HandleOnReceivedPlanes(MLResult result, MLWorldPlane[] planes, MLWorldPlaneBoundaries[] boundaries)
        {
            // We will fill this in shortly
            for (int i = _planeCache.Count - 1; i >= 0; --i)
            {
                Destroy(_planeCache[i]);
                _planeCache.Remove(_planeCache[i]);
            }

            GameObject newPlane;
            for (int i = 0; i < planes.Length; ++i)
            {
                newPlane = Instantiate(PlaneGameObject);
                newPlane.transform.position = planes[i].Center;
                newPlane.transform.rotation = planes[i].Rotation;
                newPlane.transform.localScale = new Vector3(planes[i].Width, planes[i].Height, 1f); // Set plane scale
                _planeCache.Add(newPlane);
            }
        }

        private void OnDestroy()
        {
            MLWorldPlanes.Stop();
        }

    }
}
