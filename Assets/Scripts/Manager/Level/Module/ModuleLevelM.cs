using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// All the module tiers.
/// </summary>
public enum ModuleTier
{
    Common,
    Rare,
    Epic,
}

/// <summary>
/// Stores the modules in a temporary inventory. This is also used to spawn in modules.
/// </summary>
public class ModuleLevelM : MonoBehaviour
{
    /// <summary>
    /// Singleton for the module manager.
    /// </summary>
    public static ModuleLevelM Instance { get; private set; }

    // ? could replace with dictionary with enum + int, and have it auto init, this would allow for more expansion without too much recode.
    // That could in theory be automated.
    private int currentT1Modules = 0;
    private int currentT2Modules = 0;
    private int currentT3Modules = 0;

    [SerializeField]
    private GameObject commonModulePrefab;
    [SerializeField]
    private GameObject rareModulePrefab;
    [SerializeField]
    private GameObject epicModulePrefab;

    public event Action<ModuleTier, int> OnModuleCollected;
    public event Action<ModuleTier, int> OnModuleDeposited;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogError($"Two or more {nameof(ModuleLevelM)} exists, this one was removed! Make sure only one exists at all times.", this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Add 1 to the currently stored modules in the dungeon / temporary inventory.
    /// </summary>
    /// <param name="moduleTier">The tier of card to collect.</param>
    public void CollectModule(ModuleTier moduleTier)
    {
        switch (moduleTier)
        {
            case ModuleTier.Common:
                currentT1Modules++;
                break;
            case ModuleTier.Rare:
                currentT2Modules++;
                break;
            case ModuleTier.Epic:
                currentT3Modules++;
                break;
        }

        // TODO: The modules are stored in inventory, then deposited. Please fully implement this future self.
        // What? huh?
        OnModuleCollected?.Invoke(moduleTier, 1);
        OnModuleDeposited?.Invoke(moduleTier, 1);

    }

    /// <summary>
    /// Get the current count of the module tier.
    /// </summary>
    /// <param name="moduleTier">The tier to get the quantity for.</param>
    /// <returns>The amount of modules with that tier.</returns>
    public int GetAllModuleCountOfType(ModuleTier moduleTier)
    {
        switch (moduleTier)
        {
            case ModuleTier.Common:
                return currentT1Modules;
            case ModuleTier.Rare:
                return currentT2Modules;
            case ModuleTier.Epic:
                return currentT3Modules;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Get the prefab for the specified tier of card.
    /// </summary>
    /// <param name="moduleTier">The tier of card to get the prefab for.</param>
    /// <returns>The module card prefab with that tier.</returns>
    public GameObject GetModulePrefab(ModuleTier moduleTier)
    {
        switch (moduleTier)
        {
            case ModuleTier.Common:
                return commonModulePrefab;
            case ModuleTier.Rare:
                return rareModulePrefab;
            case ModuleTier.Epic:
                return epicModulePrefab;
            default:
                return null;
        }
    }
}

