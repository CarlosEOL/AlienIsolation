
/// <summary>
/// A base reference for all decisions in the tree. 
/// Any time a new 'type' of decision needs to be made (e.g. multiple choice, true/false, etc), it will inherit from this.
/// </summary>

namespace StateMachine
{
    public abstract class AbstractDecision
    {
        public abstract void MakeDecision();
    }
}
