using UnityEngine;
using System.Collections;

public static class TransformExtensions
{
    #region Position
    public static void SetXPos(Transform transform, float x)
    {
        Vector3 newPosition =
           new Vector3(x, transform.position.y, transform.position.z);

        transform.position = newPosition;
    }

    public static void SetYPos(Transform transform, float y)
    {
        Vector3 newPosition =
           new Vector3(transform.position.x, y, transform.position.z);

        transform.position = newPosition;
    }

    public static void SetZPos(Transform transform, float z)
    {
        Vector3 newPosition =
           new Vector3(transform.position.x, transform.position.y, z);

        transform.position = newPosition;
    }
    #endregion

    #region Rotation
    public static void SetXRot(Transform transform, float x)
    {
        Vector3 newRotation =
           new Vector3(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Euler(newRotation);
    }

    public static void SetYRot(Transform transform, float y)
    {
        Vector3 newRotation =
           new Vector3(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Euler(newRotation);
    }

    public static void SetZRot(Transform transform, float z)
    {
        Vector3 newRotation =
           new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, z);

        transform.rotation = Quaternion.Euler(newRotation);
    }
    #endregion

    #region Scale
    public static void SetXScale(Transform transform, float x)
    {
        Vector3 newScale =
           new Vector3(x, transform.localScale.y, transform.localScale.z);

        transform.localScale = newScale;
    }

    public static void SetYScale(Transform transform, float y)
    {
        Vector3 newScale =
           new Vector3(transform.localScale.x, y, transform.localScale.z);

        transform.localScale = newScale;
    }

    public static void SetZScale(Transform transform, float z)
    {
        Vector3 newScale =
           new Vector3(transform.localScale.x, transform.localScale.y, z);

        transform.localScale = newScale;
    }
    #endregion

}
