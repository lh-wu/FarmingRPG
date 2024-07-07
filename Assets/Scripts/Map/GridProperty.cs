
[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;                   //����
    public GridBoolProperty gridBoolProperty;               //������
    public bool gridBoolValue = false;                      //����ֵ

    public GridProperty(GridCoordinate _gridCoordinate, GridBoolProperty _gridBoolProperty, bool _gridBoolValue)
    {
        gridCoordinate = _gridCoordinate;
        gridBoolProperty = _gridBoolProperty;
        gridBoolValue = _gridBoolValue;
    }
}
