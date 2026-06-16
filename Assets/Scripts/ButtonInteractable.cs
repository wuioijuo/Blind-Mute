using UnityEngine;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    public DoorController door;
    public string buttonName = "аварийная кнопка";

    public void Interact()
    {
        if (door != null)
        {
            door.TryOpen();
        }
    }

    public string GetHintText()
    {
        if (door != null && door.isLocked)
        {
            return "E — нажать, но сначала нужны все фрагменты";
        }

        return "E — нажать " + buttonName;
    }
}
