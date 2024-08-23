using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float gameDuration = 120f; // Duración total del juego en segundos
    [SerializeField] private PersonSpawner personSpawner; // Referencia al spawner de personas
    [SerializeField] private TMP_Text timerText; // Referencia al componente TextMeshProUGUI

    private float timeRemaining;
    private bool isTimerRunning = false;

    private void Start()
    {
        if (personSpawner == null)
        {
            Debug.LogError("PersonSpawner no está asignado en el GameTimer.");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("TextMeshProUGUI no está asignado en el GameTimer.");
            return;
        }

        timeRemaining = gameDuration;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;
                personSpawner.OnTimerEnd(); // Notificar al spawner que el temporizador terminó
            }

            // Actualiza el texto del temporizador
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}
