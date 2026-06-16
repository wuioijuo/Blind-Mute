using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 80f, 0f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
    }
}
