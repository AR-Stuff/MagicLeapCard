using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainScanning;

public class ClickableHex : Clickable
{
    public enum HexHightLightStatus
    {
        DEFAULT, //UnHighlighten
        HIGHTLIGHT, //Over Hover
        OCCUPIED, //Been OCCUPIED
        MOVEMENT,
        ATTACK,
        ERROR
    };

    #region Private Variable
    /// <summary>
    /// Get access to the HexTile
    /// </summary>
    private HexTile _myTile;

    /// <summary>
    /// Store the Hight Light Status
    /// </summary>
    private HexHightLightStatus _hightlightstatus = HexHightLightStatus.DEFAULT;
    /// <summary>
    /// If one hex is unhighlight, it will change back to preview
    /// </summary>
    private HexHightLightStatus _priviewhightlightstatus = HexHightLightStatus.DEFAULT;
    #endregion

    #region Public Interface
    /// <summary>
    /// Public Interface to get the High Light Status
    /// </summary>
    public HexHightLightStatus HightLightStatus { get { return _hightlightstatus; } }
    /// <summary>
    /// Public Reference to LineRender
    /// </summary>
    public LineRenderer HightLightRender;
    /// <summary>
    /// Public Variable to store the Radius of the hex
    /// </summary>
    public float HexRadius;
    #endregion

    #region Unity Function
    public void Start()
    {
        if(HightLightRender==null)
        {
            HightLightRender = transform.Find("range").GetComponent<LineRenderer>();
        }

        if(_myTile==null)
        {
            _myTile = gameObject.GetComponent<HexTile>();
        }

        if(HightLightRender)
        {
            HightLightRender.startWidth = 0.01f;
            HightLightRender.endWidth = 0.01f;
            for (int vert = 0; vert <= 6; vert++)
                HightLightRender.SetPosition(vert, Corner(new Vector3(transform.position.x, transform.position.y+0.05f, transform.position.z), HexRadius, vert));
            SetColor();
        }
    }
    #endregion

    #region Public Function
    /// <summary>
    /// Get Hex Conrner Position
    /// </summary>
    public static Vector3 Corner(Vector3 origin, float radius, int corner)
    {
        float angle = 60 * corner;
        angle += 30;
        angle *= Mathf.PI / 180;
        return new Vector3(origin.x + radius * Mathf.Cos(angle), origin.y, origin.z + radius * Mathf.Sin(angle));//y was 0.0
    }

    /// <summary>
    /// When Beam focus on a hex
    /// </summary>
    public override void Highlighted()
    {
        base.Highlighted();
        if(_hightlightstatus!=HexHightLightStatus.HIGHTLIGHT)
        {
            _priviewhightlightstatus = _hightlightstatus;
        }
        _hightlightstatus = HexHightLightStatus.HIGHTLIGHT;
        SetColor();
    }

    /// <summary>
    /// When Set other color
    /// </summary>
    /// <param name="lightStatus">Other color</param>
    public void Highlighted(HexHightLightStatus lightStatus)
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
        _priviewhightlightstatus = HexHightLightStatus.DEFAULT;
        SetColor();
    }
    #endregion

    #region Private Function
    /// <summary>
    /// Set Color, private function
    /// </summary>
    private void SetColor()
    {
        if (HightLightRender == null) return;

        switch (_hightlightstatus)
        {
            case HexHightLightStatus.DEFAULT:
                HightLightRender.enabled = false;
                HightLightRender.startColor = Color.black;
                HightLightRender.endColor = Color.black;
                break;
            case HexHightLightStatus.HIGHTLIGHT:
                HightLightRender.enabled = true;
                HightLightRender.startColor = Color.blue;
                HightLightRender.endColor = Color.blue;
                break;
            case HexHightLightStatus.OCCUPIED:
                HightLightRender.enabled = true;
                HightLightRender.startColor = Color.black;
                HightLightRender.endColor = Color.black;
                break;
            case HexHightLightStatus.MOVEMENT:
                HightLightRender.enabled = true;
                HightLightRender.startColor = Color.green;
                HightLightRender.endColor = Color.green;
                break;
            case HexHightLightStatus.ATTACK:
                HightLightRender.enabled = true;
                HightLightRender.startColor = Color.red;
                HightLightRender.endColor = Color.red;
                break;
            case HexHightLightStatus.ERROR:
                HightLightRender.enabled = true;
                HightLightRender.startColor = Color.black;
                HightLightRender.endColor = Color.black;
                break;
        }
    }
    #endregion
}
