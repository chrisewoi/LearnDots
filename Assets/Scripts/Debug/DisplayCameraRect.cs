using UnityEngine;
[ExecuteInEditMode]
public class DisplayCameraRect : MonoBehaviour
{
    void Start()
    {
        if (Application.isPlaying)
            DestroyImmediate(gameObject);
    }
    Camera mainCamera;
    public bool drawCardArea;
    private void OnDrawGizmos()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            return;
        }

        Vector3 a = mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector3 b = mainCamera.ScreenToWorldPoint(Vector2.right * mainCamera.pixelWidth);
        Vector3 c = mainCamera.ScreenToWorldPoint(Vector2.right * mainCamera.pixelWidth + Vector2.up * mainCamera.pixelHeight);
        Vector3 d = mainCamera.ScreenToWorldPoint(Vector2.up * mainCamera.pixelHeight);
        a.z = b.z = c.z = d.z = 0f;

        Gizmos.DrawLineStrip(new Vector3[] { a, b, c, d }, true);

        if(drawCardArea)
        {
            Vector3 cardAreaA = new Vector3(a.x, -20f, a.z);
            Vector3 cardAreaB = new Vector3(b.x, -20f, b.z);
            Gizmos.DrawLine(cardAreaA, cardAreaB);
        }
    }
}
