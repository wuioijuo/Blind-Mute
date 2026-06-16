using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    public string itemName = "Фрагмент";

    public void Interact()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItem(itemName);
        }

        Destroy(gameObject);
    }

    public string GetHintText()
    {
        return "E — подобрать: " + itemName;
    }
}
