using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSubscriptor : MonoBehaviour
{
    [SerializeField]
    private Slider sliderReference = null;

    [SerializeField]
    private string subscriptionEvent = null;

    void Start()
    {
        if (sliderReference == null) {
            Debug.LogError("Error - no sliderReference specified");
        }

        if (subscriptionEvent == null)
        {
            Debug.LogError("Error - no subscriptionEvent specified");
        }
    }

    internal virtual void OnEnable()
    {
        EventManager.StartListening(subscriptionEvent, OnValueChange);
    }

    internal virtual void OnDisable()
    {
        EventManager.StopListening(subscriptionEvent, OnValueChange);
    }

    private void OnValueChange(Dictionary<string, object> obj)
    {
        sliderReference.value = (float) obj["amount"];
    }
}
