using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFactoryScript : MonoBehaviour
{
    public static AnimationFactoryScript Instance { set; get; }

    public GameObject LeafFlutterBase;
    public int MaxLeafFlutters = 20;
    private List<LeafFlutterScript> pool = new List<LeafFlutterScript>();
    private List<LeafFlutterScript> active = new List<LeafFlutterScript>();

    // Use this for initialization
    void Start ()
    {
        Instance = this;
        for (int i = 0; i < MaxLeafFlutters; i++)
        {
            LeafFlutterScript lfs  = Instantiate(LeafFlutterBase).GetComponent<LeafFlutterScript>();
            lfs.gameObject.SetActive(false);
            pool.Add(lfs);
            
        }
	}

    public void SpawnLeafFlutter(Vector3 pos)
    {
        if (pool.Count > 1)
        {
            LeafFlutterScript lf = pool[0];
            lf.PlayAnimAtLocation(pos);
            pool.Remove(lf);
            active.Add(lf);
        }
    }

    public void RemoveLeafFlutter(LeafFlutterScript lfs)
    {
        Debug.Log("Removed");
        if (active.Contains(lfs))
        {
            
            active.Remove(lfs);
            pool.Add(lfs);
        }
        else { Destroy(lfs.gameObject); }
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
