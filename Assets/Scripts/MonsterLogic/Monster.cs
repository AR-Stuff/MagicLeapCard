using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameLogic;
using TerrainScanning;
using GameWorld;
using UI;

namespace Monsters
{
    public enum MonsterAction
    {
        Move,
        Attack,
        Spell,
        Defense,
        Nothing
    }

    public class MonsterActionParam
    {
        public int MoveDestination;
        public int AttackDamage;
        public Monster AttackSubject;
    }


    public delegate void MonsterTurnEnd();

    [System.Serializable]
    public class StateUpdateEvent : UnityEvent<string, int> { };

    public class Monster : MonoBehaviour
    {
        #region static Value
        /// <summary>
        /// the total number of all monster in the game
        /// </summary>
        static int monsterCount = 0;
        #endregion

        #region Protected Attributes
        /// <summary>
        /// the unique id of this monster 
        /// </summary>
        public int uid;
        /// <summary>
        /// The link to the GameObject
        /// </summary>
        protected GameObject monsterObj;
        /// <summary>
        /// Monster's original HP property, should not be change during game, @HP should be reset to this value when turn begin.
        /// </summary>
        protected int origin_HP;
        /// <summary>
        /// Monster's original ATK property, should not be change during game, @ATK should be reset to this value when turn begin.
        /// </summary>
        protected int origin_ATK;
        /// <summary>
        /// Monster's original SPD property, should not be change during game, @SPD should be reset to this value when turn begin.
        /// </summary>
        protected int origin_SPD;
        /// <summary>
        /// Monster's original Owner, should not be change during game. All monster should return to its owner's control when turn begin.
        /// </summary>
        protected Player origin_monsterOwner;
        /// <summary>
        /// The hex this monster is casted on. Should not be change during game. Monster will return to this hex when turn begin.
        /// </summary>
        protected int origin_HexIndex;

        protected float moving_animation_time = 1.0f;
        protected float attack_animation_time = 1.0f;

        protected List<int> moving_path = new List<int>();

        /// <summary>
        /// Animation of the monster
        /// </summary>
        protected MonsterAction WaitForAnimation = MonsterAction.Nothing;
        /// <summary>
        /// Parameter of an action of monster
        /// </summary>
        protected MonsterActionParam ActionParam = new MonsterActionParam();

        /// <summary>
        /// The time Animation Stop
        /// </summary>
        protected float AnimationFinishedTime;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Monster Current HP, will increase or decrease during battle. Will reset to @origin_HP when turn begin.
        /// </summary>
        public int HP;
        /// <summary>
        /// Monster Current ATK, will increase or decrease during battle. Will reset to @origin_ATK when turn begin.
        /// </summary>
        public int ATK;
        /// <summary>
        /// Monster Current SPD, will increase or decrease during battle. Will reset to @origin_SPD when turn begin.
        /// </summary>
        public int SPD;
        /// <summary>
        /// Monster Current Owner, will be change because of the card effect.All monster should return to its owner's control when turn begin.
        /// </summary>
        public Player monsterOwner;
        /// <summary>
        /// Monster Name, will never change after init.
        /// </summary>
        public string monsterName;
        /// <summary>
        /// The Current Hex this monster is steped on. Will reset to the @origin_HexIndex when turn begin.
        /// </summary>
        public int HexIndex;
        /// <summary>
        /// A bool value to store wether the monster is alived or not. Will return to true when turn begin.
        /// A dead monster doesn't attack and have no effect.
        /// </summary>
        public bool isAlive;
        /// <summary>
        /// A bool value to store wether the monster is exiled or not. Will never return to true when turn begin.
        /// A exiled monster will have no move and no effect. And will do nothing when turn begin.
        /// </summary>
        public bool isExiled;
        /// <summary>
        /// A bool value to store wether the monster can do skill this turn or not.
        /// A monster can play skill and cool down for SkillCoolDownCounter turn.
        /// </summary>
        public bool canSkill = true;
        /// <summary>
        /// A int value to store how many turn still need to cool down
        /// </summary>
        public int SkillCoolDownCounter = 0;
        /// <summary>
        /// A int value to store the max turns to cool down
        /// </summary>
        public int SkillCoolDownMax = 0;
        #endregion

