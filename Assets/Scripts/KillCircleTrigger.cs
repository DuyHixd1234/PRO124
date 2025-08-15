using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class KillCircleTrigger : MonoBehaviour
{
    private CircleCollider2D circleCollider;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true; // Đảm bảo là trigger
    }

    void OnEnable()
    {
        // Khi enable → check ngay lập tức
        KillClosestCrewmate();
    }

    void KillClosestCrewmate()
    {
        // Lấy tất cả collider trong bán kính
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);

        List<GameObject> crewmates = new List<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Crewmate") && hit.gameObject.activeSelf)
            {
                crewmates.Add(hit.gameObject);
            }
        }

        if (crewmates.Count == 0)
        {
            Debug.Log("[KillCircle] Không tìm thấy crewmate nào trong vùng kill.");
            return;
        }

        GameObject target = null;

        if (crewmates.Count == 1)
        {
            target = crewmates[0];
        }
        else
        {
            // Chọn crewmate gần nhất tâm kill circle
            float minDist = Mathf.Infinity;
            foreach (GameObject cm in crewmates)
            {
                float dist = Vector2.Distance(transform.position, cm.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = cm;
                }
            }
        }

        if (target != null)
        {
            target.SetActive(false);
            Debug.Log($"[KillCircle] Kill {target.name}");
        }
    }

    // Vẽ gizmo để debug vùng kill trong Scene
    void OnDrawGizmosSelected()
    {
        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
    }
}
