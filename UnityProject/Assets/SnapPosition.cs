using UnityEngine;
using System.Collections;

public class SnapPosition : MonoBehaviour 
{
    public int pixelsPerUnit = 16;
    Transform parent;

    void Start()
    {
        parent = transform.parent;
    }

    void LateUpdate () 
    {
        Vector3 newLocalPos = Vector3.zero;
        newLocalPos.x = (SnapToPixels(parent.position.x)) - parent.position.x;
        newLocalPos.y = (SnapToPixels(parent.position.y)) - parent.position.y;
        transform.localPosition = newLocalPos;
    }

    float SnapToPixels(float a)
    {        
        return Mathf.Ceil(a * pixelsPerUnit)/pixelsPerUnit;
    }
}
