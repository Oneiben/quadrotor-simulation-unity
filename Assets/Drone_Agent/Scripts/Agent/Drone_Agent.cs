using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System.Drawing.Text;

namespace DroneAgent
{
    public class MyAgent : Agent
    {

#region Variables & Initialization
        // Declare variables and components for the drone and environment
        [SerializeField] private Transform Agent; // Reference to the drone itself
        [SerializeField] private Transform propeller1, propeller2, propeller3, propeller4;
        [SerializeField] private float propRotSpeed = 300f; // Propeller rotation speed

        // Engine and control properties
        [Header("Engine Properties")]
        [SerializeField] private float maxpower = 100f; // Maximum engine power

        [Header("Control Properties")]
        [SerializeField] private float minMaxPitch = 20f; // Maximum pitch angle
        [SerializeField] private float minMaxRoll = 20f; // Maximum roll angle
        [SerializeField] private float YawPower = 5f; // Yaw control power
        [SerializeField] private float weighTnbls = 1f; // Drone weight in lbs

        [Header("Speed Settings")]
        [SerializeField] private float horizontalSpeedFactor = 2f; // Multiplier for horizontal speed
        [SerializeField] private float lerpSpeed = 2f; // Interpolation speed for smooth control

        const float lbsTokg = 0.78f; // Conversion factor for lbs to kg
        private Vector3 lastPosition; // Store the last position of the drone

        // List of engines in the drone
        private List<IEngine> engines = new List<IEngine>(); 

        // Control variables for pitch, roll, and yaw
        private float finalPitch;
        private float finalRoll;
        private float yaw;
        private float finalYaw;

        private Rigidbody rb; // Rigidbody component of the drone
        protected float startDrag = 1f; // Initial drag
        protected float startAngularDrag; // Initial angular drag
#endregion

#region Environment Reset
        public override void OnEpisodeBegin()
        {   
            // Reset drone position and rotation
            Agent.rotation = Quaternion.Euler(0f, 0f, 0f);
            yaw = 0f;
        }
#endregion

#region Engine Handling
        void Awake()
        {
            // Initialize Rigidbody and engine list
            rb = GetComponent<Rigidbody>();
            engines = GetComponentsInChildren<IEngine>().ToList();

            // Set the mass of the drone based on its weight
            if (rb)
            {
                rb.mass = weighTnbls * lbsTokg;
                startDrag = rb.linearDamping; // Store initial drag
                startAngularDrag = rb.angularDamping; // Store initial angular drag
            }
        }

        public void UpdateEngine()
        {
            // Calculate vertical thrust compensation based on tilt angle
            Vector3 upVec = transform.up;
            upVec.x = 0f;
            upVec.z = 0f;

            float tiltAngle = Vector3.Angle(Vector3.up, transform.up);
            float compensation = Mathf.Cos(tiltAngle * Mathf.Deg2Rad); // Calculate compensation

            // Calculate and apply engine force
            Vector3 engineForce = transform.up * (rb.mass * Physics.gravity.magnitude) / compensation / 4f;
            rb.AddForce(engineForce, ForceMode.Force); // Apply vertical force

            // Rotate propellers
            HandlePropellers();
        }

        void HandlePropellers()
        {
            // Rotate each propeller if it's assigned
            if (propeller1)
            {
                propeller1.Rotate(Vector3.forward, propRotSpeed);
                propeller2.Rotate(Vector3.forward, propRotSpeed);
                propeller3.Rotate(Vector3.forward, propRotSpeed);
                propeller4.Rotate(Vector3.forward, propRotSpeed);
            }
        }

        protected virtual void HandleEngines()
        {
            // Update each engine
            foreach (IEngine engine in engines)
            {
                UpdateEngine();
            }
        }
#endregion

#region Action Processing
        public override void OnActionReceived(ActionBuffers actions)
        {
            // Process engine control actions (pitch, roll, yaw)
            HandleEngines();

            float roll = -actions.ContinuousActions[0] * minMaxRoll; // Roll input
            float pitch = actions.ContinuousActions[1] * minMaxPitch; // Pitch input
            yaw += actions.ContinuousActions[2] * YawPower; // Yaw input

            // Smoothly interpolate pitch and roll
            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);
        
            // Apply throttle input for vertical force
            float throttle = actions.ContinuousActions[3];
            Vector3 engineForce = transform.up * (throttle * maxpower) / 4f;
            // Apply forces to the Rigidbody
            rb.AddForce(engineForce, ForceMode.Force);

            // Calculate the forward and right directions based on the current yaw
            float yawInRadians = finalYaw * Mathf.Deg2Rad; // Convert yaw to radians
            float forwardMovementX = Mathf.Sin(yawInRadians) * finalPitch * horizontalSpeedFactor; // Forward direction
            float forwardMovementZ = Mathf.Cos(yawInRadians) * finalPitch * horizontalSpeedFactor; // Forward direction
            float rightMovementX = -Mathf.Cos(yawInRadians) * finalRoll * horizontalSpeedFactor; // Right direction
            float rightMovementZ = Mathf.Sin(yawInRadians) * finalRoll * horizontalSpeedFactor; // Right direction

            // Combine forward and sideways movement
            Vector3 horizontalMovement = new Vector3(forwardMovementX + rightMovementX, 0, forwardMovementZ + rightMovementZ);

            // Apply forces to the Rigidbody using the local horizontal movement
            rb.AddForce(horizontalMovement, ForceMode.Force);

            // Apply rotation to the drone
            Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
            rb.MoveRotation(rot);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // Manual control inputs for heuristic mode
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            continuousActions[0] = Input.GetAxisRaw("Horizontal"); // Roll control
            continuousActions[1] = Input.GetAxisRaw("Vertical"); // Pitch control
            continuousActions[2] = Input.GetKey(KeyCode.Q) ? -1f : (Input.GetKey(KeyCode.E) ? 1f : 0f); // Yaw control
            continuousActions[3] = Input.GetKey(KeyCode.Space) ? 1f : (Input.GetKey(KeyCode.LeftShift) ? -1f : 0f); // Throttle control
        }
#endregion
    }
}