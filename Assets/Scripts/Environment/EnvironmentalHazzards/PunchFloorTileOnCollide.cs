using UnityEngine;

/// <summary>
/// Add this to physics object so when they hit <see cref="DroppablePlatform"/>, it will cause them to fall.
/// </summary>
public class PunchFloorTileOnCollide : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constants.CollapsibleFloorTag))
        {
            collision.transform.parent.gameObject.GetComponent<DroppablePlatform>().Drop();
        }
    }
}
