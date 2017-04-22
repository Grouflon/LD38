using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backdrop : MonoBehaviour
{
    public float distance = 100.0f;
    public float tilingScale = 1.0f;

	void Start ()
	{
        m_renderer = GetComponentInChildren<MeshRenderer>();
	}
	
	void Update ()
	{
        Camera cam = Camera.main;

        Ray BLRay = cam.ViewportPointToRay(Vector3.zero);
        Ray TRRay = cam.ViewportPointToRay(new Vector3(1.0f, 1.0f, 0.0f));

        Vector3 BLPoint = BLRay.origin + BLRay.direction * distance;
        Vector3 TRPoint = TRRay.origin + TRRay.direction * distance;
        Vector3 BLToTR = TRPoint - BLPoint;
        float size = BLToTR.magnitude;
        transform.position = BLPoint + BLToTR * 0.5f;
        transform.LookAt(cam.transform);
        transform.localScale = Vector3.one * size;

        m_renderer.material.mainTextureScale = Vector2.one * size * tilingScale;
    }

    MeshRenderer m_renderer;
}
