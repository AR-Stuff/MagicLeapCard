using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using GameLogic;
using Monsters;

namespace GameWorld
{
    public delegate void World_ResetFinished();

    public enum WorldStates
    {
        Idle,
        Battle,
        Wait_For_Monster,
        Wait_For_Player,
        Summary
    }


    public class WorldGlobalAttrib
    {
        public int currentTurn = 0;
    }

    public delegate void BattleEnd(int winner);

    public class World : MonoBehaviour
    {
        #region Static Variable
        /// <summary>
        /// Static value to store the only _instant
        /// </summary>
        private static World _instant;

        /// <summary>
        /// Public Interface to get the World Instant
        /// </summary>
        /// <returns>Return the World Instant</returns>
        public static World GetInstant()
        {
            return _instant;
        }
        #endregion

        /// <summary>
        /// data structure of all monsters
        /// </summary>
        public Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
        public Dictionary<int, Player> players = new Dictionary<int, Player>();
        public Dictionary<int, InteractiveTerrain> terrains = new Dictionary<int, InteractiveTerrain>();

        public List<PermanentObstacle> obstacles = new List<PermanentObstacle>();

        /// <summary>
        /// the game map
        /// </summary>
        public HexTileMap tileMap;
        /// <summary>
        /// The global informations of the world
        /// </summary>
        public WorldGlobalAttrib worldAttrib;

        public BattleEnd BattleEnd;
        public UnityEvent Event_BattleBegin;

        public World_ResetFinished World_ResetFinished;

        public UnityEvent Event_ResetStart;

        public bool created = false;

        private float steptime;
        private float stateBeginTime;

        /// <summary>
        /// A link to the gameManager
        /// </summary>
        [Tooltip("A link to the gameManager.")]
        public GameManager gameManager;

        private WorldStates currentState = WorldStates.Idle;
        private SortedSet<Monster> actingMonsterList;
        private Monster currentActMonster;
        private List<Player> actingPlayerList;
        private Player currentActPlayer;

        #region Public Function
        /// <summary>
        /// Return the terrain effect of the hextile
        /// </summary>
        /// <param name="HexTileIndex">The index of HexTile</param>
        public InteractiveTerrain CheckTerrainEffect(int HexTileIndex)
        {
            InteractiveTerrain terrain = null;
            foreach(var itr in terrains)
            {
                if (itr.Value.HexIndex == null) continue;
                foreach(var hexs in itr.Value.HexIndex)
                {
                    if(hexs==HexTileIndex)
                    {
                        terrain = itr.Value;
                        return terrain;
                    }
                }
            }
            return terrain;
        }

        /// <summary>
        /// Register New Obstacle
        /// </summary>
        /// <param name="obstacle">New Obstacle</param>
        public void RegisterPermanentObstacle(PermanentObstacle obstacle)
        {
            foreach(var itr in obstacles)
            {
                if (itr == obstacle) return;
            }
            obstacles.Add(obstacle);
            return;
        }

