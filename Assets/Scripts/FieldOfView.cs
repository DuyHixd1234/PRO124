using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [Header("Tầm nhìn")]
    public float viewRadius = 6f;
    [Range(0, 360)] public float viewAngle = 360f;

    [Header("Số tia cast")]
    public int rayCount = 100;

    [Header("Tường (Obstacle)")]
    public LayerMask obstacleMask;

    [Header("Sorting Layer (MeshRenderer)")]
    public string sortingLayerName = "FOV";
    public int sortingOrder = 0;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private Vector3 origin;
    private float startingAngle;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("[FOV] Không tìm thấy MeshFilter!");
            return;
        }

        mesh = new Mesh();
        mesh.name = "FOV Mesh";
        meshFilter.mesh = mesh;

        // Thiết lập Sorting Layer cho MeshRenderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerName = sortingLayerName;
            meshRenderer.sortingOrder = sortingOrder;
            Debug.Log($"[FOV] Gán Sorting Layer: {sortingLayerName} | Order: {sortingOrder}");
        }
        else
        {
            Debug.LogWarning("[FOV] Không tìm thấy MeshRenderer để gán sorting layer");
        }

        Debug.Log("[FOV] Đã tạo mesh mới và gán cho MeshFilter");
    }

    void LateUpdate()
    {
        origin = transform.position;
        startingAngle = transform.eulerAngles.z - viewAngle / 2f;
        float angleIncrease = viewAngle / rayCount;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero); // Điểm gốc, sẽ transform sau

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startingAngle + angleIncrease * i;
            Vector3 dir = DirFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewRadius, obstacleMask);

            Vector3 vertex;
            if (hit)
            {
                vertex = transform.InverseTransformPoint(hit.point); // chuyển sang local space
            }
            else
            {
                vertex = dir * viewRadius;
            }

            vertices.Add(vertex);

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Debug.DrawRay(origin, DirFromAngle(0) * viewRadius, Color.red);
        Debug.DrawRay(origin, DirFromAngle(90) * viewRadius, Color.green);
        Debug.DrawRay(origin, DirFromAngle(180) * viewRadius, Color.blue);
        Debug.DrawRay(origin, DirFromAngle(270) * viewRadius, Color.yellow);

      
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
