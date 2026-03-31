using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Search, Return }
    public State currentState;

    [Header("References")]
    private Transform player;
    [SerializeField] private LayerMask wallLayer;

    [Header("Vision")]
    [SerializeField] private float visionDistance = 8f;

    [Header("Chase Range")]
    [SerializeField] private float chaseRadius = 10f;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float patrolRadius = 2f;

    [Header("Search")]
    [SerializeField] private float searchTime = 2f;

    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private Vector2 lastKnownPlayerPos;

    private float searchTimer;
    private float findTimer;

    private void Start()
    {
        startPosition = transform.position;
        PickNewPatrolPoint();
        currentState = State.Patrol;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Search:
                Search();
                break;

            case State.Return:
                ReturnHome();
                break;
        }

        findTimer -= Time.deltaTime;

        if (player == null && findTimer <= 0f)
        {
            FindPlayer();
            findTimer = 1f;
        }

        if (player == null)
        {
            return;
        }
    }

    //Just like the previous patrol small Bee enemy I had, but MUCH simplier omg.
    private void Patrol()
    {
        if (player == null)
        {
            return;
        }

        MoveTowards(patrolTarget);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            PickNewPatrolPoint();
        }

        if (CanSeePlayer())
        {
            currentState = State.Chase;
        }
    }

    //Chase D Playhaaa
    private void Chase()
    {
        if (player == null)
        {
            return;
        }

        if (!IsWithinChaseRange())
        {
            currentState = State.Return;
            return;
        }

        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.position;
            MoveTowards(player.position);
        }
        else
        {
            currentState = State.Search;
            searchTimer = searchTime;
        }
    }

    //Searching for Players, YOU CAN GUESS WHAT THAT MEANS
    private void Search()
    {
        if (player == null)
        {
            return;
        }

        MoveTowards(lastKnownPlayerPos);

        searchTimer -= Time.deltaTime;

        if (CanSeePlayer())
        {
            currentState = State.Chase;
            return;
        }

        if (searchTimer <= 0f)
        {
            currentState = State.Return;
        }
    }

    //Return to Spawn if player is not here :(
    private void ReturnHome()
    {
        MoveTowards(startPosition);

        if (Vector2.Distance(transform.position, startPosition) < 0.2f)
        {
            currentState = State.Patrol;
            PickNewPatrolPoint();
        }

        if (CanSeePlayer() && IsWithinChaseRange())
        {
            currentState = State.Chase;
        }
    }

    //Main Logic
    private bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector2 dir = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, visionDistance, wallLayer | LayerMask.GetMask("Player"));

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    private bool IsWithinChaseRange()
    {
        if (player == null)
        {
            return false;
        }

        return Vector2.Distance(startPosition, player.position) <= chaseRadius;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        //flipping
        if (direction.x > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //SHOULD go around wall or collision
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);

        if (hit.collider != null)
        {
            direction += Vector2.up * 0.5f;
        }

        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
    }

    private void PickNewPatrolPoint()
    {
        patrolTarget = startPosition + Random.insideUnitCircle * patrolRadius;
    }
    private void FindPlayer()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    //Gizmsos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)startPosition : transform.position, chaseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lastKnownPlayerPos, 0.2f);
    }
}