using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField]
    public float windUp;
    private enum ArrowState {Idle, Fired}
    private ArrowState arrowState = ArrowState.Idle;
    private Coroutine arrowWindUp;
    protected override void FixedUpdate()
    {
        if(arrowState == ArrowState.Idle)
        {
            if(arrowWindUp == null && target != null) arrowWindUp = StartCoroutine(WindUp(true));
            else if (arrowWindUp == null && target == null) arrowWindUp = StartCoroutine(WindUp(false));
        }
        if(arrowState == ArrowState.Fired) base.FixedUpdate();
    }

    private IEnumerator WindUp(bool shouldHome)
    {
        float elapsedTime = 0f;

        while(elapsedTime < windUp)
        {
            elapsedTime += Time.deltaTime;

            if(shouldHome)
            {
                if(target != null) direction = RecalculateDirection(target);
                else
                {
                    target = GetClosestEnemyInRange();
                    direction = RecalculateDirection(target);
                    transform.right = direction;
                }  
            }
            else
            {
                direction = RecalculateDirection(target);
                transform.right = direction; 
            }
            transform.right = direction;
            yield return null;
        }
        arrowState = ArrowState.Fired;
        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip("Arrow"),this.transform,0.08f);

    }

}
