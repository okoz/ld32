﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Sprite Icon;

    private Rigidbody rigidBody;

    public void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (rigidBody.velocity.sqrMagnitude > 0.01f)
        {
            transform.LookAt(transform.position + rigidBody.velocity);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Animal animal = collision.gameObject.GetComponent<Animal>();
        if (animal != null)
        {
            ApplyEffect(animal);
        }
        else
            Destroy(gameObject);
    }

    protected virtual void ApplyEffect(Animal animal)
    {
        Destroy(gameObject);
    }
}
