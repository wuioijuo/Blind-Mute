using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int requiredItems = 5;
    public int collectedItems;
    public UIManager uiManager;
    public DoorController exitDoor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (uiManager != null)
        {
            uiManager.UpdateItems(collectedItems, requiredItems);
            uiManager.SetObjective("Найди 5 фрагментов, используй SPACE для эха");
            uiManager.ShowTemporaryMessage("Blind & Mute: Найди фрагменты и выход.", 4f);
        }
    }

    public void AddItem(string itemName)
    {
        collectedItems++;

        if (uiManager != null)
        {
            uiManager.UpdateItems(collectedItems, requiredItems);
            uiManager.ShowTemporaryMessage("Найдено: " + itemName, 1.4f);
        }

        if (collectedItems >= requiredItems)
        {
            if (exitDoor != null) exitDoor.UnlockDoor();
            if (uiManager != null)
            {
                uiManager.SetObjective("Все фрагменты собраны. Найди кнопку и открой выход.");
                uiManager.ShowTemporaryMessage("Все фрагменты найдены. Финальная дверь разблокирована.", 3f);
            }
        }
    }

    public bool HasAllItems()
    {
        return collectedItems >= requiredItems;
    }

    public void FinishLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(SceneNames.WinScene);
    }
}
