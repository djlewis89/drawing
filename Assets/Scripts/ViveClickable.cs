using UnityEngine;

public class ViveClickable : MonoBehaviour
{
    // Inspector Variables
    public Color Highlight;
    public Color Success;

    // Steam VR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    private Color original;
    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();

        if (r != null)
            original = r.material.color;
    }

    void OnTriggerEnter(Collider c)
    {
        r.material.color = Highlight;

        trackedObj = c.gameObject.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    void OnTriggerStay(Collider c)
    {
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            r.material.color = Success;
        }
    }

    void OnTriggerExit(Collider c)
    {
        r.material.color = original;

        trackedObj = null;
        device = null;
    }
}
