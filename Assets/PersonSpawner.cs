using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PersonSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private GameObject personPrefab;
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform[] food;
    [SerializeField] private Transform[] weights;
    [SerializeField] private Transform shop;
    [SerializeField] private int initialPersonCount = 5;
    [SerializeField] private int incrementedValue = 2; // Nuevo valor incrementado inicial
    [SerializeField] private int incrementer = 1; // Nuevo incrementador

    [SerializeField] private int currentMode = 1; // 1 = Bueno, -1 = Malo
    [SerializeField] private int currentPersonCount;
    [SerializeField] private bool isFirstRound = true;
    [SerializeField] private int currentRound = 1;
    [SerializeField] private int score = 0;

    [SerializeField] private int vivos;
    [SerializeField] private int muertos;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private PlayerStateTimer playerStateTimer; // Referencia al PlayerStateTimer
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject win;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject lose;

    [SerializeField] private Color goodModeColor; // Color para el modo bueno
    [SerializeField] private Color badModeColor; // Color para el modo malo
    [SerializeField] private Volume globalVolume; // Referencia al Global Volume

    private Vignette vignette;

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
        scoreText.text = score.ToString();
        UpdateBackgroundMusic();
        StartCoroutine(SpawnRoutine());
        // Obtener el componente Vignette del Global Volume
        if (globalVolume.profile.TryGet<Vignette>(out vignette))
        {
            UpdateVignetteColor();
        }
        else
        {
            Debug.LogError("No se pudo encontrar el componente Vignette en el Global Volume.");
        }
    }
    private void UpdateVignetteColor()
    {
        if (vignette != null)
        {
            vignette.color.value = currentMode == 1 ? goodModeColor : badModeColor;
        }
    }
    private void OnPlayerStateChange()
    {
        // Actualiza el puntaje antes de cambiar el estado
        UpdateScore();
        scoreText.text = score.ToString();

        // Cambia el modo (bueno/malo)
        AudioManager.Instance.PlaySfx(2);
        currentMode *= -1;

        // Actualiza la música de fondo según el nuevo modo
        UpdateBackgroundMusic();
        UpdateVignetteColor();
        // Actualiza la cantidad de personas para la próxima generación
        if (!isFirstRound)
        {
            incrementedValue += incrementer;
        }

        currentPersonCount = incrementedValue;

        // Incrementa la ronda y desactiva la bandera
        currentRound++;
        isFirstRound = false;

        // Empieza una nueva ronda
        StartCoroutine(SpawnRoutine());
    }

    private void UpdateBackgroundMusic()
    {
        if (AudioManager.Instance != null)
        {
            int musicIndex = currentMode == 1 ? 0 : 1;
            AudioManager.Instance.PlayMusic(musicIndex);
        }
        else
        {
            Debug.LogError("AudioManager no está asignado.");
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (!gameEnded)
        {
            ResetRoundCounters();
            yield return StartCoroutine(SpawnPersons(currentPersonCount));

            // Actualiza el puntaje al final de la ronda
            // ...

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
            person.Weights = weights[Random.Range(0,weights.Length - 1)];
            person.Food = food[Random.Range(0, food.Length - 1)];

            // En la primera ronda, genera siempre con bodyFat de 5 en modo bueno
            if (isFirstRound && currentMode == 1)
            {
                person.BodyFat = 6;
            }
            else
            {
                person.BodyFat = 6;
            }

            vivos++; // Aumenta vivos al spawn

            yield return new WaitForSeconds(0.2f); // Delay entre la generación de personas
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

        // Asegúrate de que el puntaje no sea menor a 0
        if (score < 0)
        {
            score = 0;
        }

        Debug.Log($"Ronda {currentRound}: Puntaje = {score}, Vivos = {vivos}, Muertos = {muertos}");
    }

    public void OnTimerEnd()
    {
        if (vivos > 0)
        {
            win.SetActive(true);
            Image pause1 = win.GetComponent<Image>();
            pause1.raycastTarget = true;
            uiManager.personsLive.text = vivos + " People Survived";
            uiManager.Pause();
        }
        else
        {
            lose.SetActive(true);
            Image pause1 = lose.GetComponent<Image>();
            pause1.raycastTarget = true;
            uiManager.Pause();
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
