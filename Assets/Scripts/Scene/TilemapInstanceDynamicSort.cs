using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInstanceDynamicSort : MonoBehaviour
{
    TilemapRenderer tilemapRenderer;
    int originSortOrder;
    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        originSortOrder = tilemapRenderer.sortingOrder;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!=null && collision.gameObject.CompareTag("Player"))
        {
            tilemapRenderer.sortingOrder = -1;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            tilemapRenderer.sortingOrder = originSortOrder;
        }
    }
}
