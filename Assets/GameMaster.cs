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

public class GameMaster : MonoBehaviour
{
    public List<GameObject> MustKill;
    public List<GameObject> CantKill;
    public GameObject PlayerGun;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public float DetonationStartDelay;
    public float DetonationDelay;

    public Vector2 SpawnExtents;
    public GameObject PinkSheep;
    public GameObject BlackSheep;
    public GameObject WhiteSheep;
    public GameObject NuggetSheep;
    public LevelInfo[] Levels;

    public void Start()
    {
        SetUpLevel(0);
    }

    private void SetUpLevel(int i)
    {
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        foreach (GameObject sheep in sheeps)
        {
            Destroy(sheep);
        }
        
        LevelInfo level = Levels[i];
        SpawnSheep(PinkSheep, level.NumPink);
        SpawnSheep(BlackSheep, level.NumBlack);
        SpawnSheep(WhiteSheep, level.NumWhite);
        SpawnSheep(NuggetSheep, level.NumNuggets);
    }

    private void SpawnSheep(GameObject sheep, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject newSheep = GameObject.Instantiate<GameObject>(sheep);
            newSheep.transform.position = RandomSpawnPoint();
        }
    }

    private Vector3 RandomSpawnPoint()
    {
        Vector3 position = transform.position;
        position.x += Random.RandomRange(-SpawnExtents.x, SpawnExtents.x);
        position.z += Random.RandomRange(-SpawnExtents.y, SpawnExtents.y);
        return position;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(SpawnExtents.x, 0.0f, SpawnExtents.y));
    }

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
