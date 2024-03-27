using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Racket : MonoBehaviour
{
    public Shooter shooter;
    private InputAction ballShootAction;

    private void Start()
    {
        // Create a new action for the button press
        ballShootAction = new InputAction("BallShoot", binding: "<XRController>{RightHand}/{GripButton}", type: InputActionType.Button);

        // Subscribe to the button press event
        ballShootAction.performed += OnButtonPress;
        ballShootAction.Enable();
    }

    private void OnButtonPress(InputAction.CallbackContext context)
    {
        shooter.ShootBall();
    }

}
