using UnityEngine;
using System.Collections;

public class GibExplosion : MonoBehaviour
{
    public GameObject[] Gibs;
    public int MinGibs;
    public int MaxGibs;

    public float SmallestGib;
    public float LargestGib;

    public float MinForce;
    public float MaxForce;

    public float TimeToDestroy;

	void Start()
    {
        int numGibs = Random.Range(MinGibs, MaxGibs + 1);

        for (int i = 0; i < numGibs; ++i)
        {
            GameObject gib = GameObject.Instantiate<GameObject>(Gibs[Random.Range(0, Gibs.Length)]);
            float scale = Random.Range(SmallestGib, LargestGib);
            gib.transform.localScale = new Vector3(scale, scale, scale);
            gib.transform.position = transform.position + Random.insideUnitSphere.ProjectY(0.0f) * 0.5f;

            Rigidbody rb = gib.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (Random.insideUnitSphere + 5.0f * Vector3.up).normalized;
                rb.AddForce(direction * Random.Range(MinForce, MaxForce), ForceMode.Impulse);
            }
        }

        StartCoroutine(DestroySelf());
	}	

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        Destroy(gameObject);
    }
}
