using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
    public List<GameObject> MustKill;
    public List<GameObject> CantKill;
    public GameObject PlayerGun;
    public GameObject LoseScreen;
    public GameObject WinScreen;

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
            StopSheep();
            LoseScreen.SetActive(true);
        }
        else
        {
            MustKill.Remove(go);

            if (MustKill.Count == 0)
            {
                // Victory!
                PlayerGun.SetActive(false);
                StopSheep();
                WinScreen.SetActive(true);
            }
        }
    }

    private void StopSheep()
    {
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        foreach (GameObject sheep in sheeps)
        {
            Animal animal = sheep.GetComponent<Animal>();
            if (animal != null)
            {
                animal.Stop();
            }
        }
    }
}
