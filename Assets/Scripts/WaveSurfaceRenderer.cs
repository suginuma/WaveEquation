using System.Collections.Generic;
using UnityEngine;

public class WaveSurfaceRenderer : MonoBehaviour
{
    public Material wave_equation_material_;

    private WaveEquation wave_equation_;
    private WaveInputDrawer wave_input_drawer_;

    void Start()
    {
        wave_equation_ = new WaveEquation();
        wave_equation_.init(512, RenderTextureFormat.R8, false);
        wave_input_drawer_ = GameObject.Find("WaveInput").GetComponent<WaveInputDrawer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Click();
        }
        wave_equation_.render(wave_equation_material_, wave_input_drawer_.getRenderTexture());
    }

    void Click()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point);
            wave_input_drawer_.putPoint(hit.point.x * -10, hit.point.z * -10, -0.5f, 0.1f);
        }
    }
}
