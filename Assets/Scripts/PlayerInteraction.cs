using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3.2f;
    public Camera playerCamera;
    public UIManager uiManager;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        IInteractable interactable = GetLookedAtInteractable();

        if (interactable != null)
        {
            if (uiManager != null) uiManager.ShowHint(interactable.GetHintText());

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact();
            }
        }
        else
        {
            if (uiManager != null) uiManager.HideHint();
        }
    }

    private IInteractable GetLookedAtInteractable()
    {
        if (playerCamera == null) return null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();
            }
            return interactable;
        }

        return null;
    }
}
