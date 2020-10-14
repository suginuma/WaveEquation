using System.Collections.Generic;
using UnityEngine;

public class WaveSurfaceRenderer : MonoBehaviour
{
    public Material wave_surface_material_;
    public Material wave_equation_material_;

    private const int X_NUM = 128;
    private const int Y_NUM = 128;
    private const int VERTICES_NUM = X_NUM * Y_NUM;
    private const int RECT_NUM = (X_NUM - 1) * (Y_NUM - 1);
    private const int TRIS_NUM = RECT_NUM * 2 * 3;
    private const float SCALE = 100f;

    private Vector3[] vertices_list_;
    private Mesh mesh_;
    private MeshFilter mf_;

    private WaveEquation wave_equation_;
    private WaveInputDrawer wave_input_drawer_;

    private class MaterialInfo
    {
        public static readonly int RScale = Shader.PropertyToID("_RScale");
        public static readonly int Stride = Shader.PropertyToID("_Stride");
    }

    private bool line_render_ = false;

    void Start()
    {
        vertices_list_ = new Vector3[VERTICES_NUM];
        float prev_y = 0f;
        for (var y = 0; y < Y_NUM; y++)
        {
            float last_y = 0f;
            for (var x = 0; x < X_NUM; x++)
            {
                last_y = create_vertex(x, y, ref vertices_list_[x + y * X_NUM], prev_y, true);
            }
            prev_y = last_y;
        }
        var triangles = new int[TRIS_NUM];
        {
            var i = 0;
            for (var y = 0; y < Y_NUM - 1; y++)
            {
                for (var x = 0; x < X_NUM - 1; x++)
                {
                    triangles[i] = (y + 0) * (X_NUM) + x + 0; i++;
                    triangles[i] = (y + 1) * (X_NUM) + x + 0; i++;
                    triangles[i] = (y + 0) * (X_NUM) + x + 1; i++;
                    triangles[i] = (y + 1) * (X_NUM) + x + 0; i++;
                    triangles[i] = (y + 1) * (X_NUM) + x + 1; i++;
                    triangles[i] = (y + 0) * (X_NUM) + x + 1; i++;
                }
            }
        }
        var uvs_list = new Vector2[VERTICES_NUM];
        for (var y = 0; y < Y_NUM; y++)
        {
            for (var x = 0; x < X_NUM; x++)
            {
                uvs_list[x + y * X_NUM].x = 1.0f / (X_NUM - 1) * x;
                uvs_list[x + y * X_NUM].y = 1.0f / (Y_NUM - 1) * y;
            }
        }

        mesh_ = new Mesh();
        mesh_.name = "WaveSurface";
        mesh_.vertices = vertices_list_;
        mesh_.uv = uvs_list;
        mesh_.triangles = triangles;

        if (line_render_)
        {
            int[] indices = new int[2 * triangles.Length];
            int i = 0;
            for (int t = 0; t < triangles.Length; t += 3)
            {
                indices[i++] = triangles[t + 0];
                indices[i++] = triangles[t + 1];
                indices[i++] = triangles[t + 1];
                indices[i++] = triangles[t + 2];
                indices[i++] = triangles[t + 2];
                indices[i++] = triangles[t + 0];
            }
            mesh_.SetIndices(indices, MeshTopology.Lines, 0);
        }
        mesh_.RecalculateBounds();

        mf_ = GetComponent<MeshFilter>();
        mf_.sharedMesh = mesh_;
        var mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = wave_surface_material_;

        var mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh_;

        wave_equation_ = new WaveEquation();
        wave_equation_.init(512, RenderTextureFormat.R8, false);
        wave_input_drawer_ = GameObject.Find("WaveInput").GetComponent<WaveInputDrawer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Click();
            //wave_input_drawer_.putPoint(0.0f, 0.0f, -0.5f, 0.1f);
        }
        wave_equation_.render(wave_equation_material_, wave_input_drawer_.getRenderTexture());
        wave_equation_.bind(wave_surface_material_);

        var mat = wave_surface_material_;
        var scale_ = SCALE;
        mat.SetVector(MaterialInfo.RScale, new Vector2(1f / (scale_), 1f / (scale_)));
        var stride = 1f / wave_equation_.getRenderTextureWidth();
        mat.SetVector(MaterialInfo.Stride, new Vector2(stride, stride));
    }

    void Click()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.point);
            wave_input_drawer_.putPoint(hit.point.x, hit.point.z, -0.5f, 1f);
        }
    }

    private float create_vertex(int xi, int yi, ref Vector3 vertex, float prev_y, bool distortion)
    {
        float new_y = 0f;
        float x = (float)xi;
        float y = (float)yi;
        if (distortion)
        {

        }
        vertex.x = x * (SCALE / (X_NUM - 1)) - SCALE * 0.5f;
        vertex.y = 0f;
        vertex.z = y * (SCALE / (Y_NUM - 1)) - SCALE * 0.5f;

        return new_y;
    }
}
