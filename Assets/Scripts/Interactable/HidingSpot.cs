
using UnityEngine;

namespace Interactable
{
    public class HidingSpot : Interactables, IInteractables
    {
        [SerializeField] public Transform[] points = new Transform[2];

        public override void Interact(Controller controller)
        {
            base.Interact(controller);
            LockerFunction(controller);
        }

        void LockerFunction(Controller controller)
        {
            if (!controller.isHiding)
            {
                controller.isHiding = true;
                Debug.Log("Debug: Hidden");
                controller.transform.position = points[0].position;
            }
            else
            {
                controller.isHiding = false;
                Debug.Log("Debug: Not hidden");
                controller.transform.position = points[1].position;
            }
        }
    }
}