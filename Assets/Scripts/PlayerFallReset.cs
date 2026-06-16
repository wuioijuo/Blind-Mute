using UnityEngine;

public class PlayerFallReset : MonoBehaviour
{
    public Vector3 safePosition = new Vector3(-24f, 1.4f, -24f);
    public float minY = -3f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (transform.position.y < minY)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        if (controller != null) controller.enabled = false;
        transform.position = safePosition;
        if (controller != null) controller.enabled = true;
    }
}
