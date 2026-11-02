using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class GameViewFilterFSRF : ScriptableRendererFeature
{
    public static GameViewFilterFSRF Instance { private set; get; }

    [SerializeField] private Material m_fullScreenMat;
    private Material m_runtimeMat;

    [SerializeField] private RenderPassEvent m_injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

    private FullscreenPass m_fullscreenPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (Application.isPlaying && !Instance) Instance = this;

        if (renderingData.cameraData.cameraType == CameraType.SceneView) return;

        if (!m_fullScreenMat) return;

        EnsureRuntimeMaterial();

        Material materialToUse =
        Application.isPlaying && m_runtimeMat ? m_runtimeMat : m_fullScreenMat;

        m_fullscreenPass.SetMaterial(materialToUse);
        renderer.EnqueuePass(m_fullscreenPass);
    }

    public override void Create()
    {
        if (Application.isPlaying)
        {
            Instance = this;
        }

        m_fullscreenPass = new FullscreenPass();

        EnsureRuntimeMaterial();

        Material materialToUse =
        Application.isPlaying && m_runtimeMat ? m_runtimeMat : m_fullScreenMat;

        m_fullscreenPass.SetMaterial(materialToUse);
    }

    protected override void Dispose(bool disposing)
    {
        if (Instance) Instance = null;


        if (m_runtimeMat)
        {
            CoreUtils.Destroy(m_runtimeMat);
            m_runtimeMat = null;
        }
    }

    private void EnsureRuntimeMaterial()
    {
        if (Application.isPlaying &&
        m_runtimeMat == null &&
        m_fullScreenMat != null)
        {
            m_runtimeMat = new Material(m_fullScreenMat);
            m_runtimeMat.SetInt("_InEffect", 0);
            m_runtimeMat.SetFloat("_Activation", 1);
            m_runtimeMat.SetFloat("_Intensity", 0);
            m_runtimeMat.SetFloat("_Darken", 0);
        }
    }

    public void SetIntensity(float value = 0)
    {
        EnsureRuntimeMaterial();

        value = Mathf.Clamp01(value);
        m_runtimeMat?.SetFloat("_Intensity", value);
    }

    public void SetEffectActive(bool value = false)
    {
        EnsureRuntimeMaterial();

        m_runtimeMat?.SetFloat("_InEffect", value ? 1 : 0);
    }

    public void SetDarken(float value = 0)
    {
        EnsureRuntimeMaterial();

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

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (!m_mat) return;

            var resource = frameData.Get<UniversalResourceData>();

            TextureHandle source = resource.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "Fullscreen Effect Result";

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            var blitParams = new RenderGraphUtils.BlitMaterialParameters(
            source,
            destination,
            m_mat,
            0
            );

            renderGraph.AddBlitPass(

            blitParams,
            "Full Screen Shader Effect"
            );

            resource.cameraColor = destination;
        }

    }
}
