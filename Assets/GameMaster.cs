using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameMaster : MonoBehaviour {
    public List<GameObject> MustKill;
    public List<GameObject> CantKill;
    public GameObject PlayerGun;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public float DetonationStartDelay;
    public float DetonationDelay;

    public void OnKill(GameObject go)
    {
        if (CantKill.Contains(go))
        {
            // You lose.
            PlayerGun.SetActive(false);
            StopSheep();
            LoseScreen.SetActive(true);
            StartCoroutine(Detonate());
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
                StartCoroutine(Detonate());
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

    private IEnumerator Detonate()
    {
        yield return new WaitForSeconds(DetonationStartDelay);

        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        foreach (GameObject sheep in sheeps)
        {
            Animal animal = sheep.GetComponent<Animal>();
            if (animal != null)
            {
                GameObject gibs = GameObject.Instantiate<GameObject>(animal.GibExplosion);
                gibs.transform.position = animal.transform.position;
                Destroy(animal.gameObject);
            }

            yield return new WaitForSeconds(DetonationDelay);
        }
    }
}
