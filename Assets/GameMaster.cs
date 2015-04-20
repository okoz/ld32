using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class LevelInfo
{
    public int NumPink;
    public int NumBlack;
    public int NumWhite;
    public int NumNuggets;

    public int KillPink;
    public int KillBlack;
    public int KillWhite;
    public int KillNuggets;
}

public class GameMaster : MonoBehaviour {
    public List<GameObject> MustKill;
    public List<GameObject> CantKill;
    public GameObject PlayerGun;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public float DetonationStartDelay;
    public float DetonationDelay;

    public GameObject PinkSheep;
    public GameObject BlackSheep;
    public GameObject WhiteSheep;
    public GameObject NuggetSheep;
    public LevelInfo[] Levels;

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
