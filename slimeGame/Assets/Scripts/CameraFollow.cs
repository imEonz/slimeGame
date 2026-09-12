using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultMagnitude = 0.15f;

    [SerializeField] private bool useCursorOffset = true;
    [SerializeField] private float maxCursorOffset = 2f;
    [SerializeField] private float cursorOffsetSmoothTime = 0.3f;
    private Camera cam;

    private Vector3 Pos;
    private Coroutine shakeRoutine;

    private Vector3 velocity = Vector3.zero;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 cursorOffset = Vector3.zero;
    private Vector3 cursorOffsetVelocity = Vector3.zero;

    void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateCursorOffset();
        
        Vector3 targetPosition = target.position + offset + cursorOffset;
        Vector3 smoothed = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

        transform.position = smoothed + shakeOffset;
    }


    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }


    private void UpdateCursorOffset()
    {
        Vector3 desiredCursorOffset = Vector3.zero;

        if (useCursorOffset && Mouse.current != null && cam != null)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            // Normalize to -1..1 range relative to screen center
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 normalized = new Vector2(
                (mouseScreenPos.x - screenCenter.x) / screenCenter.x,
                (mouseScreenPos.y - screenCenter.y) / screenCenter.y
            );

            // Clamp so it doesn't overshoot if cursor is outside window bounds
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            desiredCursorOffset = new Vector3(normalized.x, normalized.y, 0f) * maxCursorOffset;
        }

        cursorOffset = Vector3.SmoothDamp(cursorOffset, desiredCursorOffset, ref cursorOffsetVelocity, cursorOffsetSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }
}
