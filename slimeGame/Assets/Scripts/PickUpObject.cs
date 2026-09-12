using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private float displayDuration;
    [SerializeField]
    TMP_Text objName;
    [SerializeField]
    TMP_Text level;
    [SerializeField]
    Slider expSlider;
    public void InitializeNotif(ActiveReward item)
    {
        objName.text = item.data.RewardName;
        level.text = "Lv: "+item.curLevel.ToString();
        expSlider.maxValue = item.curLevel+1f;
        expSlider.value = item.experience;
        StartCoroutine(IdleTime());
    }
    private IEnumerator IdleTime()
    {
        yield return new WaitForSeconds(displayDuration);
        animator.Play("ExitAnim");
    }
    public void OnExitDone()
    {
        Destroy(gameObject);
    }
}
