using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelizeFeature : ScriptableRendererFeature
{
	//THIS CODE IS FROM: https://www.youtube.com/watch?v=-8xlPP4qgVo

	[System.Serializable]
	public class CustomPassSetings
	{
		public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
		public int screenHeight = 144;
	}

	[SerializeField] private CustomPassSetings settings;
	private PixelizePass customPass;

	public override void Create()
	{
		customPass = new PixelizePass(settings);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
#if UNITY_EDITOR
		if (renderingData.cameraData.isSceneViewCamera) return;
#endif
		renderer.EnqueuePass(customPass);
	}
}
