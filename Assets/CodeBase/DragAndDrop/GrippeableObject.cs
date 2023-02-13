using System;
using UnityEngine;

namespace CodeBase.DragAndDrop
{
    [RequireComponent(typeof(Collider))]
    public class GrippeableObject : MonoBehaviour
    {
        public Action<Transform> StartCaptured;
        public Action StopCaptured;
    }
}