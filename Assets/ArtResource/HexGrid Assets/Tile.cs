using UnityEngine;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour {
	public CubeIndex index;
    private LineRenderer lines;
    private LineRenderer rangeIndicator;
    private Grid myGrid;

    [SerializeField]
	private bool occupied = false;

    private enum OutlineState
	{
        DEFAULT,
        MOUSEOVER,
        //SELECTED,
        OCCUPIED,
        MOVEMENT,
        ATTACK,
	}

	private OutlineState outlineInteract = OutlineState.DEFAULT;
    private OutlineState previousOutline = OutlineState.DEFAULT;

    private void Start()
    {

        //move the line renderer to where the hex actually is 
        lines = GetComponent<LineRenderer>();
        rangeIndicator = transform.Find("range").GetComponent<LineRenderer>();

        myGrid = GetComponentInParent<Grid>();

        rangeIndicator.startWidth = 0.02f;
        rangeIndicator.endWidth = 0.02f;
        lines.startWidth = 0.04f;
        lines.endWidth = 0.04f;
        for (int vert = 0; vert <= 6; vert++)
            rangeIndicator.SetPosition(vert, Tile.Corner(new Vector3(transform.position.x, transform.position.y, transform.position.z), GetComponentInParent<Grid>().hexRadius, vert, HexOrientation.Pointy));

        for (int vert = 0; vert <= 6; vert++)
            lines.SetPosition(vert, Tile.Corner(new Vector3(transform.position.x,transform.position.y+1,transform.position.z), 1, vert, HexOrientation.Pointy));//1 = hexRadius, 
        lines.enabled = false;
        SetColor();
        //SetInRange(false);
    }

    //use this to make the lines move correctly
    public void PositionUpdated()
    {
        for (int vert = 0; vert <= 6; vert++)
            rangeIndicator.SetPosition(vert, Tile.Corner(new Vector3(transform.position.x, transform.position.y, transform.position.z), GetComponentInParent<Grid>().hexRadius, vert, HexOrientation.Pointy));
        for (int vert = 0; vert <= 6; vert++)
            lines.SetPosition(vert, Tile.Corner(transform.position, GetComponentInParent<Grid>().hexRadius, vert, HexOrientation.Pointy));//1 = hexRadius, 
    }
    public void PositionUpdatedPlus(float rot)
    {
        for (int vert = 0; vert <= 6; vert++)
            rangeIndicator.SetPosition(vert, Tile.CornerPlus(new Vector3(transform.position.x, transform.position.y, transform.position.z), myGrid.hexRadius, vert, HexOrientation.Pointy, rot));
        for (int vert = 0; vert <= 6; vert++)
            lines.SetPosition(vert, Tile.CornerPlus(transform.position, myGrid.hexRadius, vert, HexOrientation.Pointy,rot));//1 = hexRadius, 
    }

    public void SetInRange(bool inRange)//in range of attack or movement?  movement should be blue or green or something while attack is red
    {
        if (inRange)
        {
            rangeIndicator.enabled = true;
        }
        else
            rangeIndicator.enabled = false;
    }

    //TODO: this function should switch the range indicator between the attack color and the move color
    public void SetColor()
    {
		switch (outlineInteract)
		{
			case OutlineState.DEFAULT:
				rangeIndicator.startColor = Color.black;
				rangeIndicator.endColor = Color.black;
				break;
			case OutlineState.MOUSEOVER:
				rangeIndicator.startColor = Color.blue;
				rangeIndicator.endColor = Color.blue;
				break;
			//case OutlineState.SELECTED:
			//	rangeIndicator.startColor = Color.blue;
			//	rangeIndicator.endColor = Color.blue;
				//break;
			case OutlineState.OCCUPIED:
                rangeIndicator.startColor = Color.black;
                rangeIndicator.endColor = Color.black;
				break;
			case OutlineState.MOVEMENT:
				rangeIndicator.startColor = Color.green;
				rangeIndicator.endColor = Color.green;
				break;
			case OutlineState.ATTACK:
				rangeIndicator.startColor = Color.red;
				rangeIndicator.endColor = Color.red;
				break;
		}
    }
    
    public void Highlight(string interactState)
	{
        Enum.TryParse(interactState, out outlineInteract);
		SetColor();
	}

    public void Highlight() // Specifically for "mouseover"
    {
        if (outlineInteract != OutlineState.MOUSEOVER)
        {
            previousOutline = outlineInteract;
        }
        outlineInteract = OutlineState.MOUSEOVER;
		SetColor();
        //lines.enabled = true;//turns on the thick white selection highlight
    }
    public void UnHighlight() // ***specifically for "mouseover"***
    {
        outlineInteract = previousOutline;
        previousOutline = OutlineState.DEFAULT;
		SetColor();
        //lines.enabled = false;
    }


    public bool GetOccupied()
	{
		return occupied;
	}

    public void SetOccupied()
	{
		outlineInteract = OutlineState.OCCUPIED;
		occupied = true;
	}
    public void SetUnoccupied()
	{
		outlineInteract = OutlineState.DEFAULT;
		occupied = false;
	}



    public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation){
		float angle = 60 * corner;
		if(orientation == HexOrientation.Pointy)//add rotation.y maybe
			angle += 30;
		angle *= Mathf.PI / 180;
		return new Vector3(origin.x + radius * Mathf.Cos(angle), origin.y, origin.z + radius * Mathf.Sin(angle));//y was 0.0
	}

    public static Vector3 CornerPlus(Vector3 origin, float radius, int corner, HexOrientation orientation,float yRot)
    {
        float angle = 60 * corner;
        if (orientation == HexOrientation.Pointy)//add rotation.y maybe
            angle += 30;
        //Debug.Log("yrot: " + yRot);
        angle -= yRot;
        angle *= Mathf.PI / 180;
        return new Vector3(origin.x + radius * Mathf.Cos(angle), origin.y, origin.z + radius * Mathf.Sin(angle));//y was 0.0
    }

    public static void GetHexMesh(float radius, HexOrientation orientation, ref Mesh mesh) {
		mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < 6; i++)
			verts.Add(Corner(Vector3.zero, radius, i, orientation));

		tris.Add(0);
		tris.Add(2);
		tris.Add(1);
		
		tris.Add(0);
		tris.Add(5);
		tris.Add(2);
		
		tris.Add(2);
		tris.Add(5);
		tris.Add(3);
		
		tris.Add(3);
		tris.Add(5);
		tris.Add(4);

		//UVs are wrong, I need to find an equation for calucalting them
		uvs.Add(new Vector2(0.5f, 1f));
		uvs.Add(new Vector2(1, 0.75f));
		uvs.Add(new Vector2(1, 0.25f));
		uvs.Add(new Vector2(0.5f, 0));
		uvs.Add(new Vector2(0, 0.25f));
		uvs.Add(new Vector2(0, 0.75f));

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.name = "Hexagonal Plane";

		mesh.RecalculateNormals();
	}

	#region Coordinate Conversion Functions
	public static OffsetIndex CubeToEvenFlat(CubeIndex c) {
		OffsetIndex o;
		o.row = c.x;
		o.col = c.z + (c.x + (c.x&1)) / 2;
		return o;
	}

	public static CubeIndex EvenFlatToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col;
		c.z = o.row - (o.col + (o.col&1)) / 2;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToOddFlat(CubeIndex c) {
		OffsetIndex o;
		o.col = c.x;
		o.row = c.z + (c.x - (c.x&1)) / 2;
		return o;
	}
	
	public static CubeIndex OddFlatToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col;
		c.z = o.row - (o.col - (o.col&1)) / 2;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToEvenPointy(CubeIndex c) {
		OffsetIndex o;
		o.row = c.z;
		o.col = c.x + (c.z + (c.z&1)) / 2;
		return o;
	}
	
	public static CubeIndex EvenPointyToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col - (o.row + (o.row&1)) / 2;
		c.z = o.row;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToOddPointy(CubeIndex c) {
		OffsetIndex o;
		o.row = c.z;
		o.col = c.x + (c.z - (c.z&1)) / 2;
		return o;
	}
	
	public static CubeIndex OddPointyToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col - (o.row - (o.row&1)) / 2;
		c.z = o.row;
		c.y = -c.x - c.z;
		return c;
	}

	public static Tile operator+ (Tile one, Tile two){
		Tile ret = new Tile();
		ret.index = one.index + two.index;
		return ret;
	}

	public void LineColour(Color colour) {
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
        {
            lines.startColor = colour;
            lines.endColor = colour;
        }
	}

	public void LineColour(Color start, Color end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
        {
            lines.startColor = start;
            lines.endColor = end;
        }
	}

	public void LineWidth(float width){
		LineRenderer lines = GetComponent<LineRenderer>();
        if(lines)
        {
            lines.startWidth = width;
            lines.endWidth = width;
        }
	}

	public void LineWidth(float start, float end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
        {
            lines.startWidth = start;
            lines.endWidth = end;
        }
    }
	#endregion

	#region A* Herustic Variables
	public int MoveCost()
	{
		return 1; // can use to set movement cost over terrain in future if wanted
	}
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost { get { return GCost + HCost; } }
	public Tile Parent { get; set; }
	#endregion
}

[System.Serializable]
public struct OffsetIndex {
	public int row;
	public int col;

	public OffsetIndex(int row, int col){
		this.row = row; this.col = col;
	}
}

[System.Serializable]
public struct CubeIndex {
	public int x;
	public int y;
	public int z;

	public CubeIndex(int x, int y, int z){
		this.x = x; this.y = y; this.z = z;
	}

	public CubeIndex(int x, int z) {
		this.x = x; this.z = z; this.y = -x-z;
	}

	public static CubeIndex operator+ (CubeIndex one, CubeIndex two){
		return new CubeIndex(one.x + two.x, one.y + two.y, one.z + two.z);
	}

	public override bool Equals (object obj) {
		if(obj == null)
			return false;
		CubeIndex o = (CubeIndex)obj;
		if((System.Object)o == null)
			return false;
		return((x == o.x) && (y == o.y) && (z == o.z));
	}

	public override int GetHashCode () {
		return(x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5))/2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
	}

	public override string ToString () {
		return string.Format("[" + x + "," + y + "," + z + "]");
	}
}