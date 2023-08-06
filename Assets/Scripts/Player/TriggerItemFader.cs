using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] fades = collision.GetComponentsInChildren<ItemFader>();

        if (fades.Length > 0) 
        {
            foreach (var a in fades) 
            {
                a.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] fades = collision.GetComponentsInChildren<ItemFader>();

        if (fades.Length > 0)
        {
            foreach (var a in fades)
            {
                a.FadeIn();
            }
        }
    }
}
