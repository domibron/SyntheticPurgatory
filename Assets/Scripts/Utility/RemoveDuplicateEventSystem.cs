using UnityEngine;
using UnityEngine.EventSystems;


public class RemoveDuplicateEventSystem : MonoBehaviour
{
    void Awake()
    {
        if (GetComponent<EventSystem>() == null) Debug.LogError("HEY! I need to be attached to event systems!");


        if (EventSystem.current != null)
        {
            if (EventSystem.current != GetComponent<EventSystem>())
            {
                Debug.Log("Removed duplicute event systems.");
                Destroy(gameObject);
            }
        }
    }
}
