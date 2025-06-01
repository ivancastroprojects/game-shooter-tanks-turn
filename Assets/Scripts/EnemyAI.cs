
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public AudioClip inIdle;
    public AudioClip inMovement;
    public ParticleSystem fireParticle;

    //Checking correct new location
    public Vector3 actualPos;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 5;

    //Attacking
    public float timeBetweenAttacks;
    public GameObject m_Shell;
    public Transform m_FireTransform;
    public Transform tower;

    //IA Mode
    public int easyMode = 1;
    public int intermediateMode = 2;
    public int hardMode = 3;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Variables to limit the distance of movement
    float radiusLimit; //radius of limit zone
    public Vector3 centerPosition; //center of limiter circle
    float distance; //tank <---> center of circle
    private GameObject containerCircle;
    private bool controlAudio = false;

    public float turnSpeed = 25;
    private void Awake()
    {
        player = GameObject.Find("TankPlayer1").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        Destroy(containerCircle);

        //Radius of limit zone circle
        radiusLimit = 50;
        //The center of circle will be the actual position at beginning of turn
        centerPosition = transform.localPosition;
        //We create a visualizable circle to see where is the limit of the zone
        containerCircle = new GameObject();
        containerCircle.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);
        DrawCircle(containerCircle, radiusLimit, 0.5f);
    }
    private void Update()
    { 
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //Range of movement of IA
        distance = Vector3.Distance(transform.position, centerPosition); //distance from tank to center of circle

        //We are on bounds of zone, so stop the tank
        if (distance >= 46 || (playerInAttackRange & playerInSightRange) || Vector3.Distance(player.position, transform.position) <= 5)
        {
            Debug.Log("entramos");
            agent.SetDestination(transform.position);
            AttackPlayer();
        }
        //If the distance is less than the radius, we can move free into
        else if (playerInSightRange)
            ChasePlayer();
        else Patroling();


            if (!controlAudio)
            {
                controlAudio = true;
                gameObject.GetComponent<AudioSource>().clip = inMovement;
                gameObject.GetComponent<AudioSource>().Play();
            }
    }

    //If IA is far of sight of enemy, he patrol to aleatory places
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        StartCoroutine(WaitingASeconds());
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    //If IA can see the player, he goes for him
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    
    //If IA can attack
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        int difficulty = PlayerPrefs.GetInt("difficulty");

        Debug.Log("Atacamos");
        agent.SetDestination(transform.position);
        tower.LookAt(player.position);
        Vector3 direction = player.gameObject.transform.GetChild(4).position - m_FireTransform.position;

        Rigidbody rb = Instantiate(m_Shell, m_FireTransform.position, tower.rotation).GetComponent<Rigidbody>();

        //IA mode easy, shoot with not much precission
        if (difficulty == 1)
        {
            direction += new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0);

            rb.AddForce(direction * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        }
        //IA intermediate, shoot normal and normal damage
        else if (difficulty == 2)
        {
            direction += new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0);

            rb.AddForce(direction * 34f, ForceMode.Impulse);
            rb.AddForce(transform.up * 9f, ForceMode.Impulse);
        }
        //IA difficult, shoot with high precission and high damage
        else if (difficulty == 3)
        {
            direction += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);

            rb.AddForce(direction * 36f, ForceMode.Impulse);
            rb.AddForce(transform.up * 10f, ForceMode.Impulse);
        }

        fireParticle.Play();
        tower.rotation = gameObject.transform.rotation;

        StartCoroutine(WaitingTheTurn());
        Destroy(rb, 2.5f);
    }

    private void AttackRandomPosition()
    {
        Debug.Log("Atacamos random");

        Rigidbody rb = Instantiate(m_Shell, m_FireTransform.position, tower.rotation).GetComponent<Rigidbody>();
        rb.AddForce(transform.right * 32f, ForceMode.Impulse);
        rb.AddForce(transform.up * 8f, ForceMode.Impulse);
    }

    public void DrawCircle(GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private IEnumerator WaitingTheTurn()
    {
        gameObject.GetComponent<EnemyAI>().enabled = false;
        yield return new WaitForSeconds(3);
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<TurnPlayer>().isYourTurn();
    }

    IEnumerator WaitingASeconds()
    {
        yield return new WaitForSeconds(8);
        AttackPlayer();
    }
}
