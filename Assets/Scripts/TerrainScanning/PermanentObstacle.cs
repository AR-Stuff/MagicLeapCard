using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PermanentObstacle : MonoBehaviour
{
    [System.Serializable]
    public class ObstacleHexCoord
    {
        public int X;
        public int Y;
        public int Z;
    };

    /// <summary>
    /// Private Reference to get the gameworld
    /// </summary>
    [SerializeField]
    private GameWorld.World _myWorld;

    /// <summary>
    /// Public Reference to show which hex has been blocked
    /// </summary>
    public ObstacleHexCoord[] BlockHexCoord;


    // Start is called before the first frame update
    void Start()
    {
        if(_myWorld==null)
        {
            _myWorld = FindObjectOfType<GameWorld.World>();
        }
        _myWorld.RegisterPermanentObstacle(this);
    }
}
