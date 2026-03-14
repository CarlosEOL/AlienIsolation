
using UnityEngine;

namespace Interactable
{
    public abstract class Interactables : MonoBehaviour
    {
        public string name;
        public abstract void Interact(Controller controller);

        public string GetName()
        {
            return name;
        }
    }
}