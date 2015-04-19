using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

internal class StateMachine
{
    private IDictionary<string, Action<StateMachine>> states = new Dictionary<string, Action<StateMachine>>();
    private Action<StateMachine> activeState;

    public void AddState(string name, Action<StateMachine> state)
    {
        states.Add(name, state);
    }

    public void SetState(string name)
    {
        activeState = null;
        if (states.ContainsKey(name))
            activeState = states[name];
    }

    public void Update()
    {
        if (activeState != null)
            activeState(this);
    }
}

public class Animal : MonoBehaviour
{
    public float Speed;

    private CharacterController characterController;
    private LineRenderer lineRenderer;

    private StateMachine stateMachine = new StateMachine();

	void Start()
    {
        path = new NavMeshPath();

        stateMachine.AddState("Idle", Idle);
        stateMachine.AddState("Walking", Walking);
        stateMachine.AddState("Grazing", Grazing);
        stateMachine.SetState("Idle");

        characterController = GetComponent<CharacterController>();
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update()
    {
        stateMachine.Update();
    }

    #region Animal interface.
    
    bool RandomPointOnNavmesh(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 32; ++i)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * range;
            Vector3 randomSource = center + new Vector3(randomCircle.x, 0.0f, randomCircle.y);

            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomSource, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                result.y = center.y;
                return true;
            }
        }

        result = center;
        return false;
    }

    private NavMeshPath path;
    private int nextPathIndex;

    public void FindNewGrazingSpot()
    {
        Vector3 destination;
        if(RandomPointOnNavmesh(transform.position, 50.0f, out destination))
        {
            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            nextPathIndex = 0;

            if(lineRenderer != null)
            {
                lineRenderer.SetVertexCount(path.corners.Length);
                for(int i = 0; i < path.corners.Length; ++i)
                {
                    lineRenderer.SetPosition(i, path.corners[i]);
                }
            }
        }
    }

    private bool IsCloseEnough(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < characterController.radius * characterController.radius;
    }

    public Vector3 NextWaypoint()
    {
        if (nextPathIndex >= path.corners.Length)
            return transform.position;

        if(IsCloseEnough(path.corners[nextPathIndex], transform.position))
        {
            nextPathIndex++;
        }

        if (nextPathIndex < path.corners.Length)
            return path.corners[nextPathIndex];

        return transform.position;
    }

    public bool AtDestination()
    {
        if(path.corners.Length == 0 || path.status == NavMeshPathStatus.PathInvalid)
        {
            return true;
        }

        if(IsCloseEnough(path.corners[path.corners.Length - 1], transform.position))
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Effects.

    private bool isSlowed;

    public void Slow(float fraction, float duration)
    {
        StartCoroutine(SlowEffect(fraction, duration));
    }

    private IEnumerator SlowEffect(float fraction, float duration)
    {
        if (!isSlowed)
        {
            float oldSpeed = Speed;

            isSlowed = true;
            Speed *= fraction;

            yield return new WaitForSeconds(duration);

            isSlowed = false;
            Speed = oldSpeed;
        }
    }

    #endregion

    #region Machine states.
    
    private void Idle(StateMachine machine)
    {
        FindNewGrazingSpot();
        machine.SetState("Walking");
    }

    private void Walking(StateMachine machine)
    {
        if (!AtDestination())
        {
            Vector3 nextWaypoint = NextWaypoint();
            characterController.Move((nextWaypoint - transform.position).normalized * Speed * Time.deltaTime);

            // So the animals don't look down.
            nextWaypoint.y = transform.position.y;
            transform.LookAt(nextWaypoint);
        }
        else
            machine.SetState("Idle");
    }

    private void Grazing(StateMachine machine)
    {

    }

    #endregion
}
