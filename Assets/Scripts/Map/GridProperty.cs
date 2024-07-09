
[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;                   //坐标(相对于设置了显式类型转换的Vector2Int)
    public GridBoolProperty gridBoolProperty;               //属性名(是否可以放置物体、是否可挖等)
    public bool gridBoolValue = false;                      //属性值

    public GridProperty(GridCoordinate _gridCoordinate, GridBoolProperty _gridBoolProperty, bool _gridBoolValue)
    {
        gridCoordinate = _gridCoordinate;
        gridBoolProperty = _gridBoolProperty;
        gridBoolValue = _gridBoolValue;
    }
}
