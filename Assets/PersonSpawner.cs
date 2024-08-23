using System.Collections;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private GameObject personPrefab;
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform food;
    [SerializeField] private Transform weights;
    [SerializeField] private Transform shop;
    [SerializeField] private int initialPersonCount = 5;
    [SerializeField] private int incrementValue = 1;
    [SerializeField] private int minBodyFat = 4;
    [SerializeField] private int maxBodyFat = 7;

    [SerializeField] private int currentMode = 1; // 1 = Bueno, -1 = Malo
    [SerializeField] private int currentPersonCount;
    [SerializeField] private bool isFirstRound = true;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int score = 0;

    [SerializeField] private int vivos;
    [SerializeField] private int muertos;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private PlayerStateTimer playerStateTimer; // Referencia al PlayerStateTimer

    private bool gameEnded = false;

    private void Start()
    {
        currentPersonCount = initialPersonCount;

        if (gameTimer == null)
        {
            Debug.LogError("GameTimer no está asignado en el PersonSpawner.");
            return;
        }

        if (playerStateTimer == null)
        {
            Debug.LogError("PlayerStateTimer no está asignado en el PersonSpawner.");
            return;
        }
        playerStateTimer.OnStateChange += OnPlayerStateChange;
        StartCoroutine(SpawnRoutine());
    }
    private void OnPlayerStateChange()
    {
        // Actualiza el puntaje antes de cambiar el estado
        UpdateScore();

        // Cambia el modo (bueno/malo)
        currentMode *= -1;

        // Actualiza la cantidad de personas para la próxima generación
        if (currentMode == -1)
        {
            currentPersonCount += incrementValue;
        }

        // Incrementa la ronda y desactiva la bandera
        currentRound++;
        isFirstRound = false;

        // Empieza una nueva ronda
        StartCoroutine(SpawnRoutine());
    }


    private IEnumerator SpawnRoutine()
    {
        while (!gameEnded)
        {
            ResetRoundCounters();
            yield return StartCoroutine(SpawnPersons(currentPersonCount));

            // Actualiza el puntaje al final de la ronda
            

            // Espera a que el estado cambie
            yield return new WaitUntil(() => playerStateTimer.isTimerRunning == false);
        }
    }

    private IEnumerator SpawnPersons(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject personObject = Instantiate(personPrefab, spawn.position, Quaternion.identity);
            Person person = personObject.GetComponent<Person>();
            person.Initialize(this); // Pasar la referencia del spawner
            person.PatrolPoints = patrolPoints;
            person.Shop = shop;
            person.Weights = weights;
            person.Food = food;

            // En la primera ronda, genera siempre con bodyFat de 5 en modo bueno
            if (isFirstRound && currentMode == 1)
            {
                person.BodyFat = 5;
            }
            else
            {
                person.BodyFat = Random.Range(minBodyFat, maxBodyFat + 1);
            }

            vivos++; // Aumenta vivos al spawn

            yield return new WaitForSeconds(0.1f); // Delay entre la generación de personas
        }
    }

    public void NotifyDeath()
    {
        vivos--;
        muertos++;
        CheckGameOver();
    }

    private void ResetRoundCounters()
    {
        muertos = 0; // Reiniciar solo el contador de muertos
    }

    private void UpdateScore()
    {
        int points = 0;

        if (currentMode == 1) // Modo Bueno
        {
            points = vivos * 10 - muertos * 10;
        }
        else // Modo Malo
        {
            points = muertos * 10 - vivos * 5;
        }

        score += points;
        Debug.Log($"Ronda {currentRound}: Puntaje = {score}, Vivos = {vivos}, Muertos = {muertos}");
    }

    public void OnTimerEnd()
    {
        if (vivos > 0)
        {
            Debug.Log("¡Ganaste!");
        }
        else
        {
            Debug.Log("Game Over");
        }
        gameEnded = true;
    }

    private void CheckGameOver()
    {
        if (vivos <= 0)
        {
            OnTimerEnd(); // Forzar el fin del juego si todas las personas mueren
        }
    }
}
