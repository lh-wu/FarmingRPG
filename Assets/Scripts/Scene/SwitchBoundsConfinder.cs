using Cinemachine;
using UnityEngine;

public class SwitchBoundsConfinder : MonoBehaviour
{

    void Start()
    {
        SwitchToCurrentBound();
    }

    // 设置相机的Bounds为当前场景(通过匹配tag实现)
    private void SwitchToCurrentBound()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag(Tags.BoundsConfinder).GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = confinerShape;
        cinemachineConfiner.InvalidatePathCache();
    }
}