        /// <summary>
        /// HighLight all monster moveable zone
        /// </summary>
        /// <param name="monster">The selected monster</param>
        public void HighLightMovementZone(Monster monster)
        {
            foreach(var itr in tileMap.CoordToHexTile)
            {
                TerrainScanning.HexTile hex = itr.Value;
                if (hex == null) continue;
                if (hex.gameObject == null) continue;
                ClickableHex clickable=hex.gameObject.GetComponent<ClickableHex>();
                if (clickable == null) continue;
                if(hex.isObstacle==true)
                {
                    clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                }
                else
                {
                    int Dist = tileMap.getDistance(monster.HexIndex, hex.getID());
                    if (Dist <= monster.MaxMovementRange)
                    {
                        if(hex.isMonsterOn==true)
                        {
                            if(hex.monster.monsterOwner==monster.monsterOwner)
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                            }
                            else
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ATTACK);
                            }
                        }
                        else
                        {
                            clickable.Highlighted(ClickableHex.HexHightLightStatus.MOVEMENT);
                        }
                    }
                    else if (Dist <= monster.MaxMovementRange + monster.MaxAttackRange)
                    {
                        if (hex.isMonsterOn == true)
                        {
                            if (hex.monster.monsterOwner == monster.monsterOwner)
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                            }
                            else
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ATTACK);
                            }
                        }
                        else
                        {
                            clickable.Highlighted(ClickableHex.HexHightLightStatus.ATTACK);
                        }
                    }
                    else
                    {
                        clickable.Highlighted(ClickableHex.HexHightLightStatus.DEFAULT);
                    }
                }
            }
        }

        /// <summary>
        /// HighLight all monster attackable zone
        /// </summary>
        /// <param name="monster">The selected monster</param>
        public void HighLightAttackZone(Monster monster)
        {
            foreach (var itr in tileMap.CoordToHexTile)
            {
                TerrainScanning.HexTile hex = itr.Value;
                if (hex == null) continue;
                if (hex.gameObject == null) continue;
                ClickableHex clickable = hex.gameObject.GetComponent<ClickableHex>();
                if (clickable == null) continue;
                if (hex.isObstacle == true)
                {
                    clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                }
                else
                {
                    int Dist = tileMap.getDistance(monster.HexIndex, hex.getID());
                    if (Dist < monster.MinAttackRange)
                    {
                        clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                    }
                    else if(Dist<=monster.MaxAttackRange)
                    {
                        if (hex.isMonsterOn == true)
                        {
                            if (hex.monster.monsterOwner == monster.monsterOwner)
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                            }
                            else
                            {
                                clickable.Highlighted(ClickableHex.HexHightLightStatus.ATTACK);
                            }
                        }
                        else
                        {
                            clickable.Highlighted(ClickableHex.HexHightLightStatus.ATTACK);
                        }
                    }
                    else
                    {
                        clickable.Highlighted(ClickableHex.HexHightLightStatus.DEFAULT);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Set all highlight to original
        /// </summary>
        public void UnHighLightAll()
        {
            foreach (var itr in tileMap.CoordToHexTile)
            {
                TerrainScanning.HexTile hex = itr.Value;
                if (hex == null) continue;
                if (hex.gameObject == null) continue;
                ClickableHex clickable = hex.gameObject.GetComponent<ClickableHex>();
                if (clickable == null) continue;
                if (hex.isObstacle == true)
                {
                    clickable.Highlighted(ClickableHex.HexHightLightStatus.ERROR);
                }
                else
                {
                    clickable.Highlighted(ClickableHex.HexHightLightStatus.DEFAULT);
                }
            }
        }

        /// <summary>
        /// Set all hextile accessible of all obstacle
        /// </summary>
        public void SetAllObstacle()
        {
            foreach(var itr in obstacles)
            {
                foreach(var coord in itr.BlockHexCoord)
                {
                    GameWorld.HexTileMap.HexCoord tempCoord = new HexTileMap.HexCoord();
                    tempCoord.X = coord.X;
                    tempCoord.Y = coord.Y;
                    tempCoord.Z = coord.Z;
                    if(tileMap.CoordToHexTile.ContainsKey(tempCoord))
                    {
                        tileMap.CoordToHexTile[tempCoord].isObstacle = true;
                    }
                }
            }
            UnHighLightAll();
        }

        /// <summary>
        /// Upload the information of the monster to the world(Monsters)
        /// </summary>
        /// <param name="monster">A link to the monster</param>
        /// <returns>monster uid if success, -1 is error</returns>
        public int uploadMonsterInWorld(Monster monster)
        {
            int uid = monster.GetUId();
            if (monsters.ContainsKey(uid) == true)
            {
                Debug.LogErrorFormat("Error! Monster with uid {0} already exists", uid);
                return -1;
            }
            monsters[uid] = monster;
            monster.MonsterTurnEnd += MonsterMoveEnd;
            return uid;
        }

        public int uploadTerrainInWorld(InteractiveTerrain interactiveTerrain)
        {
            int uid = interactiveTerrain.UniqueIndex;
            if(terrains.ContainsKey(uid)==true)
            {
                Debug.LogErrorFormat("Error! Terrain with uid {0} already exists", uid);
                return -1;
            }
            terrains[uid] = interactiveTerrain;
            return uid;
        }

        /// <summary>
        /// Upload the information of the monster to the world(Monsters)
        /// </summary>
        /// <param name="monster">A link to the monster</param>
        /// <returns>monster uid if success, -1 is error</returns>
        public int uploadPlayerInWorld(Player player)
        {
            int playerId = player.GetPlayerId();
            if (players.ContainsKey(playerId) == true)
            {
                Debug.LogErrorFormat("Error! Monster with uid {0} already exists", playerId);
                return -1;
            }
            players[playerId] = player;
            player.Battle_PlayerEnd += PlayerTurnEnd;
            return playerId;
        }

        /// <summary>
        /// get monster instance by its uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// return the monster if it exists, otherwise return null 
        public Monster getMonsterByID(int uid)
        {
            if (monsters.ContainsKey(uid) == false)
            {
                Debug.LogErrorFormat("Error! No monster has uid {0}", uid);
                return null;
            }
            else
                return monsters[uid];
        }

        /// <summary>
        /// get player instance by its uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// return the player if it exists, otherwise return null 
        public Player getPlayerById(int playerId)
        {
            if (players.ContainsKey(playerId) == false)
            {
                Debug.LogErrorFormat("Error! No player has uid {0}", playerId);
                return null;
            }
            else
                return players[playerId];
        }
        ///


        private void ChangeState(WorldStates dstStates)
        {
            Debug.LogFormat("Game Manager: Now Change to " + dstStates);
            currentState = dstStates;
            stateBeginTime = Time.time;
        }

        #endregion

        #region Unity Function
        /// <summary>
        /// Invoke when World is enable
        /// </summary>
        void Start()
        {
            Debug.Log("Creating a new world");
            if(_instant==null)
            {
                _instant = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            actingMonsterList = new SortedSet<Monster>(new MonsterComparator());
            actingPlayerList = new List<Player>();
            if (tileMap == null)
                tileMap = new HexTileMap();
            worldAttrib = new WorldGlobalAttrib();
            if (Event_BattleBegin == null)
                Event_BattleBegin = new UnityEvent();
            Event_BattleBegin.AddListener(BattleBegin);

            //TODO Remove
            if (Event_ResetStart == null)
                Event_ResetStart = new UnityEvent();
            Event_ResetStart.AddListener(ResetBegin);
        }

        /// <summary>
        /// FSM for each tick
        /// </summary>
        void Update()
        {
            float nowTime = Time.time;
            //Debug.Log("WorldStates=" + currentState);
            switch (currentState)
            {
                case WorldStates.Idle:
                case WorldStates.Wait_For_Monster:
                    // do nothing
                    break;
                case WorldStates.Wait_For_Player:
                    if (nowTime - stateBeginTime > 1)
                    {
                        ChangeState(WorldStates.Battle);
                    }
                    break;
                case WorldStates.Battle:
                    currentActPlayer = actingPlayerList[0];
                    actingPlayerList.Remove(currentActPlayer);
                    Debug.LogFormat("Player {0} is taking control...", currentActPlayer.GetPlayerId());
                    ChangeState(WorldStates.Wait_For_Player);
                    currentActPlayer.Event_Battle_PlayerTurnStart.Invoke();
                    break;
                case WorldStates.Summary:
                    if (nowTime - steptime < 1) break;
                    WinningCheck();
                    break;
            }
        }
        #endregion

        #region Private Function
        /// <summary>
        /// For all Monster reset at the begin of each turn.
        /// </summary>
        private void TurnInit()
        {
            Debug.Log("World: Reseted All Monster.");
            foreach (KeyValuePair<int, Monster> monsterPair in monsters)
            {
                Monster thisMonster = monsterPair.Value;
                if (thisMonster.isExiled) continue; //A Exiled Monster cannot be reset.
                thisMonster.MonsterReset();
            }
            Debug.Log("World: Finished init");
            World_ResetFinished();
        }

        /// <summary>
        /// Reset the ActingMonsterList Sorted Array before battle
        /// </summary>
        private void ResetForOneBattle()
        {
            worldAttrib.currentTurn = 0;
            ResetForOneTurn();
        }
        /// <summary>
        /// Reset the ActingMonsterList Sorted Array.
        /// </summary>
        private void ResetForOneTurn()
        {
            actingMonsterList.Clear();
            actingPlayerList.Clear();
            foreach (KeyValuePair<int, Monster> monsterPair in monsters)
            {
                Monster monster = monsterPair.Value;
                if (monster.isExiled == false && monster.isAlive == true)
                    actingMonsterList.Add(monster);
            }
            foreach (KeyValuePair<int, Player> playerPair in players)
            {
                Player player = playerPair.Value;
                actingPlayerList.Add(player);
            }
            Debug.LogFormat("Clearing for one turn ended. Detected {0} living animals.", actingMonsterList.Count);
            steptime = Time.time;
        }

        /// <summary>
        /// Winning Check, Invoke when this turn finished.
        /// </summary>
        private void WinningCheck()
        {
            ResetForOneTurn();
            int Player0Count = 0;
            int Player1Count = 0;
            foreach (Monster monster in actingMonsterList)
            {
                if (monster.monsterOwner == gameManager.Player0)
                {
                    Player0Count++;
                }
                else if (monster.monsterOwner == gameManager.Player1)
                {
                    Player1Count++;
                }
            }
            if (worldAttrib.currentTurn < 20 && Player0Count > 0 && Player1Count > 0)
            {
                worldAttrib.currentTurn++;
                ChangeState(WorldStates.Battle);
                return;
            }

            if (Player0Count == Player1Count)
            {//Draw
                Debug.Log("This turn Draw");
                BattleEnd(-1);
            }
            else if (Player0Count < Player1Count)
            {//Player1Win
                Debug.Log("Player 1 wins.");
                BattleEnd(1);
            }
            else
            {//Player0Win
                Debug.Log("Player 0 wins.");
                BattleEnd(0);
            }
            ChangeState(WorldStates.Idle);
            return;
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// Event_ResetStart Handler
        /// </summary>
        private void ResetBegin()
        {
            SetAllObstacle();
            TurnInit(); //Reset all monster at the begin of the turn.
            return;
        }

        /// <summary>
        /// Delegate MonsterMove End Handler
        /// </summary>
        private void MonsterMoveEnd()
        {
            Debug.LogFormat("Monster {0} move ended.", currentActMonster.GetUId());

            ChangeState(WorldStates.Battle);
        }
        /// <summary>
        /// Delegate PlayerTurn End Handler
        /// </summary>
        private void PlayerTurnEnd()
        {
            Debug.LogFormat("Player {0}'s turn ended.", currentActPlayer.GetPlayerId());

            ChangeState(WorldStates.Battle);
        }
        /// <summary>
        /// Invoke when Battle is begin by GameManager
        /// </summary>
        private void BattleBegin()
        {
            Debug.Log("World: Battle begin.");
            // ResetForOneBattle();
            ChangeState(WorldStates.Battle);
        }
        #endregion
    }

}
