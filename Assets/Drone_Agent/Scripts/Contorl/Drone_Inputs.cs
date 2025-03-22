using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))] // Ensures that the GameObject has a PlayerInput component
public class Drone_Inputs : MonoBehaviour
{
    #region Variables
    // Variables to store input values
    private Vector2 cyclic;  // Stores cyclic control input (pitch and roll)
    private float pedals;     // Stores pedal control input (yaw)
    private float throttle;   // Stores throttle input (vertical control)

    // Public properties to access input values
    public Vector2 Cyclic { get => cyclic; }  // Property for cyclic input
    public float Pedals { get => pedals; }     // Property for pedals input
    public float Throttle { get => throttle; } // Property for throttle input
    #endregion

    #region Input Methods
    // Update is called once per frame
    void Update()
    {
        // Any updates related to inputs can be placed here.
    }

    // Method called when cyclic input changes
    private void OnCyclic(InputValue value)
    {
        cyclic = value.Get<Vector2>(); // Get the Vector2 input value for cyclic control
    }

    // Method called when pedal input changes
    private void OnPedals(InputValue value)
    {
        pedals = value.Get<float>(); // Get the float input value for pedals control
    }

    // Method called when throttle input changes
    private void OnThrottle(InputValue value)
    {
        throttle = value.Get<float>(); // Get the float input value for throttle control
    }
    #endregion
}
