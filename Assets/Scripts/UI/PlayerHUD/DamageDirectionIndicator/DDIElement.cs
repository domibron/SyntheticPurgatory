using UnityEngine;

public class DDIElement : MonoBehaviour
{
    private Vector3 direction;

    private Transform playerCamTransform;

    [SerializeField]
    private RectTransform rectTransform;

    void Update()
    {
        if (playerCamTransform == null) return;


        float angle = Vector3.Angle(direction, playerCamTransform.forward.normalized);
        float dot = Vector3.Dot(direction, playerCamTransform.right.normalized);

        rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, (dot > 0 ? -angle : angle)));
    }

    public void SetDirection(Transform playerCamTransform, Vector3 direction)
    {
        this.playerCamTransform = playerCamTransform;
        this.direction = direction;
    }
}
