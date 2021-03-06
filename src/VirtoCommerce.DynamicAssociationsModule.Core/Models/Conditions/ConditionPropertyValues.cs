using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DynamicAssociationsModule.Core.Model.Conditions
{
    public class ConditionPropertyValues : ConditionTree
    {
        public Property[] Properties { get; set; }

        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is AssociationExpressionEvaluationContext evaluationContext)
            {
                result = evaluationContext.AreItemPropertyValuesEqual(GetPropertiesValues());
            }

            return result;
        }

        public virtual Dictionary<string, string[]> GetPropertiesValues()
        {
            var result = new Dictionary<string, string[]>();

            if (!Properties.IsNullOrEmpty())
            {
                foreach (var property in Properties)
                {
                    result.Add(property.Name,
                        property.Values
                            .Where(x => x != null)
                            .Select(x => x.Value.ToString())
                            .ToArray());
                }
            }

            return result;
        }
    }
}
