using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonClickHandler : MonoBehaviour
{
    private Vector2 position;
    private Camera mainCamera;
    private Person selectedPerson;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    public void Position(InputAction.CallbackContext mouse)
    {
        position = mouse.ReadValue<Vector2>();
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleClick(ClickType.Left);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleClick(ClickType.Right);
        }
    }

    private void HandleClick(ClickType clickType)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(position);

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("person"))
        {
            selectedPerson = hit.collider.GetComponent<Person>();

            if (clickType == ClickType.Left)
            {
                selectedPerson.OnFood?.Invoke();
            }
            else if (clickType == ClickType.Right)
            {
                selectedPerson.OnExercise?.Invoke();
            }
        }
    }

    private enum ClickType
    {
        Left,
        Right
    }
}
