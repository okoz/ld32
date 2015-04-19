using UnityEngine;
using System.Collections;

public class AngryBullet : Bullet {

    public void OnCollisionEnter(Collision collision)
    {
        Animal animal = collision.gameObject.GetComponent<Animal>();
        if (animal != null)
        {
            animal.Anger();
        }

        Destroy(gameObject);
    }
}
