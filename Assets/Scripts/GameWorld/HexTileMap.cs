using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainScanning;

namespace GameWorld
{
    public class HexTileMap : MonoBehaviour
    {
        #region Static Variable
        /// <summary>
        /// Store the only instant in the game
        /// Only one HexTileMap can be existed in the game
        /// </summary>
        private static HexTileMap _instant;
        /// <summary>
        /// Return the only instant of Hex Tile in the game
        /// </summary>
        /// <returns>The Instant of hexTilemap</returns>
        public static HexTileMap GetInstant()
        {
            return _instant;
        }
        /// <summary>
        /// Public Struct to store the HexCoord
        /// </summary>
        public struct HexCoord
        {
            public int X;
            public int Y;
            public int Z;

            public override string ToString()
            {
                string s = "(X=" + X.ToString() + ",Y=" + Y.ToString() + ",Z=" + Z.ToString() + ")";
                return s;

            }
        };
        #endregion

        #region Private Variable
        /// <summary>
        /// Private Dictionary to get all hextile
        /// </summary>
        private Dictionary<HexCoord, HexTile> _coordToHexTile = new Dictionary<HexCoord, HexTile>();
        #endregion

        #region Public Variable
        /// <summary>
        /// Public Interface to visit all hextile
        /// </summary>
        public Dictionary<HexCoord, HexTile> CoordToHexTile { get { return _coordToHexTile; } }
        #endregion

        public Material OriginalMat;
        public Material NewMat;
        
        private GameObject HexMap;
        private static int[][] directions = new int[][] { new int[] { 1, -1, 0 }, new int[] { 1, 0, -1 }, new int[] { 0, 1, -1 }, new int[] { -1, 1, 0 }, new int[] { -1, 0, 1 }, new int[] { 0, -1, 1 } };
        private static Dictionary<Tuple<int, int>, int> coordinatesToId = new Dictionary<Tuple<int,int>, int>();

        

        void Start()
        {
            if(_instant==null)
            {
                _instant = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            HexMap = GameObject.Find("HexMap");
        }

        /// <summary>
        /// return a list of vertex's id
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public List<int> getShortestPath(int startId, int endId)
        {
            List<int> path = new List<int>();
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();
            Dictionary<int, int> costSoFar = new Dictionary<int, int>();
            var frontier = new PriorityQueue<int>();
            
            frontier.Enqueue(startId, 0);
            cameFrom.Add(startId, startId);
            costSoFar.Add(startId, 0);

            while (frontier.Count > 0)
            {
                int curr = frontier.Dequeue();

                if(curr == endId) break;

                foreach(int neighborId in GetAccessibleSurroundHexIndex(curr)){
                    int newCost = costSoFar[curr] + getDistance(curr, neighborId);
                    if(!costSoFar.ContainsKey(neighborId) || newCost < costSoFar[neighborId]){
                        if(costSoFar.ContainsKey(neighborId)){
                            costSoFar.Remove(neighborId);
                            cameFrom.Remove(neighborId);
                        }

                        costSoFar.Add(neighborId, newCost);
                        cameFrom.Add(neighborId, curr);
                        int priority = newCost + getDistance(neighborId, endId);
                        frontier.Enqueue(neighborId, priority);
                    }
                }
            }

            int currIdInPath = endId;
            while(currIdInPath != startId){
                if(!cameFrom.ContainsKey(currIdInPath)){
                    Debug.Log("No way!");
                    return new List<int>();
                }
                path.Add(currIdInPath);
                currIdInPath = cameFrom[currIdInPath];
            }
            path.Add(startId);
            path.Reverse();
            //Show path in blue
            //ColorPath(path);
            return path;
        }

        /// <summary>
        /// return all vertexs' id it close to.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public List<int> getEdgeByVertexId(int vertexId,int EndId)
        {
            //init coordinatesToId Dictionary
            int i = 0;
            int childCount = HexMap.transform.childCount;
            if (coordinatesToId.Count != childCount)
            {
                while (i < childCount)
                {
                    var key = Tuple.Create(HexMap.transform.GetChild(i).GetComponent<HexTile>().getX(), HexMap.transform.GetChild(i).GetComponent<HexTile>().getZ());
                    coordinatesToId.Add(key, i);
                    i++;
                }
            }

            List<int> edges = new List<int>();
            int[] currCoord = HexMap.transform.GetChild(vertexId).GetComponent<HexTile>().getCoordinates();
            foreach (int[] dirt in directions)
            {
                int[] targetCoord = new int[] { currCoord[0] + dirt[0], currCoord[1] + dirt[1], currCoord[2] + dirt[2] };
                var targetKey = Tuple.Create(targetCoord[0], targetCoord[2]);
                if (coordinatesToId.ContainsKey(targetKey) && isAccessible(targetCoord) || coordinatesToId.ContainsKey(targetKey) && coordinatesToId[targetKey]==EndId)
                {
                    edges.Add(coordinatesToId[targetKey]);
                }
            }
            return edges;
        }

        public int getDistance(int startId, int endId)
        {
            HexTile s = HexMap.transform.GetChild(startId).GetComponent<HexTile>();
            HexTile e = HexMap.transform.GetChild(endId).GetComponent<HexTile>();
            return (Math.Abs(s.getX() - e.getX()) + Math.Abs(s.getY() - e.getY()) + Math.Abs(s.getZ() - e.getZ())) / 2;
        }

        /// <summary>
        /// Return a HexTile by Index, return null if doesn't find
        /// </summary>
        /// <param name="HexIndex">The HexTile Id</param>
        /// <returns>Return a HexTile, if it doesn't find return null</returns>
        public HexTile getHexTileByIndex(int HexIndex)
        {
            if (HexMap.transform.childCount < HexIndex) return null;
            HexTile result = HexMap.transform.GetChild(HexIndex).GetComponent<HexTile>();
            return result;
        }

        /// <summary>
        /// return coordinates is accesible or not
        /// </summary>
        /// <param name="coord"></param>
        //TODO: Add more constrains
        public Boolean isAccessible(int[] coord)
        {
            var key = Tuple.Create(coord[0], coord[2]);
            int id = coordinatesToId[key];
            return HexMap.transform.GetChild(id).GetComponent<HexTile>().getStatus() != HexStatus.Blocked;
            //return HexMap.transform.GetChild(id).GetComponent<HexTile>().getAccessible();
        }

        public class PriorityQueue<T>
        {
            private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

            public int Count
            {
                get { return elements.Count; }
            }

            public void Enqueue(T item, float priority)
            {
                elements.Add(new KeyValuePair<T, float>(item, priority));
            }

            // Returns the Location that has the lowest priority
            public T Dequeue()
            {
                int bestIndex = 0;

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Value < elements[bestIndex].Value)
                    {
                        bestIndex = i;
                    }
                }

                T bestItem = elements[bestIndex].Key;
                elements.RemoveAt(bestIndex);
                return bestItem;
            }
        }

