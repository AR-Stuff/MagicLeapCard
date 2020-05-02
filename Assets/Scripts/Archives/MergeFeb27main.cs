//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GameLogic;
//using TerrainScanning;
//public enum MergeFeb27States
//{
//    Init,
//    Scan,
//    Player1,
//    Player2,
//    Battle,
//    End
//};

//public class MergeFeb27main : MonoBehaviour
//{
//    #region Public Variable
//    /// <summary>
//    /// Store a Pointer to ScanMesh to capture the ScanFinished event
//    /// </summary>
//    public meshCode ScanMesh;

//    /// <summary>
//    /// Store a Pointer to Player1 to send playerstart event and scanfinished event
//    /// </summary>
//    public Player Player1;

//    /// <summary>
//    /// Store a Pointer to Player2 which is a AI Player
//    /// </summary>
//    public Player Player2;
//    #endregion

//    #region Private Variable
//    /// <summary>
//    /// Store the state of the game logic
//    /// </summary>
//    private MergeFeb27States myState = MergeFeb27States.Init;

//    /// <summary>
//    /// Battle Start Time
//    /// </summary>
//    private float BattleStartTime;
//    #endregion

//    // Start is called before the first frame update
//    void Start()
//    {
//        ScanMesh.ScanFinished += ScanFinshedUpdate;
//        Player1.PlayerEnd += PlayerEndUpdate;
//        Player2.PlayerEnd += PlayerEndUpdate;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        float NowTime = Time.time;
//        switch(myState)
//        {
//            case (MergeFeb27States.Init):
//                myState = MergeFeb27States.Scan;
//                break;
//            case (MergeFeb27States.Scan):
//                break;
//            case (MergeFeb27States.Player1):
//                Player1.Event_PlayerTurnStart.Invoke();
//                break;
//            case (MergeFeb27States.Player2):
//                Player2.Event_PlayerTurnStart.Invoke();
//                break;
//            case (MergeFeb27States.Battle):
//                Debug.Log("BattleTime!!!");
//                if(NowTime-BattleStartTime>5)
//                {
//                    myState = MergeFeb27States.End;
//                }
//                break;
//            case (MergeFeb27States.End):
//                Debug.Log("EndTurn,Go To Next Turn");
//                myState = MergeFeb27States.Player1;
//                break;
//            default:
//                break;            
//        }
//    }

//    void ScanFinshedUpdate()
//    {
//        Debug.Log("ScanFinished");
//        Player1.Event_ScanFinished.Invoke();
//        Player2.Event_ScanFinished.Invoke();
//        if(myState==MergeFeb27States.Scan)
//        {
//            myState = MergeFeb27States.Player1;
//        }
//        return;
//    }

//    void PlayerEndUpdate(int PlayerId)
//    {
//        if (PlayerId == 1)
//        {
//            if (myState == MergeFeb27States.Player1)
//                myState = MergeFeb27States.Player2;
//        }
//        if (PlayerId == 2)
//        {
//            if (myState == MergeFeb27States.Player2)
//            {
//                BattleStartTime = Time.time;
//                myState = MergeFeb27States.Battle;
//            }
//        }
//    }
//}
