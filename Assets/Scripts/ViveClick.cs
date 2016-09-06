using UnityEngine;
using System.Collections;
using System.IO;

public class ViveClick : MonoBehaviour
{
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
        if (c.gameObject.GetComponent<ViveClickable>() == null)
            return;

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (c.gameObject.name == "Box003")
            {
                var board = GameObject.Find("DrawSurface");
                var w = board.GetComponent<Whiteboard>();

                w.ClearCanvas();

                device.TriggerHapticPulse(1000);
            }

            else if (c.gameObject.name == "Box004")
            {
                var board = GameObject.Find("DrawSurface");
                var r = board.GetComponent<Renderer>();

                var bytes = (r.material.mainTexture as Texture2D).EncodeToPNG();
                var filename = string.Format("{0}.png", System.DateTime.Now.ToString("yyyyMMddhhMMss"));
                var file = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/" + filename, FileMode.Create);
                var binary = new BinaryWriter(file);
                binary.Write(bytes);
                file.Close();

                device.TriggerHapticPulse(1000);
            }
        }
    }
}