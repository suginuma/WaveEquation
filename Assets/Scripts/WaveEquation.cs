using UnityEngine;

public class WaveEquation
{
    private RenderTexture[] render_texture_list_;
    private int front_;
    private int latest_;
    private int width_;

    static readonly int material_PrevTex = Shader.PropertyToID("_PrevTex");
    static readonly int material_PrevPrevTex = Shader.PropertyToID("_PrevPrevTex");
    static readonly int material_Stride = Shader.PropertyToID("_Stride");
    static readonly int material_InputTex = Shader.PropertyToID("_InputTex");

    public void init(int width, RenderTextureFormat format, bool clamp)
    {
        if (!SystemInfo.SupportsRenderTextureFormat(format))
        {
            format = RenderTextureFormat.ARGB32;
        }

        width_ = width;
        render_texture_list_ = new RenderTexture[3];
        for (var i = 0; i < render_texture_list_.Length; i++)
        {
            var rt = new RenderTexture(width_, width_, 0, format);
            Debug.Assert(rt != null);
            rt.wrapMode = clamp ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
            rt.Create();
            render_texture_list_[i] = rt;
            Graphics.SetRenderTarget(rt);
            GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
        front_ = 0;
        latest_ = 0;
    }

    public void render(Material material, Texture input_texture)
    {
        int prev = (front_ + 2) % 3;
        int prevprev = (front_ + 1) % 3;
        material.SetTexture(material_PrevTex, render_texture_list_[prev]);
        material.SetTexture(material_PrevPrevTex, render_texture_list_[prevprev]);
        material.SetVector(material_Stride, new Vector2(1f / (float)width_, 1f / (float)width_));
        material.SetTexture(material_InputTex, input_texture);
        Graphics.SetRenderTarget(render_texture_list_[front_]);
        GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        Graphics.Blit(render_texture_list_[prev], render_texture_list_[front_], material);
        latest_ = front_;
        front_ = prevprev;
    }

    public RenderTexture getLatestRenderTexture()
    {
        return render_texture_list_[latest_];
    }

    public void setTexture(Texture texture)
    {
        int prevprev = (front_ + 1) % 3;
        Graphics.SetRenderTarget(render_texture_list_[prevprev]);
        GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        Graphics.Blit(texture, render_texture_list_[prevprev]);
    }
}
