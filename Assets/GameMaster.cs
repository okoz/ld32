using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class LevelInfo
{
    public int NumPink;
    public int NumBlack;
    public int NumWhite;
    public int NumNuggets;

    public bool KillPink;
    public bool KillBlack;
    public bool KillWhite;
    public bool KillNuggets;

    public string Description;
}

public class GameMaster : MonoBehaviour
{
    public GameObject PlayerGun;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public GameObject VictoryScreen;
    public Text DescriptionText;
    public Text DescriptionShadowText;

    public float DetonationStartDelay;
    public float DetonationDelay;

    public Vector2 SpawnExtents;
    public GameObject PinkSheep;
    public GameObject BlackSheep;
    public GameObject WhiteSheep;
    public GameObject NuggetSheep;
    public LevelInfo[] Levels;

    private int currentLevel;
    private LevelInfo levelInfo;

    private List<GameObject> MustKill = new List<GameObject>();
    private List<GameObject> CantKill = new List<GameObject>();

    public void Start()
    {
        SetUpLevel(currentLevel);
    }

    private void SetUpLevel(int i)
    {
        MustKill.Clear();
        CantKill.Clear();

        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        foreach (GameObject sheep in sheeps)
        {
            Destroy(sheep);
        }
        
        if (i >= Levels.Length)
        {
            PlayerGun.SetActive(false);
            VictoryScreen.SetActive(true);
            return;
        }

        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
        PlayerGun.SetActive(true);

        levelInfo = Levels[i];
        SpawnSheep(PinkSheep, levelInfo.NumPink, levelInfo.KillPink ? MustKill : CantKill);
        SpawnSheep(BlackSheep, levelInfo.NumBlack, levelInfo.KillBlack ? MustKill : CantKill);
        SpawnSheep(WhiteSheep, levelInfo.NumWhite, levelInfo.KillWhite ? MustKill : CantKill);
        SpawnSheep(NuggetSheep, levelInfo.NumNuggets, levelInfo.KillNuggets ? MustKill : CantKill);

        DescriptionText.text = levelInfo.Description;
        DescriptionShadowText.text = levelInfo.Description;
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            NextLevel();
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        SetUpLevel(currentLevel);
    }

    public void RestartLevel()
    {
        SetUpLevel(currentLevel);
    }

    private void SpawnSheep(GameObject sheep, int count, IList<GameObject> list)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject newSheep = GameObject.Instantiate<GameObject>(sheep);
            newSheep.transform.position = RandomSpawnPoint();
            list.Add(newSheep);
        }
    }

    private Vector3 RandomSpawnPoint()
    {
        Vector3 position = transform.position;
        position.x += 0.5f * Random.RandomRange(-SpawnExtents.x, SpawnExtents.x);
        position.z += 0.5f * Random.RandomRange(-SpawnExtents.y, SpawnExtents.y);
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
