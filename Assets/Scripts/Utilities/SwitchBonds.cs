using Cinemachine;
using UnityEngine;

public class SwitchBonds : MonoBehaviour
{
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape() 
    {
        var confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        var confiner = GetComponent<CinemachineConfiner2D>();

        confiner.m_BoundingShape2D = confinerShape;

        //Çå³ý»º´æ
        confiner.InvalidateCache();
    }
}
