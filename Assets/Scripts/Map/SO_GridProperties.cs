using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName;
    // �����ĳ����������������·�����ֵ
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;         //����grid������(diggable��candrop)���ᱻ�洢�����list��
}
