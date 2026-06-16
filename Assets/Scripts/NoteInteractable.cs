using UnityEngine;

public class NoteInteractable : MonoBehaviour, IInteractable
{
    [TextArea(2, 6)]
    public string noteText = "Записка";

    public void Interact()
    {
        if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
        {
            GameManager.Instance.uiManager.ShowTemporaryMessage(noteText, 4f);
        }
    }

    public string GetHintText()
    {
        return "E — прочитать записку";
    }
}
