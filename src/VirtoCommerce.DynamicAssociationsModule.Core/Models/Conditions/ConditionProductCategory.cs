using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.DynamicAssociationsModule.Core.Model.Conditions
{
    public class ConditionProductCategory : DynamicAssociationTree
    {
        public string[] CategoryIds { get; set; }

        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is DynamicAssociationExpressionEvaluationContext evaluationContext)
            {
                result = evaluationContext.AreItemsInCategory(CategoryIds);
            }

            return result;
        }
    }
}
