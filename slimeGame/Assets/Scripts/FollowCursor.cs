using UnityEngine;
using UnityEngine.UI;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Sprite auto;
    [SerializeField] private Sprite aiming;
    private Image image;
    private RectTransform rect;
    PlayerController player;

    void Awake() => rect = GetComponent<RectTransform>();

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        image = GetComponent<Image>();
        Cursor.visible = false;
    }

    void Update()
    {
        if(player != null)
        {
            if(player.AutoAim) image.sprite = auto;
            else image.sprite = aiming;
        }
        else image.sprite = auto;


        Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
        (RectTransform)rect.parent, Input.mousePosition, cam, out Vector3 worldPoint))
        {
            rect.position = worldPoint;
        }
    }
}