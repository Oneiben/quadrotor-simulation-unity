using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XeroxUAV
{
    [RequireComponent(typeof(Rigidbody))] // Ensures the GameObject has a Rigidbody component
    public class Base_RigidBody : MonoBehaviour
    {
        #region Variables
        [Header("Rigidbody Properties")]
        const float lbsTokg = 0.78f; // Conversion factor from pounds to kilograms
        [SerializeField] private float weighTnbls = 1f; // Weight of the object in pounds

        protected Rigidbody rb; // Rigidbody component reference
        protected float startDrag; // Initial drag value
        protected float startAngularDrag; // Initial angular drag value
        #endregion

        #region Main Methods

        void Awake()
        {
            rb = GetComponent<Rigidbody>(); // Get the Rigidbody component

            if (rb)
            { 
                // Set the mass of the Rigidbody based on weight
                rb.mass = weighTnbls * lbsTokg; 
                startDrag = rb.linearDamping; // Store initial drag value
                startAngularDrag = rb.angularDamping; // Store initial angular drag value
            }
        }

        // FixedUpdate is called on a fixed timer, suitable for physics calculations
        void FixedUpdate()
        {
            if (!rb)
            {
                return; // Exit if Rigidbody is not found
            }
            HandlePhysics(); // Call the method to handle physics
        }

        #endregion

        #region Custom Methods
        // Virtual method for handling physics logic, can be overridden in derived classes
        protected virtual void HandlePhysics() 
        { 
            // Logic for base class physics handling
        }
        #endregion
    }
}
