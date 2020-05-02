using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monsters;

public class ClickableCharacter : Clickable
{
    public enum CharHightLightStatus
    {
        DEFAULT, //UnHighlighten
        HIGHTLIGHT, //Over Hover
        SELECTED //Been Selected
    };

    #region Private Variable
    /// <summary>
    /// Private Access to the monster class
    /// </summary>
    private Monster _myMonster;
    /// <summary>
    /// Store the Hight Light Status
    /// </summary>
    private CharHightLightStatus _hightlightstatus = CharHightLightStatus.DEFAULT;
    /// <summary>
    /// If one Character is unhighlight, it will change back to preview
    /// </summary>
    private CharHightLightStatus _priviewhightlightstatus = CharHightLightStatus.DEFAULT;
    #endregion

    #region Public Function

    /// <summary>
    /// When Beam focus on a character
    /// </summary>
    public override void Highlighted()
    {
        base.Highlighted();
        if (_hightlightstatus != CharHightLightStatus.HIGHTLIGHT)
        {
            _priviewhightlightstatus = _hightlightstatus;
        }
        _hightlightstatus = CharHightLightStatus.HIGHTLIGHT;
        SetColor();
    }

    /// <summary>
    /// When Set other color
    /// </summary>
    /// <param name="lightStatus">Other color</param>
    public void Highlighted(CharHightLightStatus lightStatus)
    {
        _hightlightstatus = lightStatus;
        SetColor();
    }

    /// <summary>
    /// when Beam doesn't focus on a hex
    /// </summary>
    public override void unHighlighted()
    {
        base.unHighlighted();
        _hightlightstatus = _priviewhightlightstatus;
        _priviewhightlightstatus = CharHightLightStatus.DEFAULT;
        SetColor();
    }
    #endregion

    #region Public Variable
    /// <summary>
    /// Public Interface to get the High Light Status
    /// </summary>
    public CharHightLightStatus HightLightStatus { get { return _hightlightstatus; } }
    /// <summary>
    /// Public Interface to get the selection circle
    /// </summary>
    public GameObject SelectionCircle;
    #endregion

    #region Private Function
    /// <summary>
    /// Set Color, private function
    /// </summary>
    private void SetColor()
    {
        if (SelectionCircle == null) return;

        switch (_hightlightstatus)
        {
            case CharHightLightStatus.DEFAULT:
                SelectionCircle.GetComponent<MeshRenderer>().enabled = false;
                break;
            case CharHightLightStatus.HIGHTLIGHT:
                SelectionCircle.GetComponent<MeshRenderer>().enabled = true;
                SelectionCircle.GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case CharHightLightStatus.SELECTED:
                SelectionCircle.GetComponent<MeshRenderer>().enabled = true;
                SelectionCircle.GetComponent<MeshRenderer>().material.color = Color.red;
                break;
        }
    }
    #endregion
}
