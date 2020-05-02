using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// Include All possible terrain effect
    /// </summary>
    public enum TerrainEffect
    {
        Fire,
        Tornado,
        Mist,
        Portal,
        None
    };

    /// <summary>
    /// A Class to organize the interactive terrain
    /// Store the data it need
    /// </summary>
    public class InteractiveTerrain : MonoBehaviour
    {
        #region static Property
        /// <summary>
        /// the total number of all terrainCard in the game
        /// </summary>
        static int terrainCardCount = 0;
        #endregion

        #region private Variable
        /// <summary>
        /// Private variable to get the game world(battlefield)
        /// </summary>
        private World _world;
        /// <summary>
        /// Private variable to store the hex index
        /// Include all the affected hex, for example fire card should be 7 and portal should be 2
        /// </summary>
        private List<int> _hexIndex=new List<int>();
        /// <summary>
        /// Private variable to store the unique ID of this card
        /// </summary>
        private int _uid;
        /// <summary>
        /// Private variable to store the unique ID
        /// </summary>
        private string _terrainName;
        /// <summary>
        /// Private variable to discribe what kind of effect of this card
        /// </summary>
        private TerrainEffect _terrainEffect = TerrainEffect.None;
        #endregion

        #region Public Interface
        /// <summary>
        /// Public Reference to get the game world
        /// </summary>
        public World World { get { return _world; } set { _world = value; } }
        /// <summary>
        /// Pulbic Interface to get which Hex this terrain card is on
        /// The terrain card can only be placed in the init process, It will never change during the game
        /// </summary>
        public List<int> HexIndex { get { return _hexIndex; } }
        /// <summary>
        /// A Unique Index to record the terrain card
        /// The uid card can be set during init process, It will never change during the game
        /// </summary>
        public int UniqueIndex { get { return _uid; } }
        /// <summary>
        /// Public Interface to get the terrain card name
        /// </summary>
        public string TerrainName { get { return _terrainName; } }
        /// <summary>
        /// Public Interface to get the effect of this card
        /// </summary>
        public TerrainEffect myTerrainEffect { get { return _terrainEffect; } }
        #endregion

        #region Init Function
        /// <summary>
        /// Terrain Card Init
        /// </summary>
        public void TerrainCardInit(string TerrainName,List<int> AffectedHexs)
        {
            _uid = terrainCardCount;
            terrainCardCount++;
            _terrainName = TerrainName;
            switch (TerrainName)
            {
                case ("Fire"):
                    _terrainEffect = TerrainEffect.Fire;
                    break;
                case ("Tornado"):
                    _terrainEffect = TerrainEffect.Tornado;
                    break;
                case ("Mist"):
                    _terrainEffect = TerrainEffect.Mist;
                    break;
                case ("Portal"):
                    _terrainEffect = TerrainEffect.Portal;
                    break;
                default:
                    _terrainEffect = TerrainEffect.None;
                    break;
            }
            foreach(int itr in AffectedHexs)
            {
                _hexIndex.Add(itr);
            }
        }
        #endregion
    }
}
