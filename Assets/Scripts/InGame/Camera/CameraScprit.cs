using TMPro;
using UnityEngine;

public class CameraScprit : MonoBehaviour
{
    public Transform target;  // 카메라가 따라갈 타겟 (캐릭터)
    public Vector3 offset;    // 카메라와 타겟 간의 오프셋
    public Vector3 StartTransform;

    bool canMoveCamera = true;

    void LateUpdate()
    {
        // 카메라의 위치를 타겟의 위치에 맞춰 업데이트
        if (Input.GetKey(KeyCode.Space)|| !canMoveCamera)
        {
            transform.position = new Vector3(target.position.x + StartTransform.x, StartTransform.y, target.position.z + StartTransform.z);
        }
            
        //transform.position = target.position +StartTransform;
    }

    void Awake()
    {
        StartTransform = transform.position-target.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            transform.position = new Vector3(target.position.x + StartTransform.x, StartTransform.y, target.position.z + StartTransform.z);
            canMoveCamera = !canMoveCamera;
        }
        if(canMoveCamera)
        {
            Vector3 pos = transform.position;
            if (Input.mousePosition.y >= Screen.height - 10)// 스크린 좌표의 왼쪽 밑은 (0,0)이고 오른쪽 위는 (Screen.width, Screen.height)이다.
            {
                pos.z -= 20 * Time.deltaTime;
            }
            if (Input.mousePosition.y <= 10)
            {
                pos.z += 20 * Time.deltaTime;
            }
            if (Input.mousePosition.x >= 10)
            {
                pos.x -= 20 * Time.deltaTime;
            }
            if (Input.mousePosition.x <= Screen.width - 10)
            {
                pos.x += 20 * Time.deltaTime;
            }

            if (pos.x >= -139 && pos.x <= -4.5 && pos.z >= -13 && pos.z <= 135f)
            {
                transform.position = pos;
            }
        }
        
    }
}
 