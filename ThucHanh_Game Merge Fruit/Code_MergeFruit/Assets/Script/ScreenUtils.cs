using UnityEngine;

public static class ScreenUtils
{
    public static Bounds GetScreenBounds(Camera cam)
    {
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector3 center = cam.transform.position;

        return new Bounds(center, new Vector3(camWidth, camHeight, 0));
    }

    public static Vector3 ClampInsideScreen(Vector3 pos, float radius)
    {
        Bounds bounds = GetScreenBounds(Camera.main);
        float minX = bounds.min.x + radius;
        float maxX = bounds.max.x - radius;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        return pos;
    }
}
