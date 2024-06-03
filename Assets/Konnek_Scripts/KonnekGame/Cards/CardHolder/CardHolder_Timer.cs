/// <summary>
/// Timer will start when pointer enter the card,
/// when pointer exited the timer will stopped;
/// </summary>
/// 
using System.Collections;
using UnityEngine;
public partial class CardHolder
{
    [Header("Timer Config")]

    [Tooltip("Duration of how long that pointer need to stay in card to trigger the event")]
    [SerializeField] private float pointerEnterTriggerDuration;
    private Coroutine _timer;

    private void StartTimer()
    {
        _timer = StartCoroutine(Timer());
    }
    private void StopTimer()
    {
        if (_timer != null)
        {
            StopCoroutine(_timer);
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(pointerEnterTriggerDuration);
        PlayerHandManager.Instance.OnPointerEnterTriggered?.Invoke(this);
        StopTimer();
    }
}