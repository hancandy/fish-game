using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FishController : MonoBehaviour {

    public Transform tail = null;
    public Vector2 turningSpeed = new Vector2(100.0f, 60.0f);
    public float wiggleThreshold = 1.0f;
    public float propulsionSpeed = 0.05f;

    private Vector2 lastPointerDelta = new Vector2(0.0f, 0.0f);
    private Vector2 lastPointerPosition = new Vector2(0.0f, 0.0f);

    /**
     * Gets pointer position
     */
    Vector2 GetPointerPosition()
    {
        Vector2 pointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // Override pointerPosition if touches are available
        if(Input.touchCount > 0)
        {
            Touch inputTouch = Input.GetTouch(0);
            pointerPosition = inputTouch.position;        
        }

        return pointerPosition;
    }

    /**
     * Updates the rotation
     */
    void UpdateRotation()
    {
        // Cancel if there are no touches or mouse inputs
        if(Input.touchCount < 1 && !Input.GetMouseButton(0)) { return; }

        // Set relevant vectors
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 pointerPosition = GetPointerPosition();
        Vector2 turningDelta = new Vector2(0.0f, 0.0f);

        // Set the delta in relation to the pointer position on the screen
        turningDelta.x = (pointerPosition.x - center.x) / Screen.width;
        turningDelta.y = (pointerPosition.y - center.y) / Screen.height;

        // Multiply by turning speed
        turningDelta.x *= turningSpeed.x;
        turningDelta.y *= turningSpeed.y;

        // Multiply by delta time
        turningDelta *= Time.deltaTime;

        // Perform rotation
        transform.Rotate(Vector3.right * turningDelta.y);
        transform.Rotate(Vector3.up * turningDelta.x);

        // Cache pointer position
        lastPointerPosition = pointerPosition;
    }

    /**
     * Updates the forward propulsion
     */
    void UpdateForwardPropulsion()
    {
        // Cancel if there are no touches or mouse inputs
        if(Input.touchCount < 1 && !Input.GetMouseButton(0)) { return; }
        
        // Get the pointer delta vector
        Vector2 pointerPosition = GetPointerPosition();
        Vector2 pointerDelta = pointerPosition - lastPointerPosition;

        // Start forward translation if input delta is above threshold
        float combinedLastPointerDelta = Mathf.Abs(lastPointerDelta.x) + Mathf.Abs(lastPointerDelta.y);
        
        if(combinedLastPointerDelta > wiggleThreshold)
        {
            // Subtract threshold to normalise propulsion
            combinedLastPointerDelta -= wiggleThreshold;

            transform.localPosition = transform.localPosition - transform.forward * (combinedLastPointerDelta * Time.deltaTime * propulsionSpeed);
        }

        // Cache last pointer delta
        lastPointerDelta = pointerDelta;
    }

    /**
     * Updates the tail rotation
     */
    void UpdateTailRotation()
    {
        Vector3 currentRotation = tail.transform.localEulerAngles;

        currentRotation.y = Mathf.PingPong(Time.time * 100.0f, 50.0f) - 25.0f;
        
        tail.transform.localEulerAngles = currentRotation;
    }

    /**
     * Update loop
     */
    void FixedUpdate()
    {
        UpdateForwardPropulsion();
        UpdateRotation();
        UpdateTailRotation();
    }
}
