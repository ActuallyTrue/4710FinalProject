using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public StatePlayerController target;
    public Vector2 focusAreaSize;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    FocusArea focusArea;
    
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;
    public void makeFocusArea(StatePlayerController playerController) {
        target = playerController;
        focusArea = new FocusArea(target.boxCollider.bounds, focusAreaSize);
        Debug.Log(lookAheadDirX);
    }

    //this updates at the end of the frame (after we've moved the player)
    private void LateUpdate()
    {
        if (target != null) {
            focusArea.Update(target.boxCollider.bounds);

            Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
            //if the camera is moving and the player is moving then do the whole camera swing
            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                float xInput = target.moveInput.x;
                if (Mathf.Sign(xInput) == Mathf.Sign(focusArea.velocity.x) && xInput != 0) {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else {
                    if (!lookAheadStopped) {
                                lookAheadStopped = true;
                                //Parentheses give how far the camera has to move to catch up, then we divide by 4 to stop it earlier
                                targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                    }
                }
            }

            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;

            transform.position = (Vector3)focusPosition + Vector3.forward * -10;

        }
    }

    public IEnumerator Shake(float duration, float magnitude) 
    {
        Debug.Log("shake it off");
        Vector3 orignalPos = transform.localPosition;

        float elapsed = 0.0f;
        while(elapsed < duration) 
        {
            float xOffset = Random.Range(-1, 1) * magnitude;
            float yOffset = Random.Range(-1, 1) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, orignalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = orignalPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left;
        float right;
        float top;
        float bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;

            center = new Vector2((left + right) /2,(top + bottom) / 2);
        }

        public void Update(Bounds targetBounds) {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right) {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);

        }
    }
}
    
