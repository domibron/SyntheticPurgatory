using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FadeDecal : MonoBehaviour
{
    [SerializeField]
    private DecalProjector decalProjector;

    [SerializeField]
    private float timeToFade = 10;

    [SerializeField]
    private float updatesPerSecond = 8;

    private float originalOpacity;

    void Start()
    {
        originalOpacity = decalProjector.fadeFactor;
        StartCoroutine("FadeOverTime");
    }

    IEnumerator FadeOverTime()
    {
        yield return new WaitForSeconds(0.1f);

        float fadeAmount = 1;

        while (fadeAmount > 0)
        {
            fadeAmount -= (1 / timeToFade / updatesPerSecond);
            decalProjector.fadeFactor = originalOpacity * fadeAmount;
            yield return new WaitForSeconds(1 / updatesPerSecond);

        }

        Destroy(this.gameObject);
    }
}
