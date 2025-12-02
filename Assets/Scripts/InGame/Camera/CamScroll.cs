using UnityEngine;

public class CamScroll : MonoBehaviour
{
    public Camera cam;
    public float zoomspeed=10;

    private float firstValue;
    private float mouseScrollInput;
    void Start()
    {
        firstValue = cam.fieldOfView;
    }
    void Update()
    {
        mouseScrollInput = Input.GetAxis("Mouse ScrollWheel");
        firstValue -= mouseScrollInput * zoomspeed;
        firstValue = Mathf.Clamp(firstValue, 10, 27);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, firstValue, zoomspeed);
    }
}
