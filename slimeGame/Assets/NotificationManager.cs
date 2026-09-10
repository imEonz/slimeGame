using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField]
    Transform notifHolder;
    [SerializeField]
    GameObject notifPrefab;
    [SerializeField]
    float delay;

    private readonly Queue<ActiveReward> notifQueue = new Queue<ActiveReward>();
    private Coroutine processCoroutine;
    public void ShowPickup(ActiveReward item)
    {
        notifQueue.Enqueue(item);

        if(processCoroutine == null) processCoroutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        while (notifQueue.Count > 0)
        {
            ActiveReward item = notifQueue.Dequeue();

            GameObject notif = Instantiate(notifPrefab, notifHolder);
            notif.GetComponent<PickUpObject>().InitializeNotif(item);

            yield return new WaitForSeconds(delay);
        }

        processCoroutine = null;
    }
}
