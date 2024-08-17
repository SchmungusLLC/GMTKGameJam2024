using UnityEngine;

public static class HelperMethods
{
    private static Matrix4x4 isoRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 IsoRotation(this Vector3 input) => isoRotationMatrix.MultiplyPoint3x4(input);

    public static bool ContainsLayer(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
