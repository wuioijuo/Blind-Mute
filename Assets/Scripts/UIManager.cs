using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text itemCounterText;
    public Text hintText;
    public Text messageText;
    public Text objectiveText;

    private Coroutine messageRoutine;

    public void UpdateItems(int collected, int required)
    {
        if (itemCounterText != null)
        {
            itemCounterText.text = "Фрагменты: " + collected + "/" + required;
        }
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }

    public void ShowHint(string text)
    {
        if (hintText == null) return;
        hintText.text = text;
        hintText.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        if (hintText == null) return;
        hintText.gameObject.SetActive(false);
    }

    public void ShowMessage(string text)
    {
        if (messageText == null) return;
        messageText.text = text;
        messageText.gameObject.SetActive(true);
    }

    public void ShowTemporaryMessage(string text, float seconds)
    {
        if (messageText == null) return;

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(MessageRoutine(text, seconds));
    }

    private IEnumerator MessageRoutine(string text, float seconds)
    {
        ShowMessage(text);
        yield return new WaitForSeconds(seconds);
        if (messageText != null) messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }
}
