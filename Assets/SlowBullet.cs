using UnityEngine;
using System.Collections;

public class SlowBullet : Bullet
{
    public float Fraction;
    public float Duration;

    protected override void ApplyEffect(Animal animal)
    {
        if (animal.HypoRoot.childCount == 1 && animal.Demeanor != Demeanor.Slow)
        {
            Destroy(animal.HypoRoot.GetChild(0).gameObject);
        }

        if (animal.Demeanor == Demeanor.Normal)
        {
            transform.SetParent(animal.HypoRoot, true);
            Destroy(this);
            Destroy(GetComponent<Rigidbody>());
        }
        else
            Destroy(gameObject);

        animal.Slow(Fraction);
    }
}
