using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace XeroxUAV
{
    public class Drone_Control : Base_RigidBody
    {
        #region Variables
        [Header("Control Properties")]
        [SerializeField] private float minMaxPitch = 30f; // Maximum pitch angle
        [SerializeField] private float minMaxRoll = 30f;  // Maximum roll angle
        [SerializeField] private float YawPower = 4f;     // Power for yaw control
        [SerializeField] private float lerpSpeed = 2f;    // Speed for interpolating angles

        // Reference to input controls
        private Drone_Inputs input; 

        // List of engines for the drone
        private List<IEngine> engines = new List<IEngine>(); 

        // Variables to store final rotation angles
        private float finalPitch;
        private float finalRoll;
        private float yaw;
        private float finalYaw;
        #endregion

        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            // Get input component
            input = GetComponent<Drone_Inputs>();
            // Get all engine components
            engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
        }
        #endregion

        #region Custom Methods
        protected override void HandlePhysics()
        {
            // Handle engines and controls for the drone
            HandleEngines();
            HandleControls();
        }

        protected virtual void HandleEngines()
        {
            // Update each engine based on the current state
            foreach (IEngine engine in engines)
            {
                engine.UpdateEngine(rb, input);
            }
        }

        protected virtual void HandleControls()
        {
            // Calculate pitch, roll, and yaw based on input
            float pitch = input.Cyclic.y * minMaxPitch; // Pitch input
            float roll = -input.Cyclic.x * minMaxRoll;  // Roll input (inverted)
            yaw += input.Pedals * YawPower;              // Yaw input

            // Smoothly interpolate final angles
            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);

            // Apply the final rotation to the rigidbody
            Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
            rb.MoveRotation(rot); 
        }
        #endregion
    }
}
