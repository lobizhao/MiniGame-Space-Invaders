using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SimpleBloomEffect : MonoBehaviour
{
    [Header("Bloom Settings")]
    [Range(0.0f, 1.5f)]
    public float threshold = 0.8f; // 亮度超过多少才开始发光

    [Range(0.0f, 5.0f)]
    public float intensity = 1.5f; // 光晕的强度

    [Range(1, 8)]
    public int blurIterations = 3; // 模糊迭代次数（越高越柔和，但性能越差）

    [Range(1, 4)]
    public int downSample = 2;     // 降采样（提高性能并增加模糊范围）

    private Material bloomMaterial;

    // 查找并创建所需的材质
    Material material
    {
        get
        {
            if (bloomMaterial == null)
            {
                // 找到我们刚才创建的隐藏 Shader
                bloomMaterial = new Material(Shader.Find("Hidden/SimpleBloom"));
                bloomMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return bloomMaterial;
        }
    }

    // Unity 后期处理的核心函数
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // 1. 准备降采样后的临时渲染纹理
        int width = source.width / downSample;
        int height = source.height / downSample;
        RenderTextureFormat format = source.format;

        RenderTexture buffer0 = RenderTexture.GetTemporary(width, height, 0, format);
        RenderTexture buffer1 = RenderTexture.GetTemporary(width, height, 0, format);

        // 2. 提取高亮区域 (Pass 0)
        material.SetFloat("_Threshold", threshold);
        Graphics.Blit(source, buffer0, material, 0);

        // 3. 迭代模糊 (Pass 1)
        // 通过在两个 buffer 之间来回 Blit 进行多次模糊
        for (int i = 0; i < blurIterations; i++)
        {
            Graphics.Blit(buffer0, buffer1, material, 1); // Blur horizontal directionish
            Graphics.Blit(buffer1, buffer0, material, 1); // Blur vertical directionish
        }

        // 4. 合并最终图像 (Pass 2)
        material.SetFloat("_Intensity", intensity);
        // 将模糊好的 buffer0 设置为全局纹理，供 Shader 的 Pass 2 使用
        Shader.SetGlobalTexture("_BloomTex", buffer0); 
        // 将原始图像 (source) 和模糊图像合并，输出到屏幕 (destination)
        Graphics.Blit(source, destination, material, 2);

        // 释放临时纹理内存
        RenderTexture.ReleaseTemporary(buffer0);
        RenderTexture.ReleaseTemporary(buffer1);
    }
}