        public void ColorPath(List<int> path)
        {
            // for (int i = 0; i < path.Count; i++)
            // {
            //     HexMap.transform.GetChild(path[i]).GetChild(0).GetComponent<Renderer>().material = NewMat;
            // }
        }


        /// <summary>
        /// Get a random hex index which is:
        /// No Terrain Effect,
        /// No Obstacle,
        /// No Occupied
        /// </summary>
        /// <returns></returns>
        public int GetARandomAviableIndex()
        {
            while(true)
            {
                int RandomNumber = UnityEngine.Random.Range(0, HexMap.transform.childCount - 1);
                HexTile RandomHex = getHexTileByIndex(RandomNumber);
                if (RandomHex.isObstacle) continue;
                if (RandomHex.isMonsterOn) continue;
                World world = World.GetInstant();
                if(world!=null)
                {
                    if (world.CheckTerrainEffect(RandomHex.getID()) != null) continue;
                }
                return RandomNumber;
            }
        }

        public List<int> GetAllSurroundHexIndex(int HexIndex)
        {
            List<int> ans = new List<int>();

            HexTile tile = getHexTileByIndex(HexIndex);
            if (tile == null) return ans;
            
            foreach(var dir in directions)
            {
                HexCoord hexCoord = new HexCoord();
                hexCoord.X = tile.getX() + dir[0];
                hexCoord.Y = tile.getY() + dir[1];
                hexCoord.Z = tile.getZ() + dir[2];

                if (!CoordToHexTile.ContainsKey(hexCoord)) continue;

                ans.Add(CoordToHexTile[hexCoord].getID());
            }
            return ans;
        }

        public List<int> GetAccessibleSurroundHexIndex(int HexIndex)
        {
            List<int> ans = new List<int>();

            HexTile tile = getHexTileByIndex(HexIndex);
            if (tile == null) return ans;

            foreach (var dir in directions)
            {
                HexCoord hexCoord = new HexCoord();
                hexCoord.X = tile.getX() + dir[0];
                hexCoord.Y = tile.getY() + dir[1];
                hexCoord.Z = tile.getZ() + dir[2];

                if (!CoordToHexTile.ContainsKey(hexCoord)) continue;
                HexTile target = CoordToHexTile[hexCoord];
                if(target.isMonsterOn || target.isObstacle) continue;
                ans.Add(CoordToHexTile[hexCoord].getID());
            }
            return ans;
        }

        /// <summary>
        /// Let HexTile register itself at the begin of game
        /// </summary>
        /// <param name="hexTile">The HexTile</param>
        public void RegisterHexTile(HexTile hexTile)
        {
            if (hexTile == null) return;

            HexCoord hexCoord = new HexCoord();
            hexCoord.X = hexTile.getX();
            hexCoord.Y = hexTile.getY();
            hexCoord.Z = hexTile.getZ();

            if(_coordToHexTile.ContainsKey(hexCoord))
            {
                //This Coordination is already existed
                Debug.LogErrorFormat("This Coordination {0} is already existed.", hexCoord.ToString());
                return;
            }
            else
            {
                _coordToHexTile.Add(hexCoord, hexTile);
            }
        }
    }
}

