using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class Maria_Brute : Monster
    {
        public bool is_defensed = false;
        protected float defense_animation_time = 0.5f;
        protected float undefense_animation_time = 0.5f;

        [SerializeField]
        private ParticleSystem DefenseParticle;

        override public void Move(int destination)
        {
            is_defensed = false;
            base.Move(destination);
        }

        public virtual void Defense()
        {
            if (is_defensed) return;
            is_defensed = true;
            GetComponent<Animator>().SetTrigger("Defense");
            AnimationFinishedTime = Time.time + defense_animation_time;
            WaitForAnimation = MonsterAction.Defense;
            if(DefenseParticle!=null)
            {
                DefenseParticle.Play();
            }
        }
        public virtual void UnDefense()
        {
            if (!is_defensed) return;
            is_defensed = false;
            GetComponent<Animator>().SetTrigger("Reset");
            AnimationFinishedTime = Time.time + undefense_animation_time;
            WaitForAnimation = MonsterAction.Defense;
            if(DefenseParticle!=null)
            {
                DefenseParticle.Stop();
                DefenseParticle.Clear();
            }
        }

        /// <summary>
        /// this function will transmit all states of the monster to its respective GameObject (with visualization)
        /// </summary>
        /// <param name="StateField">
        /// "Damage", If in defense state, the damage is decreased by 3
        /// "Defense", active defense state
        /// "UnDefense" unactive defense state
        /// </param>
        public override void StateUpdate(string StateField, int newState)
        {
            if(StateField=="Defense")
            {
                Defense();
                if(SkillCoolDownMax!=0)
                {
                    canSkill = false;
                    SkillCoolDownCounter = SkillCoolDownMax;
                }

            }
            else if(StateField=="UnDefense")
            {
                UnDefense();
            }
            else if(is_defensed && StateField=="Damage")
            {
                ///Since it defense, the damage will decrease by 3
                ///If damage is less than 3 then no damage and animation will be played
                if(newState>=3)
                    base.StateUpdate(StateField, newState - 3);
                else
                {
                    //Nothing to do
                }
            }
            else
            {
                base.StateUpdate(StateField, newState);
            }
        }

        /// <summary>
        /// Invoked when player select this monster
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            StateUpdate("UnDefense", 0);
            
            return;
        }
    }
}

