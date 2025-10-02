using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerationUtil
{
    /// <summary>
    /// Turns the Vector2Int into a Vector3. (Grid coordinates to Vector3, y is z)
    /// </summary>
    /// <param name="vectorToConvert">The Vector2Int to convert.</param>
    /// <returns>The Vector3 representation of the Vector2Int.</returns>
    public static Vector3 ConvertVector2IntToVector3(Vector2Int vectorToConvert)
    {
        return new Vector3(vectorToConvert.x, 0, vectorToConvert.y);
    }

    /// <summary>
    /// Get the opposite direction to that compass direction.
    /// </summary>
    /// <param name="direction">The direction to get a opposite for.</param>
    /// <returns>The opposite compass direction.</returns>
    public static CompassDirection GetOppositeDirection(CompassDirection direction)
    {
        CompassDirection oppositeDirection = CompassDirection.South;

        switch (direction)
        {
            case CompassDirection.North:
                oppositeDirection = CompassDirection.South;
                break;
            case CompassDirection.South:
                oppositeDirection = CompassDirection.North;
                break;
            case CompassDirection.West:
                oppositeDirection = CompassDirection.East;
                break;
            case CompassDirection.East:
                oppositeDirection = CompassDirection.West;
                break;
        }

        return oppositeDirection;
    }

    /// <summary>
    /// Returns a list of all valid level pieces with doorways facing that direction.
    /// </summary>
    /// <param name="directionToConnectTo">The doorway direction to look for.</param>
    /// <param name="levelPieceCollection">The level collection to look in.</param>
    /// <returns>A list with all pieces with at least on doorway facing that direction.</returns>
    public static List<RoomPiece> GetAllValidLevelPieces(CompassDirection directionToConnectTo, List<RoomPiece> levelPieceCollection)
    {
        directionToConnectTo = LevelGenerationUtil.GetOppositeDirection(directionToConnectTo);

        List<RoomPiece> validLevelPieces = new List<RoomPiece>();

        foreach (RoomPiece levelPiece in levelPieceCollection)
        {
            if (levelPiece.HasAnyDoorwayWithFacingDirection(directionToConnectTo))
            {
                validLevelPieces.Add(levelPiece);
            }

        }

        return validLevelPieces;
    }

    /// <summary>
    /// Turns the compass direction into a quaternion.
    /// </summary>
    /// <param name="direction">The compass direction to convert.</param>
    /// <returns>A <b>Quaternion</b> facing that direction (in world space).</returns>
    public static Quaternion GetCompassDirectionAsQuaternion(CompassDirection direction)
    {
        Quaternion rotation = Quaternion.identity;

        switch (direction)
        {
            case CompassDirection.North:
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case CompassDirection.East:
                rotation = Quaternion.Euler(0, 90, 0);
                break;
            case CompassDirection.South:
                rotation = Quaternion.Euler(0, 180, 0);
                break;
            case CompassDirection.West:
                rotation = Quaternion.Euler(0, 270, 0);
                break;
        }

        return rotation;
    }

    public static float GetCompassDirectionAsRotation(CompassDirection direction)
    {
        float degrees = 0;

        switch (direction)
        {
            case CompassDirection.North:
                degrees = 0; // yeah, yeah, you don't need this blah blah blah. ill refactor for speed later.
                break;
            case CompassDirection.East:
                degrees = 90;
                break;
            case CompassDirection.South:
                degrees = 180;
                break;
            case CompassDirection.West:
                degrees = 270;
                break;
        }

        return degrees;
    }

    public static Vector2Int GetCompassDirectionAsVector2Int(CompassDirection facingDirection)
    {
        Vector2Int direction = Vector2Int.zero;

        switch (facingDirection)
        {
            case CompassDirection.North:
                direction = Vector2Int.up;
                break;
            case CompassDirection.East:
                direction = Vector2Int.right;
                break;
            case CompassDirection.South:
                direction = Vector2Int.down;
                break;
            case CompassDirection.West:
                direction = Vector2Int.left;
                break;
        }

        return direction;
    }

    public static Vector2 GetCompassDirectionAsVector2(CompassDirection facingDirection)
    {
        Vector2 direction = Vector2.zero;

        switch (facingDirection)
        {
            case CompassDirection.North:
                direction = Vector2.up;
                break;
            case CompassDirection.East:
                direction = Vector2.right;
                break;
            case CompassDirection.South:
                direction = Vector2.down;
                break;
            case CompassDirection.West:
                direction = Vector2.left;
                break;
        }

        return direction;
    }

    public static CompassDirection RotateCompassDirectionByDegrees(CompassDirection compassDirectionToRotate, float degrees)
    {
        int rotationBy90Count = 0;

        if (degrees > 0)
        {
            rotationBy90Count = Mathf.FloorToInt(degrees / 90f);
        }


        if (rotationBy90Count == 0)
        {
            return compassDirectionToRotate;
        }

        // actual handling of rotating.
        if (compassDirectionToRotate == CompassDirection.North)
        {
            CompassDirection direction = CompassDirection.North;

            switch (rotationBy90Count)
            {
                case 1:
                    direction = CompassDirection.East;
                    break;

                case 2:
                    direction = CompassDirection.South;
                    break;

                case 3:
                    direction = CompassDirection.West;
                    break;

                default:
                    direction = CompassDirection.North;
                    break;
            }

            return direction;
        }
        else if (compassDirectionToRotate == CompassDirection.East)
        {
            CompassDirection direction = CompassDirection.East;

            switch (rotationBy90Count)
            {
                case 1:
                    direction = CompassDirection.South;
                    break;

                case 2:
                    direction = CompassDirection.West;
                    break;

                case 3:
                    direction = CompassDirection.North;
                    break;

                default:
                    direction = CompassDirection.East;
                    break;
            }

            return direction;
        }
        else if (compassDirectionToRotate == CompassDirection.South)
        {
            CompassDirection direction = CompassDirection.South;

            switch (rotationBy90Count)
            {
                case 1:
                    direction = CompassDirection.West;
                    break;

                case 2:
                    direction = CompassDirection.North;
                    break;

                case 3:
                    direction = CompassDirection.East;
                    break;

                default:
                    direction = CompassDirection.South;
                    break;
            }

            return direction;
        }
        else if (compassDirectionToRotate == CompassDirection.West)
        {
            CompassDirection direction = CompassDirection.West;

            switch (rotationBy90Count)
            {
                case 1:
                    direction = CompassDirection.North;
                    break;

                case 2:
                    direction = CompassDirection.East;
                    break;

                case 3:
                    direction = CompassDirection.South;
                    break;

                default:
                    direction = CompassDirection.West;
                    break;
            }

            return direction;
        }


        // we failed :c
        return CompassDirection.North;
    }

    public static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 pivot, float angleInRads)
    {
        Vector2 point = pointToRotate - pivot;


        float x = point.x * Mathf.Cos(angleInRads) - point.y * Mathf.Sin(angleInRads);
        float y = point.x * Mathf.Sin(angleInRads) + point.y * Mathf.Cos(angleInRads);

        return new Vector2(x, y) + pivot;
    }

    public static float GetMagnitudeOfVector2Int(Vector2Int vector)
    {
        return Mathf.Sqrt(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2));
    }
}
