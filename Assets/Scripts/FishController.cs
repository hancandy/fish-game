using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FishController : MonoBehaviour {

    public Transform tail = null;
    public Vector2 turningSpeed = new Vector2(100.0f, 60.0f);
    public float wiggleThreshold = 1.0f;
    public float turningThreshold = 0.1f;

	void Start()
    {
	}
	
    /**
     * Get the turning delta based on pointer position on screen
     */
    Vector2 GetTurningDelta()
    {
        // Cancel if there are no touches or mouse inputs
        if(Input.touchCount < 1 && !Input.GetMouseButton(0)) { return new Vector2(0.0f, 0.0f); }

        Vector2 pointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 turningDelta = new Vector2(0.0f, 0.0f);

        // Override pointerPosition if touches are available
        if(Input.touchCount > 0)
        {
            Touch inputTouch = Input.GetTouch(0);
            pointerPosition = inputTouch.position;        
        }

        // The pointer has reached the leftmost 10% of the screen
		if(pointerPosition.x < Screen.width * turningThreshold)
        {
            float percent = 1.0f - (pointerPosition.x / (Screen.width * turningThreshold));

            turningDelta.x = -turningSpeed.x * percent;
        }

        // The pointer has reached the rightmost 10% of the screen
        else if(pointerPosition.x > Screen.width - Screen.width * turningThreshold)
        {
            float percent = 1.0f - ((Screen.width - pointerPosition.x) / (Screen.width * turningThreshold));
            
            turningDelta.x = turningSpeed.x * percent;
        }
        
        // The pointer has reached the top 10% of the screen
        if(pointerPosition.y > Screen.height - Screen.height * turningThreshold)
        {
            float percent = 1.0f - ((Screen.height - pointerPosition.y) / (Screen.height * turningThreshold));
            
            turningDelta.y = turningSpeed.y * percent;
        }

        // The pointer has reached the bottom 10% of the screen
        else if(pointerPosition.y < Screen.height * turningThreshold)
        {
            float percent = 1.0f - (pointerPosition.y / (Screen.height * turningThreshold));

            turningDelta.y = -turningSpeed.y * percent;
        }

        turningDelta *= Time.deltaTime;

        return turningDelta;
    }

    /**
     * Updates the rotation
     */
    void UpdateRotation()
    {
	    Vector2 turningDelta = GetTurningDelta();
        Vector3 currentRotation = transform.localEulerAngles;

        currentRotation.y += turningDelta.x;
        currentRotation.x += turningDelta.y;

        transform.Rotate(Vector3.right * turningDelta.y);
        transform.Rotate(Vector3.up * turningDelta.x);
    }

    /**
     * Updates the forward propulsion
     */
    void UpdateForwardPropulsion()
    {
        // Cancel if there are no touches or mouse inputs
        if(Input.touchCount < 1 && !Input.GetMouseButton(0)) { return; }
        
        Vector2 pointerPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        // Override pointerPosition and deltas if touches are available
        if(Input.touchCount > 0)
        {
            Touch inputTouch = Input.GetTouch(0);
            pointerPosition = inputTouch.position;
            deltaY = inputTouch.deltaPosition.y;
            deltaX = inputTouch.deltaPosition.x;
        }

        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        float totalDelta = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);
        bool isWithinX = pointerPosition.x > center.x - Screen.width * wiggleThreshold && pointerPosition.x < center.x + Screen.width * wiggleThreshold;
        bool isWithinY = pointerPosition.y > center.y - Screen.width * wiggleThreshold && pointerPosition.y < center.y + Screen.width * wiggleThreshold;

        if(isWithinX && isWithinY)
        {
            transform.localPosition = transform.localPosition - transform.forward * (totalDelta * Time.deltaTime);
        }
    }

	void Update()
    {
        UpdateRotation();
        UpdateForwardPropulsion();
    }
}
