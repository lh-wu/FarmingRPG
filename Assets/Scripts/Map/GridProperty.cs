
[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;                   //坐标
    public GridBoolProperty gridBoolProperty;               //属性名
    public bool gridBoolValue = false;                      //属性值

    public GridProperty(GridCoordinate _gridCoordinate, GridBoolProperty _gridBoolProperty, bool _gridBoolValue)
    {
        gridCoordinate = _gridCoordinate;
        gridBoolProperty = _gridBoolProperty;
        gridBoolValue = _gridBoolValue;
    }
}
