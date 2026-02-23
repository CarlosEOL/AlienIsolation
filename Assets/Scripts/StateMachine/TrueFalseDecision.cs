
namespace StateMachine
{
    

    /// <summary>
    /// A true/false decision, evaluates a single boolean.
    /// </summary>
    public class TrueFalseDecision : AbstractDecision
    {
        /// <summary>
        /// In case this is your first time seeing it, this is how we say 'Any Function Goes Here' in C#.
        /// We can create an object of type BooleanFunction now, and assign it a function to call later.
        /// The function we call must match its return type.
        /// </summary>
        public delegate bool BooleanFunction();

        private BooleanFunction toEvaluate;
        private AbstractDecision trueNode;
        private AbstractDecision falseNode;

        public TrueFalseDecision(AbstractDecision trueNode, AbstractDecision falseNode, BooleanFunction toEvaluate)
        {
            this.trueNode = trueNode;
            this.falseNode = falseNode;
            this.toEvaluate = toEvaluate;
        }

        public override void MakeDecision()
        {
            bool evaluationIsTrue = toEvaluate();

            if (evaluationIsTrue)
            {
                trueNode.MakeDecision();
            }
            else
            {
                falseNode.MakeDecision();
            }
        }
    }

}