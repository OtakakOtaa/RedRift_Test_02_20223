using UnityEngine;

namespace CodeBase.DragAndDrop
{
    public class Gripper
    {
        private GrippeableObject _gripedObject;
        private bool _isAlreadyGriped = false;
        
        public void Update()
        {
            if (!_isAlreadyGriped && Input.GetMouseButton(0) && IsCursorPointedAtObject(out var gripedObject))
            {
                _gripedObject = gripedObject;
                _isAlreadyGriped = true;
                _gripedObject.StartCaptured?.Invoke(_gripedObject.transform);
            }
            else if(_isAlreadyGriped && !Input.GetMouseButton(0))
            {
                _isAlreadyGriped = false;
                _gripedObject.StopCaptured?.Invoke();
            }

            if (!_isAlreadyGriped) return;
            
            var transform = _gripedObject.transform;
            transform.position = new Vector3(Cusror.x, Cusror.y, transform.position.z);
        }

        private bool IsCursorPointedAtObject(out GrippeableObject gripedObject)
        {
            gripedObject = null;
            var screenPointPosition = Cusror;
            var cameraTransform = Camera.main!.transform;

            bool isNotMiss = Physics.Raycast(screenPointPosition, cameraTransform.forward, out var hit);
            if (!isNotMiss) return false;
            
            hit.transform.TryGetComponent(out gripedObject);
            return gripedObject is not null;
        }
        
        private Vector3 Cusror => Camera.main!.ScreenToWorldPoint(Input.mousePosition);
    }
}