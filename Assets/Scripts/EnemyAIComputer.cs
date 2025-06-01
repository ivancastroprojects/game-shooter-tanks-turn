using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIComputer : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Animations of tank
    public AudioClip inIdle;
    public AudioClip inMovement;
    public ParticleSystem fireParticle;
    Vector3 placeOfPlayer;

    //Variables of circle to limit zone
    private GameObject containerCircle;
    float radiusLimit; //radius of limit zone
    public Vector3 centerPosition; //center of limiter circle
    float distance; //tank <---> center of circle
    float distanceToPlayer; //tank <----> tank

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timeLeft = 10;
    public int countPatrols;

    //Attacking
    bool alreadyAttacked;
    public GameObject m_Shell;
    public Transform m_FireTransform;
    public Transform tower;

    //IA Mode
    int difficulty;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private AudioSource audioSource; // Variable para cachear el AudioSource


    public void Start()
    {
        // Lo que DEBE quedar en Start (se ejecuta UNA SOLA VEZ por vida del script):
        player = GameObject.Find("TankPlayer1").transform; // Encontrar al jugador una vez
        difficulty = PlayerPrefs.GetInt("difficulty"); // Obtener la dificultad una vez
        agent = GetComponent<NavMeshAgent>(); // Obtener referencia al NavMeshAgent una vez
        audioSource = GetComponent<AudioSource>(); // Cachear el componente AudioSource una vez

        // El resto se moverá a OnEnable
    }

    void OnEnable()
    {
        // Esta función se llama CADA VEZ que el script se activa (inicio de cada turno de la IA)
        
        // Resetear estado de ataque
        alreadyAttacked = false;

        // Reinicializar el círculo de movimiento
        radiusLimit = 50; // O el valor que desees para el radio
        centerPosition = transform.localPosition; // Nueva posición central para este turno

        // Destruir el círculo anterior si existía (importante para no duplicar)
        if (containerCircle != null) 
        {
            Destroy(containerCircle);
        }
        
        containerCircle = new GameObject("LimitCircle_" + gameObject.name + Time.frameCount); // Nombre único con frameCount para evitar problemas
        containerCircle.transform.position = gameObject.transform.position + new Vector3(0f, 1f, 0f);
        DrawCircle(containerCircle, radiusLimit, 0.5f);

        if (audioSource != null) 
        {
            audioSource.clip = inMovement; // Preparar audio de movimiento
        }
        
        // Asegurarse de que el agente esté activo si fue desactivado
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
        if(agent != null) // Resetear cualquier ruta anterior para forzar una nueva decisión
        {
            agent.ResetPath();
        }

        walkPointSet = false; // Para que busque un nuevo punto de patrulla si es necesario
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //Range of movement of IA
        distance = Vector3.Distance(transform.position, centerPosition); //distance from tank to center of circle
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log(distanceToPlayer);

        // if (agent.isStopped) // Comentado temporalmente para depuración
        //     wakeUpIA(agent.transform.position);

        if (distance < 47)
        {
            if (distanceToPlayer > sightRange) Patroling();
         
            if (distanceToPlayer < sightRange && distanceToPlayer > attackRange) ChasePlayer();

            if (distanceToPlayer <= attackRange) AttackPlayer(difficulty);
            
        }
        else
        {
            AttackPlayer(1);
            Debug.Log("3");
        }
    }

    /**private void Update()
    {
        //Check for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        //playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //Range of movement of IA
        distance = Vector3.Distance(transform.position, centerPosition); //distance from tank to center of circle
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //If we are into of bounds of circle, we can patroling, chase us or attack us
        if (distance <= 46)
        {
            gameObject.GetComponent<AudioSource>().clip = inMovement;
            gameObject.GetComponent<AudioSource>().Play();

            //if (!playerInSightRange && !playerInAttackRange) Patroling();
            //if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            //if (playerInAttackRange && playerInSightRange) AttackPlayer(difficulty);

            if (distanceToPlayer < sightRange) ChasePlayer();
            else Patroling();
            if (distanceToPlayer < attackRange) AttackPlayer(difficulty);
            else ChasePlayer();
        }
        //If we are in bounds of circle, stop and shoot randomly
        else AttackPlayer(1);
    }*/

    private void Patroling() //meanwhile you not arrive to that random place you have to continue
    {
        if (audioSource != null && !audioSource.isPlaying) // Usar la variable cacheada y verificar si ya está sonando
        {
            audioSource.Play();
        }

        Debug.Log("patroling");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint() //Function useful to create a random point to follow it meanwhile patroling
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + 0.5f, transform.position.z + randomZ); // Elevamos un poco el origen del rayo por si el pivote del tanque está en su base
        
        Debug.DrawRay(walkPoint, -transform.up * 2.5f, Color.blue, 1.0f); // Dibujar el rayo (aumenté un poco la distancia para ver mejor)
        Debug.Log($"[AI Debug] SearchWalkPoint: Calculated walkPoint = {walkPoint}");

        if (Physics.Raycast(walkPoint, -transform.up, 2.5f, whatIsGround)) // Aumenté un poco la distancia del raycast
        {
            walkPointSet = true;
            Debug.Log("[AI Debug] SearchWalkPoint: Raycast hit ground. walkPointSet = true");
        }
        else
        {
            walkPointSet = false; // Asegurarse de que se pone a false si no golpea
            Debug.LogWarning("[AI Debug] SearchWalkPoint: Raycast DID NOT hit ground. walkPointSet = false");
        }
    }

    public void ChasePlayer() //Function hunter. The IA follow you because youre in his sight
    {
        // agent.ResetPath(); // Comentamos esto para probar si es la causa del no movimiento
        Debug.Log("Chasing the player");
        if (player != null && agent != null && agent.isOnNavMesh) // Asegurarse de que todo es válido
        {
            // Solo establece el destino si no está ya calculando una ruta hacia ese destino
            // o si el destino se ha movido significativamente.
            // Esto es una simplificación; una mejor comprobación implicaría ver si agent.destination está cerca de player.position.
            if (!agent.pathPending || Vector3.Distance(agent.destination, player.position) > 1.0f) // Umbral de 1 unidad
            {
                agent.SetDestination(player.position);
                Debug.Log($"[AI Chase Debug] SetDestination called for {player.name} at {player.position}.");
            }

            Debug.Log($"[AI Chase Debug] Agent.destination: {agent.destination}, Player.position: {player.position}");
            Debug.Log($"[AI Chase Debug] Agent.hasPath: {agent.hasPath}, Agent.pathPending: {agent.pathPending}, Agent.pathStatus: {agent.pathStatus}, Agent.isStopped: {agent.isStopped}, Agent.remainingDistance: {agent.remainingDistance}");
        }
        else
        {
            Debug.LogWarning("[AI Chase Debug] Cannot Chase: Player, Agent, or Agent not on NavMesh.");
        }
    }

    private void AttackPlayer(int difficulty) //Function IA attack. 2 options: you are too near or the IA arrives to his limit distance
    {
        Debug.Log("Attacking");

        //Make sure enemy doesn't move
        if (!agent)
            agent.SetDestination(transform.position);

        Vector3 direction = player.gameObject.transform.GetChild(4).position - m_FireTransform.position;

        transform.LookAt(player.gameObject.transform.GetChild(4).position);
        //tower.LookAt(player.gameObject.transform.GetChild(4).position);

        if (!alreadyAttacked)
        {
            //Direction of projectile
            Rigidbody rb = Instantiate(m_Shell, m_FireTransform.position, Quaternion.identity).GetComponent<Rigidbody>();

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
                //direction += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);

                rb.AddForce(direction * 36f, ForceMode.Impulse);
                rb.AddForce(transform.up * 10f, ForceMode.Impulse);
            }

            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            fireParticle.Play();
            Destroy(rb, 2.5f);
            Destroy(containerCircle);

            agent.ResetPath();
            tower.rotation = gameObject.transform.rotation;

            StartCoroutine(WaitingTheTurn());

            alreadyAttacked = true;
        }
    }

    private void wakeUpIA(Vector3 pos)
    {
        //If we pass 10 segs patroling, we shoot and change the turn
        var count =+ Time.deltaTime;
        Debug.Log(Time.deltaTime);
        if (count >= 10)
        {
            count = 0;
            if(agent.transform.position == pos)
                WaitingTheTurn();

            alreadyAttacked = true;
        }
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
        // gameObject.GetComponent<EnemyAIComputer>().enabled = false; // No es necesario obtener el componente de nuevo
        this.enabled = false; // Simplemente deshabilitar este script
        yield return new WaitForSeconds(3);
        GameObject turnPlayerObject = GameObject.FindGameObjectWithTag("Canvas");
        if (turnPlayerObject != null)
        {
            TurnPlayer turnPlayerScript = turnPlayerObject.GetComponent<TurnPlayer>();
            if (turnPlayerScript != null)
            {
                turnPlayerScript.isYourTurn();
            }
            else
            {
                Debug.LogError("[AI Debug] TurnPlayer script not found on Canvas object!");
            }
        }
        else
        {
            Debug.LogError("[AI Debug] Canvas object with tag 'Canvas' not found!");
        }
    }

    private IEnumerator givingTime()
    {
        yield return new WaitForSeconds(5);
    }
}
