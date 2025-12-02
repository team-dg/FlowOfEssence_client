using UnityEngine;

public class MinionMove : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(-20, 1.8f, 91.1f);
    public Vector3 endPosition = new Vector3(-128, 1.8f, -15.7f);
    public float speed = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // 목표에 도착했는지 확인
        if (transform.position == endPosition)
        {
            Debug.Log("Minion reached the target position.");
        }
    }
}
