using UnityEngine;

public class FrameAboveGround : MonoBehaviour
{
    public float offsetY = 0.5f; // how high above the ground should the frame be suspended?
    public LayerMask groundMask;

    void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 100f, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + offsetY;
            transform.position = pos;
        }
    }
}
