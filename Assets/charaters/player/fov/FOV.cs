using UnityEngine;
using Game.Utils;
using System.Collections.Generic;

public class FOV : MonoBehaviour {
    [SerializeField] LayerMask layerMask;
    [SerializeField] string sortingLayer = "Level1";
    [SerializeField] float fov = 300f;
    [SerializeField] float viewDistance = 4f;
    [SerializeField] float startingAngle = 0f;
    [SerializeField] float drawDelay = 0.01f;

    Transform target;
    Mesh mesh;
    readonly int rayCount = 40;
    new Renderer renderer;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        renderer = GetComponent<MeshRenderer>();
        target = transform.parent.transform;
        StartCoroutine(DrawMaskShape());
    }

    IEnumerator<YieldInstruction> DrawMaskShape() {
        while (true) {
            renderer.sortingLayerName = sortingLayer;

            float angle = startingAngle;
            float angleIncrease = fov / rayCount;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = Vector3.zero;

            Sequences.Iterate((1, -3), ((int a, int b) t) => (t.a + 1, t.b + 3))
                .Take(rayCount + 1)
                .ForEach(((int vertexIdx, int triangleIdx) t) => {
                    bool isHit = IsHit(angle, out RaycastHit hit);
                    vertices[t.vertexIdx] = isHit
                        ? target.InverseTransformPoint(hit.point)
                        : vertices[0] + Vectors.ToVector3(angle) * viewDistance;

                    if (t.triangleIdx >= 0) {
                        triangles[t.triangleIdx] = 0;
                        triangles[t.triangleIdx + 1] = t.vertexIdx - 1;
                        triangles[t.triangleIdx + 2] = t.vertexIdx;
                    }

                    angle -= angleIncrease;
                });

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(vertices[0], Vector3.one * 1000f);
            yield return new WaitForSeconds(drawDelay);
        }
    }

    bool IsHit(float angle, out RaycastHit hit) =>
        Physics.Raycast(
            target.position,
            Vectors.ToVector3(angle + target.rotation.eulerAngles.z),
            out hit,
            viewDistance,
            layerMask);
}
