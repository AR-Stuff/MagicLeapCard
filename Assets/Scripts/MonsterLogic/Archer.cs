using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class Archer : Monster
    {
        #region Override Function
        /// <summary>
        /// Override the @Monster.Movement()
        /// Can Attack monster in range 4
        /// If the attack range is more than 1, the Double its atk
        /// </summary>
        public override void MoveMent()
        {
            GetComponent<Animator>().ResetTrigger("Reset");

            if (this.isAlive == false)
            {
                Debug.LogFormat("Error! Archer {0} is dead", uid);
                MonsterTurnEnd();
                return;
            }


            int Dist = 99999;
            Monster nearestMonster = null;
            foreach (KeyValuePair<int, Monster> MonsterItr in world.monsters)
            {
                Monster thisMonster = MonsterItr.Value;
                if (thisMonster.isAlive == false) continue;
                if (thisMonster.isExiled == true) continue;
                if (thisMonster.monsterOwner == this.monsterOwner) continue;
                int thisDist = world.tileMap.getDistance(this.HexIndex, thisMonster.HexIndex);
                if (thisDist < Dist)
                {
                    Dist = thisDist;
                    nearestMonster = thisMonster;
                }
            }
            if (nearestMonster == null)
            {
                Debug.Log("Cannot Find a monster.");
                MonsterTurnEnd();
                return;
            }
            if (Dist <= 2)
            {//The distance to a enemy monster is less than 3, Attack
                if(Dist<=1)
                {
                    Debug.LogFormat("Archer {0} Attack Target Monster{1}, My ATK={2}", uid, nearestMonster.GetUId(), this.ATK);
                    gameObject.transform.LookAt(nearestMonster.transform.position);
                    Attack(nearestMonster, this.ATK);
                }
                else
                {
                    Debug.LogFormat("Archer {0} Attack Target Monster{1}, My ATK={2}", uid, nearestMonster.GetUId(), this.ATK*2);
                    gameObject.transform.LookAt(nearestMonster.transform.position);
                    Attack(nearestMonster, this.ATK*2);
                }
                return;
            }
            else
            {
                List<int> myPath = world.tileMap.getShortestPath(this.HexIndex, nearestMonster.HexIndex);
                if(myPath.Count==0)
                {
                    Debug.Log("Cannot Find the Path");
                    MonsterTurnEnd();
                    return;
                }
                world.tileMap.ColorPath(myPath);
                Move(myPath[1]);
                return;
            }
        }
        #endregion
    }
}