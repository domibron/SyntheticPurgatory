using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SetRandomMaterial : MonoBehaviour
{

    [SerializeField]
    private Material[] materials;
    List<Material> selectedMaterial = new List<Material>();

    [SerializeField]
    private MeshRenderer[] objectsToChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int rando = Random.Range(0, materials.Length);
        selectedMaterial.Add(materials[rando]);

        foreach (MeshRenderer renderer in objectsToChange)
        {
            renderer.SetMaterials(selectedMaterial);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
