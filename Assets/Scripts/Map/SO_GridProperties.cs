using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName;
    // 场景的长、宽；场景的最左下方坐标值
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;         //所有grid的属性(diggable、candrop)都会被存储在这个list中
}
