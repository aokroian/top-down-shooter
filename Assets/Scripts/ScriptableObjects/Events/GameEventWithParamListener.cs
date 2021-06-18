using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventWithParamListener<T> : MonoBehaviour
{
    public GameEventWithParam<T> eventForListen;
    public UnityEvent<T> response;

    private void OnEnable()
    {
        eventForListen.AddListener(this);
    }

    private void OnDisable()
    {
        eventForListen.RemoveListener(this);
    }

    public void OnEventRaised(T param)
    {
        response.Invoke(param);
    }
}
