using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IEngine
{
    void InitEngine();
    void UpdateEngine(Rigidbody rb, Drone_Inputs input);
}

