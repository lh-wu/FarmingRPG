using Cinemachine;
using UnityEngine;

public class SwitchBoundsConfinder : MonoBehaviour
{
    // ���г�����������ʼ�����������غ󶼻�invokeһ��EventHandler.AfterSceneLoadEvent��
    // ���ĸ÷������л������confinder
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchToCurrentBound;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchToCurrentBound;

    }

    // ���������BoundsΪ��ǰ����(ͨ��ƥ��tagʵ��)
    private void SwitchToCurrentBound()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag(Tags.BoundsConfinder).GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = confinerShape;
        cinemachineConfiner.InvalidatePathCache();
    }
}
