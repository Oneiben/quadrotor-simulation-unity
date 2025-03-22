using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XeroxUAV
{
    public class Drone_Engine : MonoBehaviour, IEngine
    {
        #region Variables
        [Header("Engine Properties")]
        [SerializeField] private float maxpower = 2f; // Maximum power output of the engine

        [Header("Propeller Properties")]
        [SerializeField] private Transform propeller; // Reference to the propeller object
        [SerializeField] private float propRotSpeed = 300f; // Speed of propeller rotation
        #endregion

        #region Interface Implementation
        public void InitEngine()
        {
            // This method needs to be implemented based on your engine initialization logic
            throw new System.NotImplementedException();
        }

        public void UpdateEngine(Rigidbody rb, Drone_Inputs input)
        {
            // Calculate the upward vector and normalize it
            Vector3 upVec = transform.up;
            upVec.x = 0f; // Zero out x component to ensure vertical thrust
            upVec.z = 0f; // Zero out z component to ensure vertical thrust

            // Calculate tilt angle (the angle between the upward vector and world up)
            float tiltAngle = Vector3.Angle(Vector3.up, transform.up);
            // Use cosine of the tilt angle to compensate for the reduced vertical thrust
            float compensation = Mathf.Cos(tiltAngle * Mathf.Deg2Rad);

            // Calculate the engine force with compensation for the tilt
            Vector3 engineForce = transform.up * ((rb.mass * Physics.gravity.magnitude) / compensation + (input.Throttle * maxpower)) / 4f;

            // Apply the calculated force to the rigidbody
            rb.AddForce(engineForce, ForceMode.Force);

            // Handle propeller rotation
            HandlePropellers();
        }

        void HandlePropellers()
        {
            // Check if the propeller is assigned before attempting to rotate
            if (!propeller)
            {
                return; // Exit if no propeller is assigned
            }
            // Rotate the propeller
            propeller.Rotate(Vector3.forward, propRotSpeed);
        }
        #endregion
    }
}
