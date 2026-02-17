using UnityEngine;

public class DamageDirectionIndicator : MonoBehaviour
{
    public static DamageDirectionIndicator Instance { get; private set; }

    [SerializeField]
    GameObject damageIndicatorPrefab;

    [SerializeField]
    Transform parentForIndicators;

    private Transform playerTransform;

    private Transform playerCamMain;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerTransform == null)
        {
            if (PlayerRefFetcher.Instance != null)
            {
                playerTransform = PlayerRefFetcher.Instance.transform;
                playerCamMain = Camera.main.transform;
            }
            else
            {
                Debug.LogError("Player Ref is NULL!");
                return;
            }
        }
    }

    public void CreateDamageDirectionIndicator(Vector3 damagePos)
    {
        DDIElement ddiElement = Instantiate(damageIndicatorPrefab, parentForIndicators).GetComponent<DDIElement>();
        ddiElement.SetDirection(playerCamMain, (new Vector3(damagePos.x, 0, damagePos.z) - new Vector3(playerTransform.position.x, 0, playerTransform.position.z)).normalized);
    }
}
