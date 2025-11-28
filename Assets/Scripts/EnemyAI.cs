using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search }
    public State state = State.Patrol;

    public NavMeshAgent agent;
    public Transform player;
    public float chaseDistance = 8f;
    public float loseSightTime = 3f;
    public List<Transform> patrolPoints;
    int currentPatrol = 0;
    float lastSeenTime = Mathf.Infinity;
    private float ghostSpeed = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Count > 0) agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        switch(state)
        {
            case State.Patrol:
                PatrolUpdate(dist);
                break;
            case State.Chase:
                ChaseUpdate(dist);
                break;
            case State.Search:
                SearchUpdate(dist);
                break;
        }
        float currentSpeed = ghostSpeed * GameManager.Instance.ghostSpeedMultiplier;

        // Transitions
        if (dist <= chaseDistance) { state = State.Chase; lastSeenTime = 0f; }
        else if (state == State.Chase) { lastSeenTime += Time.deltaTime; if (lastSeenTime >= loseSightTime) state = State.Search; }
    }

    void PatrolUpdate(float dist)
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolPoints.Count>0)
        {
            currentPatrol = (currentPatrol + 1) % patrolPoints.Count;
            agent.SetDestination(patrolPoints[currentPatrol].position);
        }
    }

    void ChaseUpdate(float dist)
    {
        agent.SetDestination(player.position);
        // optionally play chase audio, change speed
    }

    void SearchUpdate(float dist)
    {
        // go to last known point then resume patrol
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            state = State.Patrol;
            if (patrolPoints.Count>0) agent.SetDestination(patrolPoints[currentPatrol].position);
        }
    }

    
}
