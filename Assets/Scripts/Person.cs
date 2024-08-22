using System.Collections;
using System;
using UnityEngine;

public class Person : MonoBehaviour
{
    private enum State
    {
        MovingToShop,
        Patrolling,
        Eating,
        Exercising,
        Idle // Añadido para manejar estados intermedios
    }

    [SerializeField] private float bodyFat; // 0 - 10
    [SerializeField] private float life = 9;
    [SerializeField] private Transform shop;
    [SerializeField] private float speed;
    private Rigidbody2D _CompRigidbody2D;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private State currentState;
    [SerializeField] private Transform food;
    [SerializeField] private Transform weights;
    public static Action OnFood;
    public static Action OnExercise;

    private bool isEatingOrExercising = false; // Nuevo flag para evitar múltiples llamadas

    public Transform[] PatrolPoints
    {
        set
        {
            patrolPoints = value;
        }
    }
    public Transform Food
    {
        get { return food; }
        set { food = value; }
    }

    public Transform Weights
    {
        get { return weights; }
        set { weights = value; }
    }

    public Transform Shop
    {
        get { return shop; }
        set { shop = value; }
    }
    public float BodyFat
    {
        get
        {
            return bodyFat;
        }
        set
        {
            bodyFat = value;
        }
    }

    public float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
        }
    }

    private void Start()
    {
        currentState = State.MovingToShop;
        StartCoroutine(HealthManagement());
    }

    private void Awake()
    {
        _CompRigidbody2D = GetComponent<Rigidbody2D>();
        OnFood += TriggerEating;
        OnExercise += TriggerExercising;
    }

    private void OnDestroy()
    {
        OnFood -= TriggerEating;
        OnExercise -= TriggerExercising;
    }

    private IEnumerator MoveToShop()
    {
        while (Vector2.Distance(transform.position, shop.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, shop.position, speed * Time.deltaTime);
            _CompRigidbody2D.MovePosition(newPosition);
            yield return null;
        }
        currentState = State.Patrolling;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.MovingToShop:
                if (!isEatingOrExercising) StartCoroutine(MoveToShop());
                break;
            case State.Patrolling:
                if (!isEatingOrExercising) StartCoroutine(Patrol());
                break;
            case State.Eating:
                if (!isEatingOrExercising) StartCoroutine(Eat());
                break;
            case State.Exercising:
                if (!isEatingOrExercising) StartCoroutine(Exercise());
                break;
        }
    }

    private IEnumerator Eat()
    {
        isEatingOrExercising = true;

        while (Vector2.Distance(transform.position, food.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, food.position, speed * Time.deltaTime);
            _CompRigidbody2D.MovePosition(newPosition);
            yield return null;
        }

        yield return new WaitForSeconds(1); // Pausa antes de cambiar el bodyFat
        bodyFat += 4;  // Incrementar bodyFat

        yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5));
        currentState = State.Patrolling;  // Volver a patrullar
        isEatingOrExercising = false;
    }

    private IEnumerator Exercise()
    {
        isEatingOrExercising = true;

        while (Vector2.Distance(transform.position, weights.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, weights.position, speed * Time.deltaTime);
            _CompRigidbody2D.MovePosition(newPosition);
            yield return null;
        }

        yield return new WaitForSeconds(1); // Pausa antes de cambiar el bodyFat
        bodyFat -= 2;  // Disminuir bodyFat

        yield return new WaitForSeconds(1);
        currentState = State.Patrolling;  // Volver a patrullar
        isEatingOrExercising = false;
    }

    private IEnumerator Patrol()
    {
        Transform randomPatrolPoint = patrolPoints[UnityEngine.Random.Range(0, patrolPoints.Length)];
        while (Vector2.Distance(transform.position, randomPatrolPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, randomPatrolPoint.position, speed * Time.deltaTime);
            _CompRigidbody2D.MovePosition(newPosition);
            yield return null;
        }
        yield return new WaitForSeconds(1);
    }

    private void TriggerEating()
    {
        if (!isEatingOrExercising)
        {
            currentState = State.Eating;
        }
    }

    private void TriggerExercising()
    {
        if (!isEatingOrExercising)
        {
            currentState = State.Exercising;
        }
    }

    private IEnumerator HealthManagement()
    {
        while (true)
        {
            if (bodyFat > 0)
            {
                bodyFat--;
            }
            else if (life > 0)
            {
                life--;
                if (life <= 0)
                {
                    Destroy(gameObject);
                    yield break;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

