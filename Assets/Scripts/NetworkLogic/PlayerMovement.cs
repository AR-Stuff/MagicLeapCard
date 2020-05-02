using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Bolt.EntityBehaviour<ICubeState>
{
    /// <summary>
    /// Just Like Monobehavior.Start()
    /// Invoked when Bolt is aware of this entity and all internal state has been setup
    /// </summary>
    public override void Attached()
    {
        base.Attached();
        state.SetTransforms(state.CubeTransform, transform);

        if(entity.IsOwner)
        {
            state.CubeColor = new Color(Random.value, Random.value, Random.value);
        }

        state.AddCallback("CubeColor", ColorChanged);
    }

    // void Update()
    /// <summary>
    /// Invoked each simulation step on the owner
    /// </summary>
    public override void SimulateOwner()
    {
        base.SimulateOwner();
        var speed = 4f;
        var movement = new Vector3(Random.value - 0.5f, 0.0f, Random.value - 0.5f);
        if(movement!=Vector3.zero)
        {
            transform.position += movement * speed * BoltNetwork.FrameDeltaTime;
        }
    }

    void ColorChanged()
    {
        GetComponent<Renderer>().material.color = state.CubeColor;
    }

    private void OnGUI()
    {
        if(entity.IsOwner)
        {
            GUI.color = state.CubeColor;
            GUILayout.Label("@@@");
            GUI.color = Color.white;
        }
    }
}