        #region Public Variable
        /// <summary>
        /// The link to the game world
        /// </summary>
        [Tooltip("A link to the game world.")]
        public World world;
        /// <summary>
        /// Monster Maximum Movement Range in a turn
        /// </summary>
        public int MaxMovementRange=3;
        /// <summary>
        /// Monster Minimum Movement Range in a turn
        /// </summary>
        public int MinAttackRange = 1;
        /// <summary>
        /// Monster Maximum Movement Range in a turn
        /// </summary>
        public int MaxAttackRange = 1;
        #endregion

        #region Delegate and Event Handler
        /// <summary>
        /// A delegate instant, invoke when monster finish their turn.
        /// </summary>
        public MonsterTurnEnd MonsterTurnEnd;
        /// <summary>
        /// A Event invoked by the World, that tell the monster to start their turn.
        /// </summary>
        public UnityEvent MonsterStartTurn;
        /// <summary>
        /// A Event invoked by the world and other monster, that tell the monster to update their state.
        /// </summary>
        public StateUpdateEvent MonsterStateUpdate;
        /// <summary>
        /// If isIdle, then the monster cannot move or attack
        /// </summary>
        public bool isIdle;
        /// <summary>
        /// The healthBar inside this monster
        /// </summary>
        [Tooltip("The healthBar inside this monster")]
        public HealthBar healthBar;
        #endregion

        #region Public Function
        /// <summary>
        /// Initialization of a monster.
        /// Set all value to its original value, the original value should not be change during game in common case.
        /// </summary>
        /// <param name="name">Monster Origin Name</param>
        /// <param name="hp">Monster Origin HP</param>
        /// <param name="atk">Monster Origin Atk</param>
        /// <param name="spd">Monster Origin Spd</param>
        /// <param name="monsterOwner">Monster Origin Owner</param>
        /// <param name="HexIndex"></param>
        public void MonsterInit(string name, int hp, int atk, int spd, Player monsterOwner, int HexIndex)
        {
            uid = monsterCount;
            monsterCount++;
            Debug.Log("A new monster with uid={0} is created." + uid);
            isExiled = false;
            monsterName = name;
            origin_HP = hp;
            origin_ATK = atk;
            origin_SPD = spd;
            origin_monsterOwner = monsterOwner;
            origin_HexIndex = HexIndex;
            MonsterReset();


        }

        /// <summary>
        /// Reset a monster, all stats to its original stats.
        /// Invoke when turn start.
        /// </summary>
        public void MonsterReset()
        {
            if (isExiled)
            {
                Debug.LogFormat("Monster {0} Error: An exiled monster should be reset.", gameObject.transform.name);
                return;
            }
            HP = origin_HP;
            ATK = origin_ATK;
            SPD = origin_SPD;
            monsterOwner = origin_monsterOwner;
            //HexIndex = origin_HexIndex;
            StateUpdate("Move", origin_HexIndex);
            isAlive = true;
            gameObject.GetComponent<Animator>().SetTrigger("Reset");
            //GetComponent<Renderer>().material.color = (monsterOwner.PlayerId == 0 ? Color.red : Color.blue);
        }
        /// <summary>
        /// get the monster's uid
        /// </summary>
        public int GetUId() { return uid; }
        #endregion

        #region Override Operator
        /// <summary>
        /// Sorted Monster by SPD and UID
        /// </summary>
        static public bool operator <(Monster a, Monster b)
        {
            if (a.SPD != b.SPD)
                return a.SPD < b.SPD;
            else
                return a.uid < b.uid;
        }
        /// <summary>
        /// Sorted Monster by SPD and UID
        /// </summary>
        static public bool operator >(Monster a, Monster b)
        {
            if (a.SPD != b.SPD)
                return a.SPD > b.SPD;
            else
                return a.uid > b.uid;
        }
        #endregion

        #region Unity Function
        /// <summary>
        /// This Function will invoke when game start and monster is created
        /// </summary>
        protected void Start()
        {
            isIdle = false;
            moving_animation_time = 1.0f;
            attack_animation_time = 1.0f;
            if (MonsterStartTurn == null)
                MonsterStartTurn = new UnityEvent();
            if (MonsterStateUpdate == null)
                MonsterStateUpdate = new StateUpdateEvent();
            MonsterStartTurn.AddListener(MoveMent);
            MonsterStateUpdate.AddListener(StateUpdate);
            healthBar.SetMaxHealth(this.HP);
        }

