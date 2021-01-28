using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform targetLookAt;
    public Transform player;
    public GameObject[] regions;

    Vector3 offset = new Vector3(0,0,-10);
    Camera cam;

    void Start()
    {
        if (targetLookAt.transform.parent == null) { targetLookAt.transform.parent = player.transform; }
        regions = GameObject.FindGameObjectsWithTag("Region");
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetLookAt.transform.position + offset;
        //Transform closestRegion = GetClosestRegion(regions);
        //cam.backgroundColor = Color.Lerp(cam.backgroundColor, closestRegion.GetComponent<RegionScript>().regionColour, 0.02f);
    }

    Transform GetClosestRegion(GameObject[] regions)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in regions)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }
}
