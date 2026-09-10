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
    float delayTimer;

    private readonly Queue<RewardInstance> notifQueue = new Queue<RewardInstance>();
    private Coroutine processCoroutine;
    public void ShowPickup(RewardInstance item)
    {
        notifQueue.Enqueue(item);

        if(processCoroutine == null) processCoroutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        while (notifQueue.Count > 0)
        {
            RewardInstance item = notifQueue.Dequeue();

            GameObject notif = Instantiate(notifPrefab, notifHolder);
            notif.GetComponent<PickUpObject>().InitializeNotif(item);

            yield return new WaitForSeconds(delay);
        }

        processCoroutine = null;
    }
}