        /// <summary>
        /// This Function will invoke when the monster is destoryed.
        /// </summary>
        void OnDestroy()
        {
            Debug.Log("A new monster with uid={0} is destroyed." + uid);
        }

        /// <summary>
        /// Timer for Animation Played
        /// </summary>
        void Update()
        {
            float nowTime = Time.time;
            if (WaitForAnimation != MonsterAction.Nothing)
            {
                if (nowTime > AnimationFinishedTime)
                {
                    switch (WaitForAnimation)
                    {
                        case MonsterAction.Move:
                            WaitForAnimation = MonsterAction.Nothing;
                            StateUpdate("Move", ActionParam.MoveDestination);
                            break;
                        case MonsterAction.Attack:
                            WaitForAnimation = MonsterAction.Nothing;
                            ActionParam.AttackSubject.MonsterStateUpdate.Invoke("Damage", ActionParam.AttackDamage);
                            break;
                    }
                    //MonsterTurnEnd();
                }
            }
        }
        #endregion

        #region Protected Virtual Function
        /// <summary>
        /// Change the State by terrrain Effect
        /// </summary>
        protected virtual void CheckTerrainEffect()
        {
            World world = World.GetInstant();
            if (world == null) return;
            InteractiveTerrain terrain = world.CheckTerrainEffect(HexIndex);
            if (terrain == null) return; //NO Effect
            switch(terrain.myTerrainEffect)
            {
                case (TerrainEffect.Fire):
                    this.StateUpdate("Damage", 5);
                    break;
                case (TerrainEffect.Mist):
                    moving_path.Clear();
                    break;
                case (TerrainEffect.Portal):
                    moving_path.Clear();
                    int TargetHex = -1;
                    foreach(var itr in terrain.HexIndex)
                    {
                        if(itr!=this.HexIndex)
                        {
                            TargetHex = itr;
                            break;
                        }
                    }
                    // if (TargetHex != -1) moving_path.Add(TargetHex);
                    if (TargetHex == 91) moving_path.Add(TargetHex);
                    break;
                case (TerrainEffect.Tornado):
                    this.StateUpdate("Damage", 3);
                    moving_path.Clear();
                    if(monsterOwner==GameObject.Find("GameManager").GetComponent<GameManager>().Player0)
                    {
                        int RandomHex = world.tileMap.GetARandomAviableIndex();
                        //moving_path.Add(RandomHex);
                        GameObject.Find("GameManager").GetComponent<GameManager>().Send_TornadoHex(this.GetUId(), RandomHex);
                    }
                    break;
            }

        }
        #endregion

