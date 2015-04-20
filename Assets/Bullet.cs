using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Sprite Icon;
    public float Speed;
    public float HitDistance;

    private Vector3 origin;
    private Vector3 target;
    private float t;

    public void Start()
    {
    }

    public void FixedUpdate()
    {
        transform.position = GetPosition(t * Speed);
        transform.LookAt(transform.position + GetDirection(t * Speed));
        t += Time.fixedDeltaTime;
    }

    public void Update()
    {
        if (transform.position.y < 0.0f)
        {
            Destroy(gameObject);
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo))
        {
            if (hitInfo.distance <= HitDistance)
            {
                Animal animal = hitInfo.collider.gameObject.GetComponent<Animal>();
                if (animal != null)
                {
                    ApplyEffect(animal);
                }
            }
        }
    }

    public void Fire(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
    }

    protected Vector3 GetPosition(float t)
    {
        float dx = (origin.ProjectY(0) - target.ProjectY(0)).magnitude;
        float dy = -origin.y;
        float v0 = dx * Mathf.Sqrt(-Physics.gravity.magnitude / (2.0f * dy));

        return origin + v0 * (target - origin).ProjectY(0).normalized * t + 0.5f * Physics.gravity * t * t;
    }

    protected Vector3 GetDirection(float t)
    {
        float dx = (origin.ProjectY(0) - target.ProjectY(0)).magnitude;
        float dy = -origin.y;
        float v0 = dx * Mathf.Sqrt(-Physics.gravity.magnitude / (2.0f * dy));

        return (v0 * (target - origin).ProjectY(0).normalized + Physics.gravity * t).normalized;
    }

    protected virtual void ApplyEffect(Animal animal)
    {
        Destroy(gameObject);
    }
}
