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

    private int currentMode = 1; // 1 = Bueno, -1 = Malo
    private int currentPersonCount;
    private bool isFirstRound = true; // Indica si es la primera ronda

    public int vivos;
    public int muertos;
    private void Start()
    {
        currentPersonCount = initialPersonCount;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnPersons(currentPersonCount);

            yield return new WaitForSeconds(30f);

            // Cambia el modo (bueno/malo)
            currentMode *= -1;

            // Actualiza la cantidad de personas para la próxima generación
            if (currentMode == -1)
            {
                currentPersonCount += incrementValue;
            }

            // Después de la primera ronda, desactiva la bandera
            isFirstRound = false;

        }
    }

    private void SpawnPersons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject personObject = Instantiate(personPrefab, spawn.position, Quaternion.identity);
            Person person = personObject.GetComponent<Person>();
            person.PatrolPoints = patrolPoints;
            person.Shop = shop;
            person.Weights = weights;
            person.Food = food;

            // En la primera ronda, genera siempre con bodyFat de 5 en modo bueno
            if (isFirstRound && currentMode == 1)
            {
                person.BodyFat = 5f;
            }
            else
            {
                person.BodyFat = Random.Range(4f, 7f);
            }
        }
    }
}
