using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bolt;

public class ReplicatedController : EntityBehaviour<IMagicLeapControllerState>
{
    #region Public Reference
    /// <summary>
    /// The LineRenderer to show the line from the input to the hit point.
    /// </summary>
    [SerializeField, Tooltip("The LineRenderer to show the line from the input to the hit point.")]
    private LineRenderer beamLine;
    #endregion

    #region Private Reference
    /// <summary>
    /// A private reference to get the real controller location
    /// </summary>
    private GameObject RealControllerObject = null;
    #endregion

    /// <summary>
    /// Just Like Monobehavior.Start()
    /// Invoked when Bolt is aware of this entity and all internal state has been setup
    /// </summary>
    public override void Attached()
    {
        base.Attached();
        state.SetTransforms(state.ControllerTransform, transform);
    }

    // void Update()
    /// <summary>
    /// Invoked each simulation step on the owner
    /// </summary>
    public override void SimulateOwner()
    {
        base.SimulateOwner();
        if(RealControllerObject==null)
        {
            RealControllerObject = GameObject.Find("Controller");
        }
        if(RealControllerObject)
        {
            this.transform.position = RealControllerObject.transform.position;
            this.transform.rotation = RealControllerObject.transform.rotation;
        }
    }

    void Update()
    {
        RaycastHit hit;

        if (true||Physics.Raycast(transform.position, transform.forward, out hit))
        {
            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, transform.position + transform.forward * 5);
        }

    }
}
