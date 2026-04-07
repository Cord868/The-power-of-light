using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f; //Удерживание кнопки на сколько хочешь прыгнуть выше
    public Image fillCircle;

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if (holdTimer >= holdDuration)
            {
                //Загруказ следующего уровня
                OnHoldComplete.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }

}
