using System.Collections.Generic;
using UnityEngine;

public class WaveInputDrawer : MonoBehaviour
{
    public Material wave_input_material_;

    struct InputInfo
    {
        public enum Type
        {
            Circle,
        }
        public Type type_;
        public float x_;
        public float y_;
        public float value_;
        public float size_;
        public InputInfo(Type type, float x, float y, float value, float size)
        {
            type_ = type;
            x_ = x;
            y_ = y;
            value_ = value;
            size_ = size;
        }
    }
    private List<InputInfo> input_info_list_;

    private Mesh mesh_;
    private List<Vector3> vertices_;
    private List<int> indices_;
    private List<Vector3> empty_vertices_;
    private List<int> empty_indices_;
    private RenderTexture render_texture_;
    private Camera camera_;

    private const float SCALE = 100f;
    private const float SCALE_HALF = SCALE * 0.5f;
    private const int WAVE_INPUT_LAYER = 8;
    private const int VERTICES_MAX = 256;

    public RenderTexture getRenderTexture()
    {
        return render_texture_;
    }

    public void putPoint(float x, float y, float value, float size)
    {
        if (input_info_list_.Count >= input_info_list_.Capacity)
        {
            Debug.LogError("exceed wave input buffer");
            return;
        }
        input_info_list_.Add(new InputInfo(InputInfo.Type.Circle,
                                           Mathf.Repeat(x + SCALE_HALF, SCALE) - SCALE_HALF,
                                           Mathf.Repeat(y + SCALE_HALF, SCALE) - SCALE_HALF,
                                           value,
                                           size));
    }

    void Start()
    {
        input_info_list_ = new List<InputInfo>();
        input_info_list_.Capacity = VERTICES_MAX / 4;

        mesh_ = new Mesh();
        mesh_.MarkDynamic();
        mesh_.name = "input_draw";
        mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
        vertices_ = new List<Vector3>();
        vertices_.Capacity = VERTICES_MAX;
        indices_ = new List<int>();
        indices_.Capacity = VERTICES_MAX;
        empty_vertices_ = new List<Vector3>();
        empty_indices_ = new List<int>();

        render_texture_ = new RenderTexture(512, 512, 0, RenderTextureFormat.R8);
        render_texture_.name = "wave_input";
        render_texture_.filterMode = FilterMode.Bilinear;
        render_texture_.autoGenerateMips = false;
        render_texture_.wrapMode = TextureWrapMode.Repeat;
        render_texture_.anisoLevel = 0;
        render_texture_.Create();
        Graphics.SetRenderTarget(render_texture_);
        GL.Clear(false, true, new Color(0.5f, 0.5f, 0.5f, 0.5f));

        var camera_go = new GameObject();
        camera_go.name = "wave_input";
        camera_ = camera_go.AddComponent<Camera>();
        camera_.useOcclusionCulling = false;
        camera_.cullingMask = 1 << WAVE_INPUT_LAYER;
        camera_.orthographic = true;
        camera_.orthographicSize = SCALE_HALF;
        camera_.nearClipPlane = -1f;
        camera_.farClipPlane = 1f;
        camera_.clearFlags = CameraClearFlags.SolidColor;
        camera_.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        camera_.targetTexture = render_texture_;
    }

    void Update()
    {
        foreach(var input_info in input_info_list_)
        {
            var posx = input_info.x_;
            var posy = input_info.y_;
            var value = input_info.value_;
            var size = input_info.size_;
            vertices_.Add(new Vector3(posx - size, posy - size, value));
            vertices_.Add(new Vector3(posx + size, posy - size, value));
            vertices_.Add(new Vector3(posx - size, posy + size, value));
            vertices_.Add(new Vector3(posx + size, posy + size, value));
            int idx = vertices_.Count - 4;
            indices_.Add(idx + 0);
            indices_.Add(idx + 1);
            indices_.Add(idx + 2);
            indices_.Add(idx + 2);
            indices_.Add(idx + 1);
            indices_.Add(idx + 3);
        }
        input_info_list_.Clear();

        mesh_.SetTriangles(empty_indices_, 0, false);
        mesh_.SetVertices(empty_vertices_);
        mesh_.SetVertices(vertices_);
        mesh_.SetTriangles(indices_, 0, false);
        Graphics.DrawMesh(mesh_,
                          Vector3.zero,
                          Quaternion.identity,
                          wave_input_material_,
                          WAVE_INPUT_LAYER,
                          camera_,
                          0,
                          null,
                          false,
                          false,
                          false);
        vertices_.Clear();
        indices_.Clear();
    }
}
