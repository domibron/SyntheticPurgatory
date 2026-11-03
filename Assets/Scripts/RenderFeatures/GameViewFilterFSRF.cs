using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

/// <summary>
/// Full screen render feature that only targets the game view.
/// </summary>
public class GameViewFilterFSRF : ScriptableRendererFeature
{
    public static GameViewFilterFSRF Instance { private set; get; }

    [SerializeField] private Material m_fullScreenMat;
    private Material m_runtimeMat;

    [SerializeField] private RenderPassEvent m_injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

    private FullscreenPass m_fullscreenPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (Application.isPlaying) Instance = this;

        if (renderingData.cameraData.isSceneViewCamera) return;

        if (!renderingData.cameraData.resolveFinalTarget) return;

        if (!m_fullScreenMat) return;

        EnsureRuntimeMaterial();

        Material materialToUse =
        Application.isPlaying && m_runtimeMat ? m_runtimeMat : m_fullScreenMat;

        m_fullscreenPass.SetMaterial(materialToUse);
        renderer.EnqueuePass(m_fullscreenPass);
    }

    public override void Create()
    {
        m_fullscreenPass = new FullscreenPass
        {
            renderPassEvent = m_injectionPoint
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (Instance == this) Instance = null;


        if (m_runtimeMat != null)
        {
            CoreUtils.Destroy(m_runtimeMat);
            m_runtimeMat = null;
        }

        m_fullscreenPass = null;
    }

    private void EnsureRuntimeMaterial()
    {
        if (!Application.isPlaying) return;

        if (m_runtimeMat != null || m_fullScreenMat == null) return;


        m_runtimeMat = new Material(m_fullScreenMat)
        {
            name = m_fullScreenMat.name + " (Runtime)",
            hideFlags = HideFlags.DontSave
        };

        m_runtimeMat.SetInt("_InEffect", 0);
        m_runtimeMat.SetFloat("_Activation", 1);
        m_runtimeMat.SetFloat("_Intensity", 0);
        m_runtimeMat.SetFloat("_Darken", 0);

    }

    public void SetIntensity(float value = 0)
    {
        EnsureRuntimeMaterial();

        if (m_runtimeMat == null) return;

        value = Mathf.Clamp01(value);
        m_runtimeMat?.SetFloat("_Intensity", value);
    }

    public void SetEffectActive(bool value = false)
    {
        EnsureRuntimeMaterial();

        if (m_runtimeMat == null) return;

        m_runtimeMat?.SetFloat("_InEffect", value ? 1 : 0);
    }

    public void SetDarken(float value = 0)
    {
        EnsureRuntimeMaterial();

        if (m_runtimeMat == null) return;

        value = Mathf.Clamp01(value);
        m_runtimeMat?.SetFloat("_Darken", value);
    }

    public void ResetEffect()
    {
        SetIntensity();
        SetEffectActive();
        SetDarken();
    }

    public void SetFullEffect()
    {
        SetIntensity(1);
        SetEffectActive(true);
        SetDarken();
    }


    private class FullscreenPass : ScriptableRenderPass
    {
        private Material m_mat;

        public void SetMaterial(Material material)
        {
            m_mat = material;
        }


        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            if (!m_mat)
                return;

            var resource = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            // Only process the final camera in the stack.
            if (!cameraData.resolveFinalTarget)
                return;

            TextureHandle source = resource.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "Fullscreen Effect Result";

            TextureHandle destination =
                renderGraph.CreateTexture(destinationDesc);

            // Apply Shader Graph effect.
            var blitParams =
                new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    m_mat,
                    0
                );

            renderGraph.AddBlitPass(
                blitParams,
                "Fullscreen Shader Effect"
            );

            // Copy the processed image back into the original texture.
            renderGraph.AddBlitPass(
                destination,
                source,
                Vector2.one,
                Vector2.zero,
                passName: "Copy Fullscreen Result Back"
            );
        }


    }
}
