using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Yuanju.ActuatorData
{
    [CreateAssetMenu(menuName = "Data/ActuatorData")]
    public class ActuatorDataSO : ScriptableObject
    {
        public bool HasRigidBody;
        public bool HasInteractionBehaviour;
        public bool HasHingeJoint;
    }
}

