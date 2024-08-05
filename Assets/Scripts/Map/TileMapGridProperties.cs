using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine;

[ExecuteAlways]
public class TileMapGridProperties : MonoBehaviour
{
    //由于本脚本中的EditorUtility.SetDirty(gridProperties);仅可在编辑模式中运行，因此需要#if UNITY_EDITOR来控制编译（仅在编辑可用）
#if UNITY_EDITOR
    // 被attach到每一个scene的特定(属性，diggable或candrop或其他)的tilemap中
    // 附属的tilemap在enable时可绘制，在disable的时候将绘制的结果以list形式保存在SO文件中
    private Tilemap tilemap;
    [SerializeField] private SO_GridProperties gridProperties = null;               //一个scene的共用一个SO_GridProperties，需要在hierarchy手动赋值
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
    /// 将绘制好的结果保存在gridProperties.gridPropertyList中
    /// </summary>
    private void UpdateGridProperties()
    {
        tilemap.CompressBounds();                       //unity的边界不会自动调整到最优，（如在非常远的地方绘制了一个tile再删除，不会自动缩小）

        // 仅编辑时运行
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
        // 提示在运行之前，要将该脚本attch的gameobject禁用
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("You need to disable property tilemaps before running game");
        }
    }
#endif
}
