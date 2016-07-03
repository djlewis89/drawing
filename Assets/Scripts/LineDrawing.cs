using UnityEngine;
using System.Collections.Generic;

public class LineDrawing : MonoBehaviour 
{
	public Material DefaultMaterial;

	private bool draw = false;

	private GameObject current = null;
	private LineRenderer line = null;

	//private float lastPointTime = 0.0f;
	private List<Vector3> points = null;
	
	void Update () 
	{
		if (Input.GetMouseButtonDown (0))
			StartDrawing ();

		if (Input.GetMouseButtonUp (0))
			StopDrawing ();

		if (draw)
			Draw ();
	}

	void StartDrawing()
	{
		draw = true;

		current = new GameObject ();
		current.transform.parent = transform;
		line = current.AddComponent<LineRenderer> ();
		line.material = DefaultMaterial;

		points = new List<Vector3>();
	}

	void Draw()
	{
		points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

		line.SetVertexCount (points.Count);
		line.SetPositions (points.ToArray ());
	}

	void StopDrawing()
	{
		draw = false;

		current = null;
		line = null;
	}
}
