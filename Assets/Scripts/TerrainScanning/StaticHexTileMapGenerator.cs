using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainScanning
{
    public class StaticHexTileMapGenerator : MonoBehaviour
    {
        #region Public Variable

        /// <summary>
        /// The parent transform you want to create the hex map.
        /// </summary>
        [Tooltip("The parent transform you want to create a hex map")]
        public GameObject parent;
        #endregion

        public float hexScale;
        public int hexRange = 8;
        public GameObject Raycast;
        public GameObject FancyGridObject;
        public GameObject hexTileTemplate;
        public GameObject BattleGround;

        #region Private Variable
        /// <summary>
        /// The plane that we can generate hex on.
        /// Generate by PlaneGenerator during gameplay.
        /// </summary>
        private GameObject quadObject;
        /// <summary>
        /// Camps for different player
        /// </summary>
        private int[] Camp0 = new int[] {
            161,146,130,113,96,80,65,51,38,26,15,5,
            147,131,114,97,81,66,52,39,27,16,6,
            132,115,98,82,67,53,40,28,17,7,
            116,99,83,68,54,41,29,18,8
        };
        private HashSet<int> CampSet0 = new HashSet<int>();
        private int[] Camp1 = new int[] {
            55,70,86,103,120,136,151,165,178,190,201,211,
            69,85,102,119,135,150,164,177,189,200,210,
            84,101,118,134,149,163,176,188,199,209,
            100,117,133,148,162,175,187,198,208
        };
        private HashSet<int> CampSet1 = new HashSet<int>();
        #endregion

        private void addHex(float x, float y, float z, int coordx, int coordy, int coordz, int cellID, HexStatus status, HexType type)
        {
            GameObject tile = Instantiate(hexTileTemplate);
            tile.transform.parent = parent.transform;
            tile.transform.localPosition = new Vector3(x, y, z);
            tile.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            //tile.AddComponent<HexTile>();
            tile.GetComponent<HexTile>().setStatus(status);
            tile.GetComponent<HexTile>().setType(type);
            tile.GetComponent<HexTile>().setCoordinates(coordx, coordz);
            tile.GetComponent<HexTile>().setID(cellID);
            tile.GetComponent<HexTile>().setAccessible(true);
            tile.name = "HexTile" + tile.GetComponent<HexTile>().getID() + " [" + coordx.ToString() + "," + coordy.ToString() + "," + coordz.ToString() + "]";

            GameWorld.HexTileMap.GetInstant().RegisterHexTile(tile.GetComponent<HexTile>());
        }

        public void createHex(Vector3 hitPosition)
        {
            BattleGround.transform.position = hitPosition+new Vector3(0.0f, 0.1f, 0.0f);
            BattleGround.transform.localScale = new Vector3(hexScale, hexScale, hexScale);
            BattleGround.SetActive(true);
            quadObject = GameObject.Find("Quad(Clone)");
            Debug.Log("I am a " + quadObject.name);
            Renderer rend = quadObject.GetComponent<Renderer>();
            Debug.Log(rend.bounds.max);
            Debug.Log(rend.bounds.min);
            //int x = 0;
            //int y = 0;
            //int z = 0;
            //int x0 = 0;
            //int y0 = 0;
            //int z0 = 0;
            int cellID = 0;

            float height = quadObject.transform.position.y;
            quadObject.transform.position = new Vector3(quadObject.transform.position.x, height - 0.11f, quadObject.transform.position.z);
            Physics.SyncTransforms();

            Debug.LogFormat("{0}, {1}, {2}", rend.bounds.min.x, rend.bounds.min.y, rend.bounds.min.z);

            Vector3 cent = (rend.bounds.min + rend.bounds.max) / 2;

            foreach (var item in Camp0)
            {
                CampSet0.Add(item);
            }
            foreach (var item in Camp1)
            {
                CampSet1.Add(item);
            }
            //Texture type
            int HexTypeIndex;

            for (int coordx = -hexRange; coordx <= hexRange; coordx++)
                for (int coordz = hexRange; coordz >= -hexRange; coordz--)
                {
                    int coordy = -coordx - coordz;
                    if (coordy < -hexRange || coordy > hexRange) continue;
                    float z = 0.075f * coordy;
                    float x = 0.0866f * (coordx + coordy / 2f);
                    
                    if (CampSet0.Contains(cellID))
                    {
                        HexTypeIndex = 1;
                    }
                    else if (CampSet1.Contains(cellID))
                    {
                        HexTypeIndex = 2;
                    }
                    else
                    {
                        HexTypeIndex = Random.Range(3, 5);
                    }
                    addHex(x, 0.0f, z, coordx, coordy, coordz, cellID++, HexStatus.Normal, (HexType)HexTypeIndex);
                }

            GameObject.Find("FancyGrid").SetActive(false);
            //for (float i = rend.bounds.min.x + 0.5f; i <= rend.bounds.max.x - 0.5f; i = i + 0.9f)
            //{
            //    for (float j = rend.bounds.max.z + 0.3f; j >= rend.bounds.min.z - 0.3f; j = j - 0.52f)
            //    {
            //        Vector3 center = new Vector3(i, height + 0.4f, j);
            //        Collider[] hitColliders = Physics.OverlapSphere(center, 0.3f);
            //        Vector3 centertest = new Vector3(i, height - 0.4f, j);
            //        Collider[] hitColliderstest = Physics.OverlapSphere(centertest, 0.3f);
            //        //if (hitColliders.Length == 0 && hitColliderstest.Length == 1)
            //        addHex(i, height + 0.1f, j, x, y, z, cellID++, HexStatus.Normal, (HexType)Random.Range(1, 4));
            //        Vector3 center1 = new Vector3(i + 0.45f, height + 0.4f, j - 0.26f);
            //        Collider[] hitColliders1 = Physics.OverlapSphere(center1, 0.3f);
            //        Vector3 centertest1 = new Vector3(i, height - 0.4f, j);
            //        Collider[] hitColliderstest1 = Physics.OverlapSphere(centertest1, 0.3f);
            //        //if (hitColliders1.Length == 0 && hitColliderstest1.Length == 1)
            //        addHex(i + 0.45f, height + 0.1f, j - 0.26f, x + 1, y - 1, z, cellID++, HexStatus.Normal, (HexType)Random.Range(1, 4));
            //        y = y - 1;
            //        z = z + 1;
            //    }
            //    x0 = x0 + 2;
            //    y0 = y0 - 1;
            //    z0 = z0 - 1;
            //}
            //Destroy(quadObject);
        }
    }
}
