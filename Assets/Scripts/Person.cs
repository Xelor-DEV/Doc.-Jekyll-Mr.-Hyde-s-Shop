using System.Collections;
using UnityEngine;
using System;

public class Person : MonoBehaviour
{
    private enum State
    {
        MovingToShop,
        Patrolling,
        Eating,
        Exercising,
        Idle
    }

    [SerializeField] private float bodyFat; // 0 - 10
    [SerializeField] private float minBodyFat = 2;
    [SerializeField] private float maxBodyFat = 8;
    [SerializeField] private float max = 10;
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

    private bool isEatingOrExercising = false;
    private Transform target;

    public Transform[] PatrolPoints
    {
        set { patrolPoints = value; }
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
        get { return bodyFat; }
        set { bodyFat = Mathf.Clamp(value, 0, max); }
    }
    public float Life
    {
        get { return life; }
        set { life = value; }
    }

    public event Action OnSpawn;
    public event Action OnDeath;

    private void Start()
    {
        currentState = State.MovingToShop;
        _CompRigidbody2D = GetComponent<Rigidbody2D>();
        OnFood += TriggerEating;
        OnExercise += TriggerExercising;
        OnSpawn?.Invoke();
        StartCoroutine(HealthManagement());
    }

    private void OnDestroy()
    {
        OnFood -= TriggerEating;
        OnExercise -= TriggerExercising;
        OnDeath?.Invoke();
    }

    private void FixedUpdate()
    {
        if (isEatingOrExercising) return;

        switch (currentState)
        {
            case State.MovingToShop:
                SetTarget(shop);
                MoveTowardsTarget();
                if (Vector2.Distance(transform.position, shop.position) <= 0.1f)
                {
                    currentState = State.Patrolling;
                }
                break;

            case State.Patrolling:
                if (target == null)
                {
                    target = patrolPoints[UnityEngine.Random.Range(0, patrolPoints.Length)];
                }
                SetTarget(target);
                MoveTowardsTarget();
                if (Vector2.Distance(transform.position, target.position) <= 0.1f)
                {
                    target = null;
                }
                break;

            case State.Eating:
                SetTarget(food);
                MoveTowardsTarget();
                if (Vector2.Distance(transform.position, food.position) <= 0.1f)
                {
                    StartCoroutine(HandleEating());
                }
                break;

            case State.Exercising:
                SetTarget(weights);
                MoveTowardsTarget();
                if (Vector2.Distance(transform.position, weights.position) <= 0.1f)
                {
                    StartCoroutine(HandleExercising());
                }
                break;
        }
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
        _CompRigidbody2D.MovePosition(newPosition);
    }

    private IEnumerator HandleEating()
    {
        isEatingOrExercising = true;
        yield return null;
        BodyFat += 4;
        currentState = State.Patrolling;
        isEatingOrExercising = false;
    }

    private IEnumerator HandleExercising()
    {
        isEatingOrExercising = true;
        yield return null;
        BodyFat -= 2;
        currentState = State.Patrolling;
        isEatingOrExercising = false;
    }

    private IEnumerator HealthManagement()
    {
        while (life > 0)
        {
            BodyFat -= 1;

            if (bodyFat <= minBodyFat || bodyFat >= maxBodyFat)
            {
                life--;
            }

            if (life <= 0)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void TriggerEating()
    {
        if (!isEatingOrExercising && currentState != State.Eating)
        {
            currentState = State.Eating;
        }
    }

    private void TriggerExercising()
    {
        if (!isEatingOrExercising && currentState != State.Exercising)
        {
            currentState = State.Exercising;
        }
    }
}
