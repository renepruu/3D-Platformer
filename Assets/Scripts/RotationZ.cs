using UnityEngine;

public class RotateAroundZ : MonoBehaviour
{
    public float rotationSpeed = 60f;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
