
[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;                   //����(�������������ʽ����ת����Vector2Int)
    public GridBoolProperty gridBoolProperty;               //������(�Ƿ���Է������塢�Ƿ���ڵ�)
    public bool gridBoolValue = false;                      //����ֵ

    public GridProperty(GridCoordinate _gridCoordinate, GridBoolProperty _gridBoolProperty, bool _gridBoolValue)
    {
        gridCoordinate = _gridCoordinate;
        gridBoolProperty = _gridBoolProperty;
        gridBoolValue = _gridBoolValue;
    }
}
