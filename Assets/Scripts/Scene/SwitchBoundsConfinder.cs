using Cinemachine;
using UnityEngine;

public class SwitchBoundsConfinder : MonoBehaviour
{
    // 所有场景（包括初始场景）被加载后都会invoke一次EventHandler.AfterSceneLoadEvent，
    // 订阅该方法以切换相机的confinder
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchToCurrentBound;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchToCurrentBound;

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
