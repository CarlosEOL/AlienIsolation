using NPCs;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "State Machine/Actions/Wander")]
    public class WanderAction : Node
    {
        public float speed;
        public override NodeStatus Execute(NPC npc) 
        {
            Debug.Log("WANDER WANDER!!");
            if (npc.Target == null)
            {
                Debug.Log("Wander FAILED!");
                return NodeStatus.Failure;
            }
            
            if (npc.Target != null & !npc.CheckTargetTag())
            {
                npc.agent.SetDestination(npc.Target.position);
                npc.agent.speed = speed;

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