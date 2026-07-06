using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cameracontroll : MonoBehaviour
{
    public static Cameracontroll Instance;

    // If we want to select an item to follow, inside the item script add:
    // public void OnMouseDown(){
    //   CameraController.instance.followTransform = transform;
    // }

    [Header("General")]
    [SerializeField] Transform cameraTransform;
    public Transform followTransform;
    public GameObject fieldfofView;
    Vector3 newPosition;
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;

    [Header("Optional Functionality")]
    [SerializeField] bool moveWithKeyboad;
    [SerializeField] bool moveWithEdgeScrolling;
    [SerializeField] bool moveWithMouseDrag;

    [Header("Keyboard Movement")]
    [SerializeField] float fastSpeed = 0.05f;
    [SerializeField] float normalSpeed = 0.01f;
    [SerializeField] float movementSensitivity = 1f; // Hardcoded Sensitivity
    float movementSpeed;

    [Header("Edge Scrolling Movement")]
    [SerializeField] float edgeSize = 50f;
    bool isCursorSet = false;
    public Texture2D cursorArrowUp;
    public Texture2D cursorArrowDown;
    public Texture2D cursorArrowLeft;
    public Texture2D cursorArrowRight;

    [Header("Edges")]
    public Vector3 minBounds;
    public Vector3 maxBounds;

    private Transform cameraPosition;
    private Vector3 fowDefaultPosition;
    private Vector3 fowDefaultScale;
    private Vector3 cameraDefaultPosition;
    CursorArrow currentCursor = CursorArrow.DEFAULT;
    enum CursorArrow
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        DEFAULT
    }

    private void Start()
    {
        Instance = this;

        newPosition = transform.position;

        movementSpeed = normalSpeed;

        cameraPosition = Camera.main.transform;

        fowDefaultPosition = fieldfofView.transform.position;
        fowDefaultScale = fieldfofView.transform.localScale;
        cameraDefaultPosition = Camera.main.transform.position;
    }

    private void FixedUpdate()
    {
        // Allow Camera to follow Target
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        // Let us control Camera
        else
        {
            HandleCameraMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    private void LateUpdate()
    {
        //Restricting the camera position to specific limits
        Vector3 clampedPosition = cameraTransform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.z, maxBounds.z);

        //Assigning a restricted position to a camera
        cameraTransform.position = clampedPosition;
    }

    void HandleCameraMovement()
    {
        // Mouse Drag
        if (moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }

        // Keyboard Control
        if (moveWithKeyboad)
        {
            if (Input.GetKey(KeyCode.LeftCommand))
            {
                movementSpeed = fastSpeed;
            }
            else
            {
                movementSpeed = normalSpeed;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                newPosition += transform.right * -movementSpeed;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                newPosition += transform.right * movementSpeed;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                newPosition += transform.forward * movementSpeed;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                newPosition += transform.forward * -movementSpeed;
            }
            if (Input.GetKey(KeyCode.Equals) || Input.mouseScrollDelta.y > 0)
            {
                newPosition += transform.up * movementSpeed;
                if (cameraTransform.position.y < maxBounds.y)
                {
                    Debug.Log(cameraPosition.transform.position.y);
                    Debug.Log("Boundy: " + maxBounds.y);
                    fieldfofView.transform.position += new Vector3(-0.5f, 0.5f, 0);//<- rozkminka
                    fieldfofView.transform.localScale += new Vector3(0.3f, 0.3f, 0);
                }
            }
            if (Input.GetKey(KeyCode.Minus) || Input.mouseScrollDelta.y < 0)
            {
                newPosition += transform.up * -movementSpeed;
                if (cameraTransform.position.y > minBounds.y)
                {
                    float newXscale = fieldfofView.transform.localScale.x;
                    float newYscale = fieldfofView.transform.localScale.y;
                    newXscale -= 0.3f;
                    newYscale -= 0.3f;
                    if (newXscale <= Mathf.Max(newXscale, 0.1f) || newYscale <= Mathf.Max(newYscale, 0.1f))
                    {
                        newXscale = Mathf.Max(newXscale, 0.1f);
                        newYscale = Mathf.Max(newYscale, 0.1f);
                    }
                    fieldfofView.transform.position += new Vector3(0.5f, -0.5f, 0);
                    fieldfofView.transform.localScale = new Vector3(newXscale, newYscale, 0);
                }
            }
            if (Input.GetKey(KeyCode.Backspace))
            {
                cameraPosition.transform.position = cameraDefaultPosition;
                fieldfofView.transform.position = fowDefaultPosition;
                fieldfofView.transform.localScale = fowDefaultScale;
            }
        }

        // Edge Scrolling
        if (moveWithEdgeScrolling)
        {

            // Move Right
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                newPosition += Vector3.forward * movementSpeed;
                ChangeCursor(CursorArrow.RIGHT);
                isCursorSet = true;
            }

            // Move Left
            else if (Input.mousePosition.x < edgeSize)
            {
                newPosition += Vector3.forward * -movementSpeed;
                ChangeCursor(CursorArrow.LEFT);
                isCursorSet = true;
            }

            // Move Up
            else if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                newPosition += Vector3.right * -movementSpeed;
                ChangeCursor(CursorArrow.UP);
                isCursorSet = true;
            }

            // Move Down
            else if (Input.mousePosition.y < edgeSize)
            {
                newPosition += Vector3.right * movementSpeed;
                ChangeCursor(CursorArrow.DOWN);
                isCursorSet = true;
            }
            else
            {
                if (isCursorSet)
                {
                    ChangeCursor(CursorArrow.DEFAULT);
                    isCursorSet = false;
                }
            }
        }

        transform.position = newPosition;

        Cursor.lockState = CursorLockMode.Confined; // If we have an extra monitor we don't want to exit screen bounds
    }

    private void ChangeCursor(CursorArrow newCursor)
    {
        // Only change cursor if its not the same cursor
        if (currentCursor != newCursor)
        {
            switch (newCursor)
            {
                case CursorArrow.UP:
                    Cursor.SetCursor(cursorArrowUp, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.DOWN:
                    Cursor.SetCursor(cursorArrowDown, new Vector2(cursorArrowDown.width, cursorArrowDown.height), CursorMode.Auto); // So the Cursor will stay inside view
                    break;
                case CursorArrow.LEFT:
                    Cursor.SetCursor(cursorArrowLeft, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.RIGHT:
                    Cursor.SetCursor(cursorArrowRight, new Vector2(cursorArrowRight.width, cursorArrowRight.height), CursorMode.Auto); // So the Cursor will stay inside view
                    break;
                case CursorArrow.DEFAULT:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }

            currentCursor = newCursor;
        }
    }



    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;

        Gizmos.DrawWireCube(center, size);
    }

    public void ResetCameraCursor()
    {
        ChangeCursor(CursorArrow.DEFAULT);
    }
}