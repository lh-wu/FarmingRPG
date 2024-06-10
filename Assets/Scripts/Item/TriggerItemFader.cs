
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] itemFaders = collision.gameObject.GetComponentsInChildren<ItemFader>();
        if (itemFaders.Length > 0)
        {
            for(int i = 0; i < itemFaders.Length; ++i)
            {
                itemFaders[i].FadeOut();
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] itemFaders = collision.gameObject.GetComponentsInChildren<ItemFader>();
        if (itemFaders.Length > 0)
        {
            for (int i = 0; i < itemFaders.Length; ++i)
            {
                itemFaders[i].FadeIn();
            }
        }
    }
}
