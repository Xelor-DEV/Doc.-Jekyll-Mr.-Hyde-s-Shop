using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        currentPersonCount = initialPersonCount;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            ResetRoundCounters();
            SpawnPersons(currentPersonCount);

            yield return new WaitForSeconds(10);

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
        }
    }

    private void SpawnPersons(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject personObject = Instantiate(personPrefab, spawn.position, Quaternion.identity);
            Person person = personObject.GetComponent<Person>();
            person.OnDeath += Person_OnDeath;
            person.OnSpawn += Person_OnSpawn;
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

            // Suscribirse a los eventos de instancia


            vivos++; // Aumenta vivos al spawn
        }
    }

    private void Person_OnSpawn()
    {
        // Este evento se maneja directamente en SpawnPersons ahora
    }

    private void Person_OnDeath()
    {
        vivos--;
        muertos++;
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

    private void OnDestroy()
    {
        // Limpiar la lista de personas al destruir el spawner
        foreach (Person person in FindObjectsOfType<Person>())
        {
            person.OnDeath -= Person_OnDeath;
            person.OnSpawn -= Person_OnSpawn;
        }
    }
}
