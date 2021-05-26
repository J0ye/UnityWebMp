using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LastFrameInfo
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public LastFrameInfo(Transform values)
    {
        position = values.position;
        scale = values.localScale;
        rotation = values.rotation;
    }

    /// <summary>
    /// Method to conmpare values of a target transform with the values of this entity.
    /// </summary>
    /// <param name="target">Compare to target transformn</param>
    /// <param name="decimalPlaces">Defines how many decimals are used for the comparision of a Vector3</param>
    /// <returns> Returns true if every value is the same as the target values.</returns>
    public bool CompareValues(Transform target, int decimalPlaces)
    {
        if (position.Round(decimalPlaces) != target.position.Round(decimalPlaces))
        {
            // Return false if the position is different from the target psotion
            return false;
        }

        if (scale.Round(decimalPlaces) != target.localScale.Round(decimalPlaces))
        {
            // Return false if the scale is different from the target scale
            return false;
        }

        // Return false if the rotation is different from target rotation and true if every value is the same as the target
        return rotation.Compare(target.rotation);
    }

    public void UpdateValues(Transform target)
    {
        position = target.position;
        scale = target.localScale;
        rotation = target.rotation;
    }
}

//###################################################################################################################################################################################################################################################

static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    /// <summary>
    /// Compares two rotations.
    /// </summary>
    /// <param name="quatA">The value of this Quaternion</param>
    /// <param name="value">Target to compare to</param>
    /// <param name="acceptableRange">Acceptable Range for the comparision. For example: 0.0000004f for 1 degree</param>
    /// <returns>Returns true if both angles are within the acceptable range</returns>
    public static bool Approximately(this Quaternion quatA, Quaternion value, float acceptableRange = 0.0000004f)
    {
        return 1 - Mathf.Abs(Quaternion.Dot(quatA, value)) < acceptableRange;
    }

    /// <summary>
    /// Compares two rotations
    /// </summary>
    /// <param name="quatA">This rotation</param>
    /// <param name="target">The target of this comparison</param>
    /// <param name="acceptableRange">The range in which the difference between the two rotations may lie. 1f = 1 degree</param>
    /// <returns>True if the difference bewteen both rotations is smaller then the range</returns>
    public static bool Compare(this Quaternion quatA, Quaternion target, float acceptableRange = 1f)
    {
        // Quaternion.Angle() returns the angle between two rotations in degree.
        return Quaternion.Angle(quatA, target) < acceptableRange;
    }
}