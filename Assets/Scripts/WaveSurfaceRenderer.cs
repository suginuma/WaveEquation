using System.Collections.Generic;
using UnityEngine;

public class WaveSurfaceRenderer : MonoBehaviour
{
    public Material wave_equation_material_;
    public Texture texture_;

    private WaveEquation wave_equation_;

    void Start()
    {
        wave_equation_ = new WaveEquation();
        wave_equation_.init(512, RenderTextureFormat.R8, false);
        wave_equation_.setTexture(texture_);
    }

    void Update()
    {
        wave_equation_.render(wave_equation_material_, wave_equation_.getLatestRenderTexture());
    }
}
