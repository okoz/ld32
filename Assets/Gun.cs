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

    private LineRenderer lineRenderer;

	void Start()
    {
        if(FireStatus != null)
        {
            Transform child = FireStatus.transform.FindChild("Fill Area");
            FireStatusBackground = child.GetComponentInChildren<Image>();
        }

        lineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            if (Projectiles.Length > 0)
            {
                GameObject projectile = GameObject.Instantiate<GameObject>(Projectiles[0]);

                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                projectile.transform.position = mouseRay.origin;

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(mouseRay.direction * GetPower(), ForceMode.Impulse);
                }
            }

            SetSliderValue(0.0f);
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            SetSliderValue(0.0f);
        }

        if (Input.GetButton("Fire1"))
        {
            SetSliderValue(FireStatus.value + Time.deltaTime);
        }

        DisplayFireArc(GetPower());
	}

    private float GetPower()
    {
        return MinForce + FireStatus.value * (MaxForce - MinForce);
    }

    private void SetSliderValue(float value)
    {
        FireStatus.value = Mathf.Clamp(value, 0.0f, 1.0f);
        FireStatusBackground.color = ExtensionMethods.FromHSV(FireStatus.value * 100.0f, 1.0f, 1.0f);
    }

    private void DisplayFireArc(float power)
    {
        if (Projectiles.Length > 0)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float mass = Projectiles[0].GetComponent<Rigidbody>().mass;
            Vector3 v0 = mouseRay.direction * power / mass;

            lineRenderer.SetVertexCount(32);
            for(int i = 0; i < 32; ++i)
            {
                float t = 0.1f * i;
                Vector3 p = mouseRay.origin + v0 * t + 0.5f * Physics.gravity * t * t;
                lineRenderer.SetPosition(i, p);
            }
        }
    }
}
