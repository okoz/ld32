using UnityEngine;
using System.Collections;

public class SlowBullet : MonoBehaviour
{
    public float Fraction;
    public float Duration;

    public void OnCollisionEnter(Collision collision)
    {
        Animal animal = collision.gameObject.GetComponent<Animal>();
        if(animal != null)
        {
            animal.Slow(Fraction, Duration);
        }

        Destroy(gameObject);
    }
}
