using UnityEngine;

public class PermanentObject : MonoBehaviour
{
    public GameObject Prefab;
    public float DistanceLimit;

	void Update ()
    {
        Transform child;
        if ((child = transform.FindChild(Prefab.name)) == null)
            Spawn();
        else if (Vector3.Distance(child.position, transform.position) > DistanceLimit) {
            Destroy(child.gameObject);
            Spawn();
        }
	}

    private void Spawn()
    {
        var newChild = Instantiate(Prefab, transform.position, Quaternion.identity) as GameObject;
        newChild.name = Prefab.name;
        newChild.transform.parent = transform;
    }
}
