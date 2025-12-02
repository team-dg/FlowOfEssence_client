using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D clickCursor;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        if (defaultCursor != null)
        {
            SetDefaultCursor();
        }
        else
        {
            Debug.LogError("Default cursor texture is not set.");
        }
    }

    public void SetDefaultCursor()
    {
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogError("Default cursor texture is not set.");
        }
    }
}
