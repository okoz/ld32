using UnityEngine;
using System.Collections;

public class SlowBullet : Bullet
{
    public float Fraction;
    public float Duration;

    public void OnCollisionEnter(Collision collision)
    {
        Animal animal = collision.gameObject.GetComponent<Animal>();
        if(animal != null)
        {
            animal.Slow(Fraction);
        }

        Destroy(gameObject);
    }
}
