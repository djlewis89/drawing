using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class LineDrawing : MonoBehaviour 
{
	public Material DefaultMaterial;
    public float drawInterval = 0.1f;
    public float lineWidth = 0.05f;

	private bool draw = false;

	private GameObject current = null;
	private LineRenderer line = null;
    private List<Vector3> points = null;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    private float lastDrawTime = 0.0f;

    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate () 
	{
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
			StartDrawing ();

		if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
			StopDrawing ();

		if (draw)
			Draw ();
	}

	void StartDrawing()
	{
		draw = true;

		current = new GameObject ();
		line = current.AddComponent<LineRenderer> ();
		line.material = DefaultMaterial;
        line.SetWidth(lineWidth, lineWidth);

		points = new List<Vector3>();
	}

	void Draw()
	{
        //if (Time.time - lastDrawTime < drawInterval)
            //return;

        lastDrawTime = Time.time;

        points.Add(device.transform.pos);

        line.SetVertexCount (points.Count);
        line.SetPositions (points.ToArray ());        
	}

	void StopDrawing()
	{
		draw = false;

		current = null;
		line = null;

        lastDrawTime = 0.0f;
	}
}
