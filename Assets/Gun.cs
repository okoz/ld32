﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GameObject[] Projectiles;
    public float MaxForce;
    public float MinForce;
    public Slider FireStatus;
    public Image FireStatusBackground;
    public Image ActiveWeaponIcon;

    private LineRenderer lineRenderer;
    private GameObject activeProjectile;

	void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetProjectile(0);
	}
	
	void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        AimAt(mouseRay.direction);

        if (Input.GetButtonDown("Weapon 1"))
        {
            SetProjectile(0);
        }
        else if(Input.GetButtonDown("Weapon 2"))
        {
            SetProjectile(1);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (activeProjectile != null)
            {
                GameObject projectile = GameObject.Instantiate<GameObject>(activeProjectile);

                projectile.transform.position = transform.position;

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.forward * GetPower(), ForceMode.Impulse);
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
            float mass = Projectiles[0].GetComponent<Rigidbody>().mass;
            Vector3 v0 = transform.forward * power / mass;

            lineRenderer.SetVertexCount(32);
            for(int i = 0; i < 32; ++i)
            {
                float t = 0.1f * i;
                Vector3 p = transform.position + v0 * t + 0.5f * Physics.gravity * t * t;
                lineRenderer.SetPosition(i, p);
            }
        }
    }

    private void AimAt(Vector3 direction)
    {
        transform.LookAt(transform.position + direction, Vector3.up);
    }

    private void SetProjectile(int i)
    {
        if (i < Projectiles.Length)
        {
            activeProjectile = Projectiles[i];

            Bullet bullet = activeProjectile.GetComponent<Bullet>();
            ActiveWeaponIcon.sprite = bullet.Icon;
            
        }
    }
}
