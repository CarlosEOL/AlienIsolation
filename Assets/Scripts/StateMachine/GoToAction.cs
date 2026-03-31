using NPCs;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Wonder")]
    public class GoToAction : Node
    {
        public float speed;
        public override NodeStatus Execute(NPC npc) 
        {
            if (npc.Target == null) return NodeStatus.Failure;
            
            if (npc.Target != null) 
            {
                npc.agent.SetDestination(npc.Target.position);
                Debug.Log("Going to " + npc.Target.name);

                if (npc.CheckIsInTargetRange())
                {
                    npc.ChangeTarget();
                    Debug.Log("Arrived at  " + npc.Target.name);
                    return NodeStatus.Success;
                }
            }
            
            return NodeStatus.Running;
        }
    }
}