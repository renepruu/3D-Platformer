using UnityEngine;

public class UpDownFloat : MonoBehaviour
{
    public float amplitude = 1f; 
    public float speed = 1.5f;       

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + offset, startPos.z);
    }
}