        #region Event Function
        /// <summary>
        /// (Deprecated)
        /// Monster's movement in one turn.
        /// Maybe Override in Child Class
        /// </summary>
        public virtual void MoveMent()
        {
            GetComponent<Animator>().ResetTrigger("Reset");

            if (this.isAlive == false)
            {
                Debug.LogFormat("Error! Monster {0} is dead", uid);
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
            if (Dist == 1 || Dist == 0)
            {//Adjcent to a enemy monster, Attack
                //Debug.LogFormat("Attack Target Monster{0}, My ATK={1}", uid, this.ATK);
                nearestMonster.MonsterStateUpdate.Invoke("Damage", this.ATK);
                MonsterTurnEnd();
                return;
            }
            else
            {
                List<int> myPath = world.tileMap.getShortestPath(this.HexIndex, nearestMonster.HexIndex);
                world.tileMap.ColorPath(myPath);
                this.StateUpdate("Move", myPath[1]);
                MonsterTurnEnd();
                return;
            }
        }

        /// <summary>
        /// Monster's move to another Hex
        /// </summary>
        /// <param name="destination"> the destination Hex</param>
        /// <param name="animationTime"> Animation time. </param>
        public virtual void Move(int destination)
        {
            HexTile hexTile = world.tileMap.getHexTileByIndex(destination);
            gameObject.transform.LookAt(hexTile.transform.position);
            GetComponent<Animator>().SetTrigger("Walk");
            AnimationFinishedTime = Time.time + moving_animation_time;
            ActionParam.MoveDestination = destination;
            WaitForAnimation = MonsterAction.Move;
        }        
        
        /// <summary>
        /// Monster's move to another Hex With a Path
        /// </summary>
        /// <param name="destination"> the destination Hex</param>
        /// <param name="animationTime"> Animation time. </param>
        public virtual void Move(List<int> path)
        {
            if (path.Count == 0)
                return; 
            int startHex = path[0];
            moving_path.Clear();
            for (int i = 1; i < path.Count; i++)
                moving_path.Add(path[i]);
            Move(path[0]);
        }

        /// <summary>
        /// Attack another monster
        /// </summary>
        /// <param name="subjectMonster"> the attack subject </param>
        /// <param name="atk"> attack value. Default value is -1 (The original ATK value will be used in this case).  </param>
        /// <param name="animationTime"> Animation time. </param>
        public virtual void Attack(Monster subjectMonster, int atk = -1)
        {
            gameObject.transform.LookAt(subjectMonster.transform.position);
            if (atk < 0) atk = this.ATK;
            GetComponent<Animator>().SetTrigger("Attack");
            AnimationFinishedTime = Time.time + attack_animation_time;
            ActionParam.AttackDamage = atk;
            ActionParam.AttackSubject = subjectMonster;
            WaitForAnimation = MonsterAction.Attack;
        }

        /// <summary>
        /// this function will transmit all states of the monster to its respective GameObject (with visualization)
        /// </summary>
        /// <param name="StateField">
        /// "Damage", HP-=newState
        /// "Move", newState=next HexIndex Index
        /// "Exile", All state=0 and @isExile=true, @isAlive=false
        /// </param>
        public virtual void StateUpdate(string StateField, int newState)
        {
            if (StateField== "TornadoMove")
            {
                HexTile hexTile = world.tileMap.getHexTileByIndex(newState);
                if (hexTile == null)
                {
                    Debug.Log("Monster Error: Move to a null HexTile!");
                    return;
                }
                gameObject.transform.LookAt(hexTile.transform.position);
                gameObject.transform.SetParent(hexTile.transform);
                //gameObject.transform.localPosition.Set(0.0f, 0.0f, -1.5f);
                transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                HexIndex = newState;
                //Check TerrainEffect After every movement
                CheckTerrainEffect();
                if (moving_path.Count != 0)
                {
                    moving_path.Clear();
                    WaitForAnimation = MonsterAction.Nothing;
                }
            }

            if (StateField == "Damage")
            {
                HP -= newState;
                healthBar.SetHealth(this.HP);
                Debug.LogFormat("Monster {0} get hitted. Current HP is: {1}", this.uid, this.HP);
                if (HP <= 0)
                {
                    isAlive = false;
                    gameObject.GetComponent<Animator>().SetTrigger("Death");
                }
                else
                {
                    gameObject.GetComponent<Animator>().SetTrigger("Damage");
                }

            }
            if (StateField == "Move")
            {
                HexTile hexTile = world.tileMap.getHexTileByIndex(newState);
                if (hexTile == null)
                {
                    Debug.Log("Monster Error: Move to a null HexTile!");
                    return;
                }
                gameObject.transform.LookAt(hexTile.transform.position);
                gameObject.transform.SetParent(hexTile.transform);
                //gameObject.transform.localPosition.Set(0.0f, 0.0f, -1.5f);
                transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                HexIndex = newState;
                //Check TerrainEffect After every movement
                CheckTerrainEffect();
                if(moving_path.Count != 0)
                {
                    int nextHex = moving_path[0];
                    moving_path.RemoveAt(0);
                    Move(nextHex);
                }
            }
            if (StateField == "Exile")
            {
                isExiled = true;
                isAlive = false;
                HP = 0;
                ATK = 0;
                SPD = 0;
            }
        }
        
        // <summary>
        /// Check the possibility of moving to another Hex
        /// </summary>
        /// <param name="destination"> the destination Hex</param>
        public bool CanReach(int destination){
            //TODO
            return true;
        }

        /// <summary>
        /// Invoked when player select this monster
        /// </summary>
        public virtual void OnSelected()
        {
            if(SkillCoolDownCounter>0)
                SkillCoolDownCounter--;
            if (SkillCoolDownCounter <= 0)
            {
                canSkill = true;
            }
            return;
        }
        #endregion
    }

    /// <summary>
    /// Implement a comparator for Monster
    /// </summary>
    public class MonsterComparator : Comparer<Monster>
    {
        override
        public int Compare(Monster m1, Monster m2)
        {
            if (m1.SPD != m2.SPD)
                return m1.SPD - m2.SPD;
            else
                return m1.GetUId() - m2.GetUId();
        }
    }


}
