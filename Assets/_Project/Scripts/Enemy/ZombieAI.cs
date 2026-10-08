using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Attack }

    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float walkSpeed = 0.2f;
    [SerializeField] private float waitTime = 2f;

    [Header("Detección")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float loseRange = 18f;
    [SerializeField] private float viewAngle = 110f;
    [SerializeField] private float eyeHeight = 1.6f;

    [Header("Persecución y ataque")]
    [SerializeField] private float runSpeed = 0.5f;
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int attackDamage = 10;

    [Header("Animator (ajustar a tus nombres)")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private float animReferenceSpeed = 0.2f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private PlayerHealth playerHealth;

    private State state = State.Patrol;
    private int waypointIndex;
    private float waitTimer;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponentInParent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("ZombieAI: no se encontró ningún objeto con el tag Player");
        }

        GoToWaypoint();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase:  UpdateChase();  break;
            case State.Attack: UpdateAttack(); break;
        }

        UpdateAnimator();
    }

    private void UpdatePatrol()
    {
        agent.speed = walkSpeed;
        agent.stoppingDistance = 0.3f;

        if (CanSeePlayer())
        {
            state = State.Chase;
            return;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.3f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                GoToWaypoint();
            }
        }
    }

    private void UpdateChase()
    {
        if (PlayerIsGone())
        {
            ReturnToPatrol();
            return;
        }

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.stoppingDistance = attackRange * 0.75f;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
            state = State.Attack;
        else if (distance > loseRange)
            ReturnToPatrol();
    }

    private void UpdateAttack()
    {
        if (PlayerIsGone())
        {
            ReturnToPatrol();
            return;
        }

        agent.isStopped = true;

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 8f * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange + 0.5f)
        {
            agent.isStopped = false;
            state = State.Chase;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            SetTrigger(attackTrigger);

            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }
    }

    private bool PlayerIsGone()
    {
        return player == null || (playerHealth != null && playerHealth.IsDead);
    }

    private void ReturnToPatrol()
    {
        state = State.Patrol;
        GoToWaypoint();
    }

    private bool CanSeePlayer()
    {
        if (PlayerIsGone()) return false;

        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 direction = target - eye;

        if (direction.magnitude > detectionRange) return false;

        Vector3 flat = new Vector3(direction.x, 0f, direction.z);
        if (Vector3.Angle(transform.forward, flat) > viewAngle * 0.5f) return false;

        if (Physics.Raycast(eye, direction.normalized, out RaycastHit hit, detectionRange, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform == player || hit.transform.IsChildOf(player);

        return false;
    }

    private void GoToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        agent.isStopped = false;
        agent.SetDestination(waypoints[waypointIndex].position);
    }

   private void UpdateAnimator()
{
    if (animator == null) return;

    float velocity = agent.velocity.magnitude;

    // Ajusta la velocidad de la animación al movimiento real para que los pies no patinen
    animator.speed = velocity > 0.05f ? Mathf.Clamp(velocity / animReferenceSpeed, 0.4f, 2f) : 1f;

    foreach (var p in animator.parameters)
    {
        if (p.name == speedParam && p.type == AnimatorControllerParameterType.Float)
            animator.SetFloat(speedParam, velocity);
    }
}

    private void SetTrigger(string triggerName)
    {
        if (animator == null) return;

        foreach (var p in animator.parameters)
        {
            if (p.name == triggerName && p.type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(triggerName);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
