using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    
    float moveSpeed;
    float xDiff, yDiff;
    [SerializeField] float maxRotationX, maxRotationY;
    Vector3 worldPostionOfMouse;
    void Start()
    {
        maxRotationY = 0.1f;
        maxRotationX = 0.1f;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPostionOfMouse = Camera.main.ScreenToWorldPoint(mousePos);

        float screenX = (ScreenUtils.ScreenWidth/2), screenY = (ScreenUtils.ScreenHeight/2);

        xDiff = (  Mathf.Clamp( worldPostionOfMouse.x, -screenX, screenX ) ) / screenX;
        yDiff = (  Mathf.Clamp( worldPostionOfMouse.y, -screenY, screenY ) ) / screenY;


        Vector3 newAngle = new Vector3( yDiff*maxRotationY, xDiff*maxRotationX,  0 );
        transform.eulerAngles = newAngle;

        // Vector3 newPosition = new Vector3( -xDiff/10, -yDiff/10, -10);
        // transform.position = newPosition;
    }
}
