using System;
using UnityEngine;

namespace TerrainScanning
{
    public enum HexStatus
    {
        Normal,
        Blocked,
        Fire,
        BlackHole,
    }
    public enum HexType
    {
        Empty,
        Ground,
        Grass,
        Mud,
        Flower
    }

    public class HexTile : MonoBehaviour
    {
        public GameObject mesh;
        public Material[] HexMats;

        private int x;
        private int y;
        private int z;
        private int id;
        private bool accessible;
        public bool isObstacle;

        public bool isMonsterOn;
        public Monsters.Monster monster;

        public HexStatus hexStatus = HexStatus.Normal;
        private HexType hexType = HexType.Empty;

        private void Update()
        {
            //if(this.transform.childCount>1)
            //{
            //    setAccessible(false);
            //}
            //else
            //{
            //    setAccessible(true);
            //}
            isMonsterOn = false;
            for (int i = 0; i < this.transform.childCount; i++) 
            {
                var tempObject = this.transform.GetChild(i);
                Monsters.Monster monster = tempObject.GetComponent<Monsters.Monster>();
                if(monster!=null)
                {
                    isMonsterOn = true;
                    this.monster = monster;
                }
            }
        }

        public HexTile(int X, int Z, int myID){
            this.x = X;
            this.z = Z;
            this.y = 0 - X - Z;
            this.id = myID;
            this.accessible = true;
        }
        public void setCoordinates(int X, int Z)
        {
            this.x = X;
            this.z = Z;
            this.y = 0 - X - Z;
        }
        public int[] getCoordinates()
        {
            return new int[] { x, y, z };
        }
        public int getX()
        {
            return this.x;
        }
        public int getY()
        {
            return this.y;
        }
        public int getZ()
        {
            return this.z;
        }
        public void setID(int temp)
        {
            this.id = temp;
        }
        public int getID()
        {
            return this.id;
        }
        
        public void setAccessible(bool b){
            this.accessible = b;
        }
        public bool getAccessible(){
            return this.accessible;
        }
        public void setStatus(HexStatus status)
        {
            hexStatus = status;
            // TODO: operations when status is changed
        }
        public HexStatus getStatus()
        {
            return hexStatus;
        }
        public void setType(HexType type)
        {
            hexType = type;
            if(mesh != null)
            {
                Renderer rend = mesh.GetComponent<Renderer>();
                switch (type)
                {
                    case HexType.Ground:
                        rend.sharedMaterial = HexMats[1];
                        break;
                    case HexType.Grass:
                        rend.sharedMaterial = HexMats[2];
                        break;
                    case HexType.Mud:
                        rend.sharedMaterial = HexMats[3];
                        break;
                    case HexType.Flower:
                        rend.sharedMaterial = HexMats[4];
                        break;
                    default:
                        rend.sharedMaterial = HexMats[0];
                        break;
                }
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            HexTile tile = obj as HexTile;
            return this.x == tile.getX() && this.y == tile.getY();
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            return base.GetHashCode();
        }

        public static int operator-(HexTile a,HexTile b)
        {
            if (a == null) return -1;
            if (b == null) return -1;

            return (Math.Abs(a.getX() - b.getX()) + Math.Abs(a.getY() - b.getY()) + Math.Abs(a.getZ() - b.getZ())) / 2;
        }
    }
}
