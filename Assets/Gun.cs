using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GameObject[] Projectiles;
    public float MaxForce;
    public float MinForce;
    public Slider FireStatus;
    public Image FireStatusBackground;
    public GameObject[] GunPoints;

    private LineRenderer lineRenderer;
    private Plane groundPlane = new Plane(Vector3.up, 0.0f);

	void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float t;
        groundPlane.Raycast(mouseRay, out t);
        Vector3 target = mouseRay.GetPoint(t);
        Vector3 origin = GetNearestGunPoint(target).transform.position;

        float dx = (origin.ProjectY(0) - target.ProjectY(0)).magnitude;
        float dy = -origin.y;
        float v0 = dx * Mathf.Sqrt(-Physics.gravity.magnitude / (2.0f * dy));

        if (Input.GetButton("Fire1") || Input.GetButton("Fire2"))
        {
            DisplayFireArc(origin, target);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            GameObject projectile = GameObject.Instantiate<GameObject>(Projectiles[0]);

            projectile.transform.position = origin;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((target - origin).ProjectY(0).normalized * v0, ForceMode.Impulse);
            }

            ClearFireArc();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            GameObject projectile = GameObject.Instantiate<GameObject>(Projectiles[1]);

            projectile.transform.position = origin;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((target - origin).ProjectY(0).normalized * v0, ForceMode.Impulse);
            }

            ClearFireArc();
        }
	}

    private void SetSliderValue(float value)
    {
        FireStatus.value = Mathf.Clamp(value, 0.0f, 1.0f);
        FireStatusBackground.color = ExtensionMethods.FromHSV(FireStatus.value * 100.0f, 1.0f, 1.0f);
    }

    private void DisplayFireArc(Vector3 origin, Vector3 target)
    {
        float dx = (origin.ProjectY(0) - target.ProjectY(0)).magnitude;
        float dy = -origin.y;
        float v0 = dx * Mathf.Sqrt(-Physics.gravity.magnitude / (2.0f * dy));

        lineRenderer.SetVertexCount(32);
        for(int i = 0; i < 32; ++i)
        {
            float t = 0.1f * i;
            Vector3 p = origin + v0 * (target - origin).ProjectY(0).normalized * t + 0.5f * Physics.gravity * t * t;
            lineRenderer.SetPosition(i, p);
        }
    }

    private void ClearFireArc()
    {
        lineRenderer.SetVertexCount(0);
    }

    private GameObject GetNearestGunPoint(Vector3 target)
    {
        float distance = float.MaxValue;
        GameObject nearest = null;
        foreach (GameObject gunPoint in GunPoints)
        {
            float d = (gunPoint.transform.position - target).sqrMagnitude;
            if (d < distance)
            {
                nearest = gunPoint;
                distance = d;
            }
        }
        return nearest;
    }
}
