using NPCs;

namespace StateMachine
{
    public enum NodeStatus { Running, Success, Failure }
    
    public interface IAction
    {
        NodeStatus Execute(NPC npc);
    }
}