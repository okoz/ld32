using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GameObject[] Projectiles;
    public float MaxForce;
    public float MinForce;
    public Slider FireStatus;

	void Start()
    {

	}
	
	void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            float fireForce = MinForce + FireStatus.value * (MaxForce - MinForce);
            SetSliderValue(0.0f);

            if (Projectiles.Length > 0)
            {
                GameObject projectile = GameObject.Instantiate<GameObject>(Projectiles[0]);


                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                projectile.transform.position = mouseRay.origin;

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(mouseRay.direction * fireForce, ForceMode.Impulse);
                }
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            SetSliderValue(0.0f);
        }

        if (Input.GetButton("Fire1"))
        {
            SetSliderValue(FireStatus.value + Time.deltaTime);
        }
	}

    private void SetSliderValue(float value)
    {
        FireStatus.value = Mathf.Clamp(value, 0.0f, 1.0f);
    }
}
