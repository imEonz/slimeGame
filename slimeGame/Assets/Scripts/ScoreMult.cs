using UnityEngine;

public class ScoreMult : MonoBehaviour
{
    void Start()
    {
        Vector3 thislocation = transform.localPosition;
        thislocation.x = Random.Range(-40,40);
        thislocation.y = Random.Range(0,30);

        transform.localPosition = thislocation;
    }
    public void Despawn()
    {
        Destroy(this.gameObject);
    }
}
