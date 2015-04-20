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
	}

    public void OnKill(GameObject go)
    {
        if (CantKill.Contains(go))
        {
            // You lose.
            PlayerGun.SetActive(false);
        }
        else
        {
            MustKill.Remove(go);

            if (MustKill.Count == 0)
            {
                // Victory!
                PlayerGun.SetActive(false);
            }
        }
    }
}
