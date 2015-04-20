using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
    public List<GameObject> MustKill;
    public List<GameObject> CantKill;
    public GameObject PlayerGun;

	void Start ()
    {
	}
	
	void Update ()
    {
	    if (MustKill.Count == 0)
        {
            // Victory!
        }
	}

    public void OnKill(GameObject go)
    {

    }
}
