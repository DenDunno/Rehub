using UnityEngine;

public static class BoundsExtensions
{
    public static Vector3 GetRandomPositionInBox(this Bounds bounds)
    {
        Vector3 boundsMax = bounds.max;
        Vector3 boundsMin = bounds.min;

        float randomX = GetRandomAxisPosition(boundsMin.x, boundsMax.x);
        float randomY = GetRandomAxisPosition(boundsMin.y, boundsMax.y);
        float randomZ = GetRandomAxisPosition(boundsMin.z, boundsMax.z);

        return new Vector3(randomX, randomY, randomZ);
    }

    private static float GetRandomAxisPosition(float min, float max)
    {
        float lerp = Random.Range(0f, 1);
        return Mathf.Lerp(min, max, lerp);
    }
}