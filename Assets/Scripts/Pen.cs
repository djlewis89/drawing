using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    // Inspector Variables
    public Color Colour = Color.black;

    [Range(1, 20)]
    public int Width = 2;

    [Range(0.05f, 1.0f)]
    public float DrawDistance = 0.075f;

    [Range(0.0f, 1.0f)]
    public float Softness = 0.0f;

    // VR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    // State
    private Vector2? lastDrawPoint;
    private Vector2? lastErasePoint;
    private Vector3 originalForward;

    private GameObject indicator;

    void Start()
    {
        indicator = transform.FindChild("pencilRing").gameObject;
    }

    void FixedUpdate()
    {
        //Debug.DrawRay(transform.position + transform.forward, transform.forward);
        //Debug.DrawRay(transform.position - transform.forward, -transform.forward);

        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();

        if (trackedObj == null)
            return;

        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device == null)
            return;

        UpdateWidth();
        UpdateState();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, DrawDistance))
            Draw(hit);
        else
            lastDrawPoint = null;

        if (Physics.Raycast(transform.position, transform.forward, out hit, DrawDistance))
            Erase(hit);
        else
            lastErasePoint = null;
    }

    void Draw(RaycastHit hit)
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Whiteboard board = hit.transform.GetComponent<Whiteboard>();

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null || board == null)
            return;

        Texture2D tex = rend.sharedMaterial.mainTexture as Texture2D;

        if (tex == null)
            return;

        Vector2 point = CalculateDrawPoint(tex, hit.textureCoord);

        DrawPoint(tex, point, Width, Colour);

        if (lastDrawPoint.HasValue && Vector2.Distance(point, lastDrawPoint.Value) > 1)
            device.TriggerHapticPulse();

        if (lastDrawPoint.HasValue && Vector2.Distance(lastDrawPoint.Value, point) > Width) {
            var fillIterations = Mathf.Ceil(Vector2.Distance(lastDrawPoint.Value, point) / Width);
            var increment = (point - lastDrawPoint.Value) / fillIterations;

            for (int i = 1; i <= fillIterations; ++i) {
                DrawPoint(tex, lastDrawPoint.Value + (increment * i), Width, Colour);
            }
        }

        tex.Apply();

        lastDrawPoint = point;
    }

    void Erase(RaycastHit hit)
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Whiteboard board = hit.transform.GetComponent<Whiteboard>();

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null || board == null)
            return;

        Texture2D tex = rend.sharedMaterial.mainTexture as Texture2D;

        if (tex == null)
            return;

        var point = CalculateDrawPoint(tex, hit.textureCoord);

        DrawPoint(tex, point, Width, board.CanvasColor);

        if (lastErasePoint.HasValue && Vector2.Distance(point, lastErasePoint.Value) > 1)
            device.TriggerHapticPulse();

        if (lastErasePoint.HasValue && Vector2.Distance(lastErasePoint.Value, point) > Width) {
            var fillIterations = Mathf.Ceil(Vector2.Distance(lastErasePoint.Value, point) / Width);
            var increment = (point - lastErasePoint.Value) / fillIterations;

            for (int i = 1; i <= fillIterations; ++i) {
                DrawPoint(tex, lastErasePoint.Value + (increment * i), Width, board.CanvasColor);
            }
        }

        tex.Apply();

        lastErasePoint = point;
    }

    private void UpdateWidth()
    {
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Axis0)) {
            indicator.SetActive(true);

            var touchPoint = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            var newWidth = (int)Mathf.Lerp(2, 20, (touchPoint.x + 1.0f) / 2.0f);

            if (Width != newWidth)
            {
                Width = newWidth;
                device.TriggerHapticPulse(250);
            }

            indicator.transform.localScale = new Vector3(Width / 10.0f, Width / 10.0f, Width / 10.0f);
        }
        else
            indicator.SetActive(false);
    }

    private void UpdateState()
    {
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
            transform.Rotate(Vector3.up, 180.0f);
        
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
            transform.Rotate(Vector3.up, 180.0f);
    }

    private Vector2 CalculateDrawPoint(Texture t, Vector2 textureCoord)
    {
        Vector2 p = textureCoord;
        p.x *= t.width;
        p.y *= t.height;

        return p;
    }

    private void DrawPoint(Texture2D t, Vector2 p, int w, Color c)
    {
        if (Softness == 0.0f) {
            Color[] colours = new Color[w * w];
            for (int i = 0; i < w * w; ++i)
                colours[i] = c;

            t.SetPixels((int)p.x - w / 2, (int)p.y - w / 2, w, w, colours);
        } else {
            for (int y = (int)p.y - (w / 2); y < p.y + (w / 2); ++y) {
                for (int x = (int)p.x - (w / 2); x < p.x + (w / 2); ++x) {
                    if (Vector2.Distance(new Vector2(x, y), p) > (Width / 2) * 1 / Softness)
                        continue;

                    t.SetPixel(x, y, c);
                }
            }
        }
    }
}
