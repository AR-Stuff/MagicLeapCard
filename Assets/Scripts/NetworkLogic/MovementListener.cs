using UnityEngine;

//The listener to get the opponent of the opponent
//Add this listener to GameManager
public class MovementListener : Bolt.GlobalEventListener
{
    public override void OnEvent(GamaManagerMoveEvent evnt)
    {
        Debug.Log(evnt.PlayerMove);
        //Transfer json stirng into object.
        object movementobj = JsonUtility.FromJson<object>(evnt.PlayerMove);
        //..
    }
}
