using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isLocked = true;
    public float openHeight = 3.8f;
    public float openSpeed = 2.0f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void TryOpen()
    {
        if (isLocked)
        {
            if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
            {
                GameManager.Instance.uiManager.ShowTemporaryMessage("Дверь не реагирует. Нужны все фрагменты.", 2f);
            }
            return;
        }

        if (!isOpening)
        {
            StartCoroutine(OpenRoutine());
        }
    }

    private IEnumerator OpenRoutine()
    {
        isOpening = true;
        while (Vector3.Distance(transform.position, openPosition) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, openSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
