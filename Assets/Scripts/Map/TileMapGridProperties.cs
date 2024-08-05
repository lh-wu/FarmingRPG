using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine;

[ExecuteAlways]
public class TileMapGridProperties : MonoBehaviour
{
    //���ڱ��ű��е�EditorUtility.SetDirty(gridProperties);�����ڱ༭ģʽ�����У������Ҫ#if UNITY_EDITOR�����Ʊ��루���ڱ༭���ã�
#if UNITY_EDITOR
    // ��attach��ÿһ��scene���ض�(���ԣ�diggable��candrop������)��tilemap��
    // ������tilemap��enableʱ�ɻ��ƣ���disable��ʱ�򽫻��ƵĽ����list��ʽ������SO�ļ���
    private Tilemap tilemap;
    [SerializeField] private SO_GridProperties gridProperties = null;               //һ��scene�Ĺ���һ��SO_GridProperties����Ҫ��hierarchy�ֶ���ֵ
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();
            if (gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();
            if (gridProperties != null)
            {
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }


    /// <summary>
    /// �����ƺõĽ��������gridProperties.gridPropertyList��
    /// </summary>
    private void UpdateGridProperties()
    {
        tilemap.CompressBounds();                       //unity�ı߽粻���Զ����������ţ������ڷǳ�Զ�ĵط�������һ��tile��ɾ���������Զ���С��

        // ���༭ʱ����
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for(int x= startCell.x; x < endCell.x; ++x)
                {
                    for(int y = startCell.y; y < endCell.y; ++y)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        // ��ʾ������֮ǰ��Ҫ���ýű�attch��gameobject����
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("You need to disable property tilemaps before running game");
        }
    }
#endif
}
