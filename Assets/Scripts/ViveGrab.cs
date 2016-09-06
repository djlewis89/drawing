using UnityEngine;
using System.Collections;

public class ViveGrab : MonoBehaviour
{
    // Inspector Variables
    public bool Clone = false;
    public bool Snap = false;
    public Vector3 SnapToPosition;
    public Vector3 SnapToAngle;

    // State
    private bool grabbed = false;

    // Steam VR
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.GetComponent<ViveGrabbable>() == null)
            return;

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && !grabbed) {
            grabbed = true;
            c.attachedRigidbody.isKinematic = true;
            c.gameObject.transform.SetParent(gameObject.transform);

            if (Snap) {
                c.gameObject.transform.localPosition = SnapToPosition;
                c.gameObject.transform.localRotation = Quaternion.Euler(SnapToAngle);
            }

            if (Clone) {
                var clone = Instantiate(c.gameObject) as GameObject;
                clone.transform.SetParent(null);
                clone.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) {
            grabbed = false;
            c.attachedRigidbody.isKinematic = false;
            c.gameObject.transform.SetParent(null);

            Throw(c.attachedRigidbody);
        }
    }

    private void Throw(Rigidbody rigidbody)
    {
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

        if (origin != null) {
            rigidbody.velocity = origin.TransformVector(device.velocity);
            rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
        } else {
            rigidbody.velocity = device.velocity;
            rigidbody.angularVelocity = device.angularVelocity;
        }
    }
}
