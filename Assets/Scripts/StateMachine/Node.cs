namespace StateMachine
{
    public struct Node : IAction
    {
        IAction[] Actions;

        void IAction.ExecuteAction()
        {
            foreach (IAction action in Actions)
            {
                action.ExecuteAction();
            }
        }
    }
}