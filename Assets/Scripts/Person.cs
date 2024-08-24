using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private float normalSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float bodyFatIncrease;
    [SerializeField] private float bodyFatDecrease;
    private float currentSpeed;
    private Rigidbody2D _CompRigidbody2D;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private State currentState;
    [SerializeField] private Transform food;
    [SerializeField] private Transform weights;
    [SerializeField] private Animator animator;  // Referencia al Animator
    public Action OnFood;
    public Action OnExercise;

    [SerializeField] private Image lifeBar;  // Barra de vida
    [SerializeField] private Image bodyFatBar;  // Barra de grasa corporal
    [SerializeField] private Color safeColor = Color.yellow;  // Color seguro (amarillo)
    [SerializeField] private Color dangerColor = Color.red;  // Color peligroso (rojo)

    private bool isEatingOrExercising = false;
    private Transform target;
    private PersonSpawner spawner;

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
        set
        {
            bodyFat = Mathf.Clamp(value, 0, max);
            UpdateBodyFatBar();  // Actualiza la barra de grasa corporal
        }
    }
    public float Life
    {
        get { return life; }
        set
        {
            life = Mathf.Clamp(value, 0, max);
            UpdateLifeBar();  // Actualiza la barra de vida
        }
    }

    public void Initialize(PersonSpawner spawner)
    {
        this.spawner = spawner;
    }

    private void Start()
    {
        currentState = State.MovingToShop;
        _CompRigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();  // Obtén el componente Animator
        OnFood += TriggerEating;
        OnExercise += TriggerExercising;
        StartCoroutine(HealthManagement());
        UpdateLifeBar();  // Inicializa la barra de vida
        UpdateBodyFatBar();  // Inicializa la barra de grasa corporal
        currentSpeed = normalSpeed;  // Establece la velocidad inicial como la velocidad normal
    }

    private void OnDestroy()
    {
        OnFood -= TriggerEating;
        OnExercise -= TriggerExercising;
        if (spawner != null)
        {
            spawner.NotifyDeath();
        }
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
                currentSpeed = normalSpeed;  // Establece la velocidad normal
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
                currentSpeed = runningSpeed;  // Establece la velocidad de correr
                SetTarget(food);
                MoveTowardsTarget();
                if (Vector2.Distance(transform.position, food.position) <= 0.1f)
                {
                    StartCoroutine(HandleEating());
                }
                break;

            case State.Exercising:
                currentSpeed = runningSpeed;  // Establece la velocidad de correr
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

        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, currentSpeed * Time.fixedDeltaTime);
        _CompRigidbody2D.MovePosition(newPosition);
    }

    private IEnumerator HandleEating()
    {
        isEatingOrExercising = true;
        animator.SetBool("IsEating", true);  // Activa la animación de comer
        AudioManager.Instance.PlaySfx(0);
        BodyFat += bodyFatIncrease/2;
        yield return new WaitForSeconds(0.5f);  // Espera a que termine la animación
        BodyFat += bodyFatIncrease / 2;
        animator.SetBool("IsEating", false);  // Desactiva la animación de comer
        currentState = State.Patrolling;
        isEatingOrExercising = false;
    }

    private IEnumerator HandleExercising()
    {
        isEatingOrExercising = true;
        animator.SetBool("IsExercising", true);  // Activa la animación de ejercitarse
        AudioManager.Instance.PlaySfx(1);
        BodyFat -= bodyFatDecrease / 2;
        yield return new WaitForSeconds(0.5f);  // Espera a que termine la animación
        BodyFat -= bodyFatDecrease / 2;
        animator.SetBool("IsExercising", false);  // Desactiva la animación de ejercitarse
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
                Life--;
            }

            if (life <= 0)
            {
                Destroy(gameObject);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateLifeBar()
    {
        lifeBar.fillAmount = life / max;
    }

    private void UpdateBodyFatBar()
    {
        bodyFatBar.fillAmount = bodyFat / max;

        if (bodyFat <= minBodyFat || bodyFat >= maxBodyFat)
        {
            bodyFatBar.color = dangerColor;
        }
        else
        {
            bodyFatBar.color = safeColor;
        }
    }

    public void TriggerEating()
    {
        if (!isEatingOrExercising && currentState != State.Eating)
        {
            currentState = State.Eating;
        }
    }

    public void TriggerExercising()
    {
        if (!isEatingOrExercising && currentState != State.Exercising)
        {
            currentState = State.Exercising;
        }
    }
}
