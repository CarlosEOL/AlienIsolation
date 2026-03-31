namespace StateMachine
{
    public interface IStateAndGoals
    {
        public enum NPCState
        {
            Idle,
            Hunt,
            Pursue,
            Investigate,
            Hide,
            Wander
        }

        public enum NPCGoals
        {
            Idle,
            FindAndKill,
            Protect,
            Lead,
            Wander,
        }
    }
}