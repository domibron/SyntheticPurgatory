using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoomDataDisplay : MonoBehaviour
{
    public SpawnedLevelRoomData LevelRoomData = null;

    public void SetUpRoom(SpawnedLevelRoomData levelRoomData)
    {
        LevelRoomData = levelRoomData;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if (LevelRoomData == null) return;

        float halfOfUnitPerMeters = LevelRoomData.GetUnitSizeInMeters() / 2f;


        Vector3 CentrePos = transform.position + (Vector3.up * halfOfUnitPerMeters) + (LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize) * halfOfUnitPerMeters) - (LevelRoomData.GetSpawnPoint() - (LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.GridCoordinates) * LevelRoomData.GetUnitSizeInMeters()));
        //transform.position - (LevelRoomData.SpawnPointOffset * halfOfUnitPerMeters) - (LevelRoomData.GetOffsetBasedOnDirection() * LevelRoomData.GetUnitSizeInMeters()) + (LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize) / 2f) * LevelRoomData.GetUnitSizeInMeters() + ((Vector3.up * LevelRoomData.GetUnitSizeInMeters()) / 2f);

        foreach (var door in LevelRoomData.DoorwayData)
        {
            Vector3 facingDirection = LevelGenerationUtil.ConvertVector2IntToVector3(LevelGenerationUtil.GetCompassDirectionAsVector2Int(door.FacingDirection));
            // TODO FIx this shit, as its broken since the converting to actual level pieces.
            Vector3 doorLocation = (LevelGenerationUtil.ConvertVector2IntToVector3(door.Location) + LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize - Vector2Int.one)) * LevelRoomData.GetUnitSizeInMeters();

            Vector2 reverseRotation = LevelGenerationUtil.RotatePoint(new Vector2(door.Location.x, door.Location.y), (new Vector2(LevelRoomData.BoundingSize.x, LevelRoomData.BoundingSize.y) - Vector2.one) / 2f, LevelRoomData.OrientationInDegrees * Mathf.Deg2Rad);
            //Vector3 doorPosOffset = new Vector3(reverseRotation.x, 0, reverseRotation.y) * LevelRoomData.GetUnitSizeInMeters();


            Vector3 doorStartPos = transform.position + (transform.up * LevelRoomData.GetUnitSizeInMeters()) + (transform.forward * reverseRotation.y + transform.right * reverseRotation.x) * LevelRoomData.GetUnitSizeInMeters() + ((transform.forward + transform.right) * halfOfUnitPerMeters);

            // Vector3 startLocation = doorStartPos;
            Vector3 endLocation = doorStartPos + (facingDirection * halfOfUnitPerMeters);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(doorStartPos, endLocation);
        }

        Gizmos.color = Color.blue;



        Gizmos.DrawWireCube(CentrePos,
        (LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize) * LevelRoomData.GetUnitSizeInMeters()) + (Vector3.up * LevelRoomData.GetUnitSizeInMeters()));

        // Gizmos.color = Color.red;
        // Gizmos.DrawCube((transform.position - LevelRoomData.SpawnPointOffset - (Vector3.one / 2f)) + (Vector3.up / 2f) + (LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize) / 2f),
        // LevelGenerationUtil.ConvertVector2IntToVector3(LevelRoomData.BoundingSize) + Vector3.up);
    }
}
