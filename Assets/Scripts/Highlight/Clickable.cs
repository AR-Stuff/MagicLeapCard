using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    /// <summary>
    /// Private Variable to get wether this hex is highlights ornot
    /// </summary>
    private bool _highlighted=false;

    /// <summary>
    /// Public Interface to check wether 
    /// </summary>
    public bool HightLighted { get { return _highlighted; } }

    /// <summary>
    /// Public Interface to Set one object to highlight
    /// </summary>
    public virtual void Highlighted()
    {
        if(!_highlighted)
        {
            _highlighted = true;
        }
    }

    /// <summary>
    /// Public Interface to Set one object to un_highlight
    /// </summary>
    public virtual void unHighlighted()
    {
        if(_highlighted)
        {
            _highlighted = false;
        }
    }
}
