using UnityEngine;

public class BillboardText : MonoBehaviour
{
    public Transform target;      
    public Vector3 offset = Vector3.up * 2f;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || mainCam == null) return;

        transform.position = target.position + offset;

        transform.forward = mainCam.transform.forward;
    }
}