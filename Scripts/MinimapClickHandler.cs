using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera minimapCamera;   // the camera rendering the minimap
    public Camera mainCamera;

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localClick;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localClick))
            return;

        // Convert to normalised coordinates [0..1]
        Vector2 normalized = Rect.PointToNormalized(rectTransform.rect, localClick);

        // Convert to the pixel coordinates of the minimap camera
        Vector3 minimapPoint = new Vector3(
            normalized.x * minimapCamera.pixelWidth,
            normalized.y * minimapCamera.pixelHeight,
            0f
        );

        // Ray from the minimap camera into the world
        Ray ray = minimapCamera.ScreenPointToRay(minimapPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;

            // take the difference between the clicked point and the centre of the main camera’s view
            Ray centerRay = mainCamera.ViewportPointToRay(new Vector3(0.25f, 0.75f, 0));
            if (Physics.Raycast(centerRay, out RaycastHit centerHit))
            {
                Vector3 offset = hitPoint - centerHit.point;

                // we move the camera by the same offset → the view ‘moves’ to the clicked location
                mainCamera.transform.position += offset;
            }
            else
            {
                // fallback: if the ray from the centre of the screen doesn’t hit anything, we simply position the camera above that point
                Vector3 pos = mainCamera.transform.position;
                pos.x = hitPoint.x;
                pos.z = hitPoint.z;
                mainCamera.transform.position = pos;
            }
        }
    }
}