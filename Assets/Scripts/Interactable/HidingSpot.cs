
using UnityEngine;

namespace Interactable
{
    public class HidingSpot : Interactables
    {
        [SerializeField] public Transform[] points =  new Transform[2];
        
        HidingSpot()
        {
            name = "HidingSpot";
        }

        public override void Interact(Controller controller)
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