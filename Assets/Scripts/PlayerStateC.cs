using UnityEngine;
using TMPro;

public class PlayerStateTimer : MonoBehaviour
{
    [SerializeField] private float goodStateDuration = 30f; // Tiempo en segundos en el estado bueno
    [SerializeField] private float badStateDuration = 20f;  // Tiempo en segundos en el estado malo
    [SerializeField] private TextMeshProUGUI stateText;     // Texto para mostrar el estado actual
    [SerializeField] private TextMeshProUGUI timerText;     // Texto para mostrar el tiempo restante
    [SerializeField] private TMP_FontAsset goodStateFont;   // Fuente para el estado bueno
    [SerializeField] private TMP_FontAsset badStateFont;    // Fuente para el estado malo
    [SerializeField] private Color goodStateColor = Color.green; // Color para el estado bueno
    [SerializeField] private Color badStateColor = Color.red;    // Color para el estado malo

    public delegate void StateChangeHandler();
    public event StateChangeHandler OnStateChange;

    private float timeRemaining;
    private bool isGoodState = true;
    public bool isTimerRunning = false;

    private void Start()
    {
        if (stateText == null)
        {
            Debug.LogError("TextMeshProUGUI para el estado no está asignado en el PlayerStateTimer.");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("TextMeshProUGUI para el temporizador no está asignado en el PlayerStateTimer.");
            return;
        }

        if (goodStateFont == null || badStateFont == null)
        {
            Debug.LogError("Fuentes no asignadas en el PlayerStateTimer.");
            return;
        }

        // Iniciar en el estado bueno (puede ser el estado predeterminado)
        SetGoodState();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                ToggleState(); // Cambia de estado cuando el tiempo se agota
            }

            // Actualiza el texto del temporizador
            UpdateTimerText();
        }
    }

    private void ToggleState()
    {
        isGoodState = !isGoodState;

        // Notifica el cambio de estado
        OnStateChange?.Invoke();

        if (isGoodState)
        {
            SetGoodState();
        }
        else
        {
            SetBadState();
        }
    }

    public void SetState(int mode)
    {
        // mode = 1 para Bueno, -1 para Malo
        if (mode == 1)
        {
            SetGoodState();
        }
        else if (mode == -1)
        {
            SetBadState();
        }
    }

    private void SetGoodState()
    {
        timeRemaining = goodStateDuration;
        stateText.text = "DR. JEKYLL";
        stateText.font = goodStateFont;  // Cambia la fuente para el estado bueno
        stateText.color = goodStateColor; // Cambia el color para el estado bueno
        timerText.font = goodStateFont;  // Cambia la fuente para el temporizador
        timerText.color = goodStateColor; // Cambia el color para el temporizador
        isTimerRunning = true;
    }

    private void SetBadState()
    {
        timeRemaining = badStateDuration;
        stateText.text = "MR. HYDE";
        stateText.font = badStateFont;   // Cambia la fuente para el estado malo
        stateText.color = badStateColor; // Cambia el color para el estado malo
        timerText.font = badStateFont;   // Cambia la fuente para el temporizador
        timerText.color = badStateColor; // Cambia el color para el temporizador
        isTimerRunning = true;
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
