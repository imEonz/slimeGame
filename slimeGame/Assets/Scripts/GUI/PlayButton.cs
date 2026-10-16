using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=> GameManager.Instance.ButtonChangeScene(1));
    }
}
