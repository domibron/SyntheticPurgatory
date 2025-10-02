using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

#region GradientDirection
public enum GradientDirection
{
    Vertical,
    Horizontal,
    DiagonalLeft,
    DiagonalRight,
}
#endregion


#region FloatingTextSystem class
/// <summary>
/// Spawns numbers around the entity to display the damage it took.
/// </summary>
public class FloatingTextSystem : MonoBehaviour
{
    #region Variables
    public GameObject magicNumberPrefab;

    public float MinDistance = 1.2f;
    public float MaxDistance = 1.5f;

    public float MaxRotationOffsetHorizontal = 120f;
    public float MaxRotationOffsetVertical = 60f;

    private Transform cameraTransform;

    #endregion



    #region Start
    void Start()
    {
        cameraTransform = Camera.main.transform;
        //StartCoroutine(spawnNum());
    }
    #endregion



    #region spawnNum
    /// <summary>
    /// Used for debugging.
    /// </summary>
    /// <returns>Coroutine.</returns>
    IEnumerator spawnNum()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.1f);
            //SpawnTwoToneText("test", Color.cyan, Color.magenta);
        }
    }
    #endregion



    #region SpawnText
    /// <summary>
    /// Spawns a magic floating number.
    /// </summary>
    /// <param name="text">The message or number to display.</param>
    /// <param name="textColor">The colour for the text.</param>
    /// <param name="textSize">How large the text should be.</param>
    /// <param name="characterSpacing">Spacing between each character</param>
    public void SpawnText(string text, Color textColor, float textSize = 10f, float characterSpacing = -30)
    {
        Vector3 directionTowardsCamera = (cameraTransform.position - transform.position).normalized;
        directionTowardsCamera.y = 0f;

        Vector3 horizontalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetHorizontal, MaxRotationOffsetHorizontal), transform.up) * directionTowardsCamera;
        Vector3 verticalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetVertical, MaxRotationOffsetVertical), transform.right) * directionTowardsCamera;

        Vector3 targetSpawnPoint = transform.position + (horizontalDisplacement + verticalDisplacement) * UnityEngine.Random.Range(MinDistance, MaxDistance);

        GameObject spawnedNumber = Instantiate(magicNumberPrefab, targetSpawnPoint, Quaternion.identity);

        // then we can do cool UI stuff.
        TMP_Text textComp = spawnedNumber.GetComponent<TMP_Text>();

        textComp.text = text;

        textComp.color = textColor;
        textComp.enableVertexGradient = false;

        textComp.fontSize = textSize;
        textComp.characterSpacing = characterSpacing;
    }
    #endregion

    #region SpawnText
    /// <summary>
    /// Spawns a magic floating number.
    /// </summary>
    /// <param name="text">The message or number to display.</param>
    /// <param name="textColorGradient">The colour for the text.</param>
    /// <param name="textSize">How large the text should be.</param>
    /// <param name="characterSpacing">Spacing between each character</param>
    public void SpawnText(string text, TMP_ColorGradient textColorGradient, float textSize = 10f, float characterSpacing = -30)
    {
        Vector3 directionTowardsCamera = (cameraTransform.position - transform.position).normalized;
        directionTowardsCamera.y = 0f;

        Vector3 horizontalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetHorizontal, MaxRotationOffsetHorizontal), transform.up) * directionTowardsCamera;
        Vector3 verticalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetVertical, MaxRotationOffsetVertical), transform.right) * directionTowardsCamera;

        Vector3 targetSpawnPoint = transform.position + (horizontalDisplacement + verticalDisplacement) * UnityEngine.Random.Range(MinDistance, MaxDistance);

        GameObject spawnedNumber = Instantiate(magicNumberPrefab, targetSpawnPoint, Quaternion.identity);

        // then we can do cool UI stuff.
        TMP_Text textComp = spawnedNumber.GetComponent<TMP_Text>();

        textComp.text = text;

        textComp.color = Color.white;
        textComp.enableVertexGradient = true;
        textComp.colorGradientPreset = textColorGradient;

        textComp.fontSize = textSize;
        textComp.characterSpacing = characterSpacing;
    }
    #endregion


    #region SpawnTwoToneText
    /// <summary>
    /// Spawns floating text with the two gradient colors.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="color1">The first color in the two tone color.</param>
    /// <param name="color2">The second color in the two tone color.</param>
    /// <param name="gradientDirection">The direction of the gradient.</param>
    /// <param name="textSize">The size of the text.</param>
    /// <param name="characterSpacing">Spacing between each character</param>
    [Obsolete("Please use SpawnText functions", false)]
    public void SpawnTwoToneText(string text, Color color1, Color color2, GradientDirection gradientDirection = GradientDirection.Vertical, float textSize = 10f, float characterSpacing = -30)
    {
        Vector3 directionTowardsCamera = (cameraTransform.position - transform.position).normalized;
        directionTowardsCamera.y = 0f;

        Vector3 horizontalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetHorizontal, MaxRotationOffsetHorizontal), transform.up) * directionTowardsCamera;
        Vector3 verticalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetVertical, MaxRotationOffsetVertical), transform.right) * directionTowardsCamera;

        Vector3 targetSpawnPoint = transform.position + (horizontalDisplacement + verticalDisplacement) * UnityEngine.Random.Range(MinDistance, MaxDistance);

        GameObject spawnedNumber = Instantiate(magicNumberPrefab, targetSpawnPoint, Quaternion.identity);

        // then we can do cool UI stuff.
        TMP_Text textComp = spawnedNumber.GetComponent<TMP_Text>();

        textComp.text = text;

        textComp.color = Color.white;

        textComp.enableVertexGradient = true;

        switch (gradientDirection)
        {
            case GradientDirection.Vertical:
                textComp.colorGradientPreset = CreateColorGradient(color1, color1, color2, color2);
                break;
            case GradientDirection.Horizontal:
                textComp.colorGradientPreset = CreateColorGradient(color1, color2, color1, color2);
                break;
            case GradientDirection.DiagonalLeft:
                textComp.colorGradientPreset = CreateColorGradient(color1, color2, color2, color1);
                break;
            case GradientDirection.DiagonalRight:
                textComp.colorGradientPreset = CreateColorGradient(color2, color1, color1, color2);
                break;
        }

        textComp.fontSize = textSize;
        textComp.characterSpacing = characterSpacing;
    }
    #endregion


    #region SpawnFourToneText
    /// <summary>
    /// Spawns the floating text with the four tone color gradient.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="topLeft">The top left vertex color.</param>
    /// <param name="topRight">The top right vertex color.</param>
    /// <param name="bottomLeft">The bottom left vertex color.</param>
    /// <param name="bottomRight">The bottom right vertex color.</param>
    /// <param name="textSize">The size of the text.</param>
    /// <param name="characterSpacing">Spacing between each character</param>
    [Obsolete("Please use SpawnText functions", false)]
    public void SpawnFourToneText(string text, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight, float textSize = 10f, float characterSpacing = -30)
    {
        Vector3 directionTowardsCamera = (cameraTransform.position - transform.position).normalized;
        directionTowardsCamera.y = 0f;

        Vector3 horizontalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetHorizontal, MaxRotationOffsetHorizontal), transform.up) * directionTowardsCamera;
        Vector3 verticalDisplacement = Quaternion.AngleAxis(UnityEngine.Random.Range(-MaxRotationOffsetVertical, MaxRotationOffsetVertical), transform.right) * directionTowardsCamera;

        Vector3 targetSpawnPoint = transform.position + (horizontalDisplacement + verticalDisplacement) * UnityEngine.Random.Range(MinDistance, MaxDistance);

        GameObject spawnedNumber = Instantiate(magicNumberPrefab, targetSpawnPoint, Quaternion.identity);

        // then we can do cool UI stuff.
        TMP_Text textComp = spawnedNumber.GetComponent<TMP_Text>();

        textComp.text = text;

        textComp.color = Color.white;

        textComp.enableVertexGradient = true;

        textComp.colorGradientPreset = CreateColorGradient(topLeft, topRight, bottomLeft, bottomRight);

        textComp.fontSize = textSize;
        textComp.characterSpacing = characterSpacing;
    }
    #endregion



    #region CreateColorGradient
    /// <summary>
    /// Creates a gradient asset to be shoved into the TMP text asset.
    /// </summary>
    /// <param name="topLeft">Top left axis color.</param>
    /// <param name="topRight">Top right axis color.</param>
    /// <param name="bottomLeft">Bottom left axis color.</param>
    /// <param name="bottomRight">Bottom right axis color.</param>
    /// <returns>The color gradient asset. Shove that into the TMP text asset.</returns>
    private TMP_ColorGradient CreateColorGradient(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
    {
        TMP_ColorGradient gradient = new TMP_ColorGradient();

        gradient.colorMode = ColorMode.FourCornersGradient;

        gradient.topLeft = topLeft;
        gradient.topRight = topRight;
        gradient.bottomLeft = bottomLeft;
        gradient.bottomRight = bottomRight;

        return gradient;
    }
    #endregion
}
#endregion
