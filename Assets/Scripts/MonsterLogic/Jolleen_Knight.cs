using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TerrainScanning;

namespace Monsters
{
    public class Jolleen_Knight : Monster
    {
        protected float spell_animation_time = 2.5f;

        public ParticleSystem AOE_VFX;

        /// <summary>
        /// x=b.x-a.x; y=b.y-a.y; z=b.z-a.z;
        /// Case1: x+y>=0 y+z>=0 y>=0
        /// Case2: x+y>=0 y+z<0 y>=0
        /// Case3: x+y>=0 y+z<0 y<0
        /// Case4: x+y<0 y+z<0 y<0
        /// Case5: x+y<0 y+z>=0 y<0
        /// Case6: x+y<0 y+z>=0 y>=0
        /// </summary>
        private int GetDirCase(HexTile a,HexTile b)
        {
            int x = b.getX() - a.getX();
            int y = b.getY() - a.getY();
            int z = b.getZ() - a.getZ();
            if (x + y >= 0 && y + z >= 0 && y >= 0) return 1;
            if (x + y >= 0 && y + z < 0 && y >= 0) return 2;
            if (x + y >= 0 && y + z < 0 && y < 0) return 3;
            if (x + y < 0 && y + z < 0 && y < 0) return 4;
            if (x + y < 0 && y + z >= 0 && y < 0) return 5;
            if (x + y < 0 && y + z >= 0 && y >= 0) return 6;
            return -1;
        }

        public virtual void Spell(Monster subjectMonster)
        {
            gameObject.transform.LookAt(subjectMonster.transform.position);
            GetComponent<Animator>().SetTrigger("Spell");
            AnimationFinishedTime = Time.time + spell_animation_time;
            //WaitForAnimation = MonsterAction.Spell;
            ActionParam.AttackDamage = 2;
            ActionParam.AttackSubject = subjectMonster;
            AOE_VFX.Play();

            HexTile thisHex = world.tileMap.getHexTileByIndex(this.HexIndex);
            HexTile targetHex = world.tileMap.getHexTileByIndex(subjectMonster.HexIndex);

            int monsterCase=GetDirCase(thisHex, targetHex);

            foreach(var itr in world.monsters)
            {
                if(itr.Value.monsterOwner==this.monsterOwner)
                {
                    continue; //No Friendly Fire
                }
                HexTile tempHex = world.tileMap.getHexTileByIndex(itr.Value.HexIndex);
                if (tempHex - thisHex > 3) continue;//Too Far for Skill

                int tempCast = GetDirCase(thisHex, tempHex);
                if(tempCast==monsterCase)
                {
                    itr.Value.MonsterStateUpdate.Invoke("Damage", ActionParam.AttackDamage);
                }
            }

            WaitForAnimation = MonsterAction.Attack;
        }

        void Start()
        {
            base.Start();
            attack_animation_time = 2.0f; 
        }

        /// <summary>
        /// "AOE" to use AOE skill, newState= target Monster UID
        /// </summary>
        public override void StateUpdate(string StateField, int newState)
        {
            if (StateField == "AOE")
            {
                if (SkillCoolDownMax <= 0)
                {
                    SkillCoolDownCounter = SkillCoolDownMax;
                    canSkill = false;
                }
                var targetMonster = GameWorld.World.GetInstant().getMonsterByID(newState);
                Spell(targetMonster);                
            }
            else
            {
                base.StateUpdate(StateField, newState);
            }
        }
    }
}

