using UnityEngine;
using System.Collections;

public class AngryBullet : Bullet {

    protected override void ApplyEffect(Animal animal)
    {
        if (animal.HypoRoot.childCount == 1 && animal.Demeanor != Demeanor.Angry)
        {
            Destroy(animal.HypoRoot.GetChild(0).gameObject);
        }

        if (animal.Demeanor == Demeanor.Normal)
        {
            transform.SetParent(animal.HypoRoot, true);
            Destroy(this);
        }
        else
            Destroy(gameObject);

        animal.Anger();
    }
}
