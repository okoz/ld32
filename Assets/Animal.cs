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

public enum Demeanor
{
    Slow,
    Normal,
    Angry
}

public class Animal : MonoBehaviour
{
    public float Speed;
    public float BiteRange;
    public Transform HypoRoot;
    public float WaypointRange;
    public GameObject GibExplosion;

    private CharacterController characterController;
    private LineRenderer lineRenderer;

    private StateMachine stateMachine = new StateMachine();
    private Demeanor demeanor = Demeanor.Normal;

    public Demeanor Demeanor
    {
        get { return demeanor; }
    }

	void Start()
    {
        path = new NavMeshPath();

        stateMachine.AddState("Idle", Idle);
        stateMachine.AddState("Walking", Walking);
        stateMachine.AddState("AngryIdle", AngryIdle);
        stateMachine.AddState("Angry", Angry);
        stateMachine.AddState("Stopped", Stopped);
        stateMachine.SetState("Idle");

        characterController = GetComponent<CharacterController>();
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update()
    {
        stateMachine.Update();

        if (moveDirection != Vector3.zero)
        {
            Quaternion q = Quaternion.LookRotation(moveDirection, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, q, 0.1f);
            transform.rotation = newRotation;
        }
    }

    #region Animal interface.
    
    bool RandomPointOnNavmesh(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 32; ++i)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * range;
            Vector3 randomSource = center + new Vector3(randomCircle.x, 0.0f, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomSource, out hit, 1.0f, 1))
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

    private void FindNewGrazingSpot()
    {
        Vector3 destination;
        if(RandomPointOnNavmesh(transform.position.ProjectY(0.0f), WaypointRange, out destination))
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

    private GameObject FindClosestSheep()
    {
        // Damn plurals.
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        GameObject closest = null;

        float distance = float.MaxValue;
        foreach (GameObject sheep in sheeps)
        {
            if (sheep == gameObject)
                continue;

            float d = (sheep.transform.position - transform.position).sqrMagnitude;
            if (d < distance)
            {
                closest = sheep;
                distance = d;
            }
        }

        return closest;
    }

    private bool IsCloseEnough(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a.ProjectY(0.0f) - b.ProjectY(0.0f)) < characterController.radius * characterController.radius;
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

    private float slowFraction;

    public void Slow(float fraction)
    {
        // Force a new path if the sheep was angry before.
        if (demeanor == Demeanor.Angry)
        {
            stateMachine.SetState("Idle");
        }

        demeanor = demeanor.Decrement();
        slowFraction = fraction;
    }

    public void Anger()
    {
        demeanor = demeanor.Increment();

        if (demeanor == Demeanor.Angry)
        {
            stateMachine.SetState("AngryIdle");
        }
    }

    public void Stop()
    {
        stateMachine.SetState("Stopped");
    }

    public void WakeUp()
    {
        stateMachine.SetState("Idle");
    }

    #endregion

    #region Machine states.
    
    private void Idle(StateMachine machine)
    {
        FindNewGrazingSpot();
        machine.SetState("Walking");
    }

    Vector3 lastPosition;
    Vector3 moveDirection = Vector3.zero;

    private void Walking(StateMachine machine)
    {
        if (!AtDestination())
        {
            Vector3 nextWaypoint = NextWaypoint();
            float effectiveSpeed = Speed;
            
            if (demeanor == Demeanor.Slow)
                effectiveSpeed *= slowFraction;

            moveDirection = (nextWaypoint - transform.position).ProjectY(0.0f).normalized;            
            characterController.SimpleMove(moveDirection * effectiveSpeed);
        }

        // Should handle both stuck and end of movement.
        if (lastPosition == transform.position)
        {
            machine.SetState("Idle");
        }

        lastPosition = transform.position;
    }

    private void AngryIdle(StateMachine machine)
    {
        path.ClearCorners();
        if (lineRenderer != null)
        {
            lineRenderer.SetVertexCount(0);
        }

        machine.SetState("Angry");
    }

    private void Angry(StateMachine machine)
    {
        GameObject target = FindClosestSheep();
        if (target != null)
        {
            Vector3 displacement = target.transform.position - transform.position;
            moveDirection = displacement.ProjectY(0.0f).normalized;
            characterController.SimpleMove(moveDirection * Speed);

            if (displacement.sqrMagnitude <= BiteRange * BiteRange)
            {
                GameObject gameMaster = GameObject.FindGameObjectWithTag("Game Master");
                if (gameMaster != null)
                {
                    GameMaster gm = gameMaster.GetComponent<GameMaster>();
                    if (gm != null)
                    {
                        gm.OnKill(target);
                    }
                }

                GameObject gibs = GameObject.Instantiate<GameObject>(GibExplosion);
                gibs.transform.position = target.transform.position;

                Destroy(target);
            }
        }
    }

    private void Stopped(StateMachine machine)
    {
        path.ClearCorners();
        if (lineRenderer != null)
        {
            lineRenderer.SetVertexCount(0);
        }
    }

    #endregion
}
