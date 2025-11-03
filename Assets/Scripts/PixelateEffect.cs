using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Range(1, 10)]
        public int pixelSize = 4;
    }

    public Settings settings = new Settings();

    class PixelatePass : ScriptableRenderPass
    {
        private RTHandle tempTexture;
        private int pixelSize;
        private string profilerTag = "PixelatePass";

        public PixelatePass(int pixelSize)
        {
            this.pixelSize = pixelSize;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            int width = Mathf.Max(1, Screen.width / pixelSize);
            int height = Mathf.Max(1, Screen.height / pixelSize);

            
            tempTexture = RTHandles.Alloc(
                width,
                height,
                colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                name: "_TempPixelRT",
                filterMode: FilterMode.Point
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (tempTexture == null)
                return;

            var cmd = CommandBufferPool.Get(profilerTag);

            
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            
            Blit(cmd, source, tempTexture);
            Blit(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempTexture != null)
            {
                RTHandles.Release(tempTexture);
                tempTexture = null;
            }
        }
    }

    PixelatePass pass;

    public override void Create()
    {
        pass = new PixelatePass(settings.pixelSize)
        {
            renderPassEvent = RenderPassEvent.AfterRendering
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
