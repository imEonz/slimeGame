using UnityEngine;

public class DVDBounce : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private float cornerTolerance = 20f;

    private RectTransform rect;
    private Vector2 direction;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        direction = new Vector2(
            Random.Range(-1f,1f),
            Random.Range(-1f,1f)
        ).normalized;
    }

    void Update()
    {
        rect.anchoredPosition += direction * speed * Time.unscaledDeltaTime;

        Vector2 halfCanvas = canvasRect.rect.size / 2f;
        Vector2 halfSelf = rect.rect.size / 2f;

        Vector2 pos = rect.anchoredPosition;

        bool hitX = false;
        bool hitY = false;

        float maxX = halfCanvas.x - halfSelf.x;
        float maxY = halfCanvas.y - halfSelf.y;

        if (pos.x > maxX)  { pos.x = maxX;  direction.x = -Mathf.Abs(direction.x); hitX = true;}
        if (pos.x < -maxX) { pos.x = -maxX; direction.x =  Mathf.Abs(direction.x); hitX = true;}
        if (pos.y > maxY)  { pos.y = maxY;  direction.y = -Mathf.Abs(direction.y); hitY = true;}
        if (pos.y < -maxY) { pos.y = -maxY; direction.y =  Mathf.Abs(direction.y); hitY = true;}

        rect.anchoredPosition = pos;

        if (hitX && Mathf.Abs(pos.y) >= maxY - cornerTolerance) OnCornerHit();
        else if (hitY && Mathf.Abs(pos.x) >= maxX - cornerTolerance) OnCornerHit();
    }

    private void OnCornerHit()
    {
        Debug.Log("CORNERRRR FUCING CORNERRR");
    }
}
