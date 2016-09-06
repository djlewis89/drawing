using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ObjectSpawn : MonoBehaviour 
{
    public GameObject spawnTarget;

    public float spawnRate = 0.25f;
    public float force = 25;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;

    private float lastSpawnTime = 0.0f;

    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate () 
	{
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && Time.time - lastSpawnTime > spawnRate)
			Spawn ();
	}

	void Spawn ()
	{
        lastSpawnTime = Time.time;

        GameObject o = Instantiate(spawnTarget, device.transform.pos, Quaternion.identity) as GameObject;

        float s = Random.Range(0.01f, 0.1f);

        o.transform.localScale = new Vector3(s, s, s);

        Rigidbody rb = o.GetComponent<Rigidbody>();
        rb.AddForce(trackedObj.transform.forward * force);

        BoxCollider b = o.GetComponent<BoxCollider>();
        b.size = new Vector3(1, 1, 1);
	}
}
