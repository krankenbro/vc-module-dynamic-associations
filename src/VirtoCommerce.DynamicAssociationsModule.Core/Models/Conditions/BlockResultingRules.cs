using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DynamicAssociationsModule.Core.Model.Conditions
{
    public class BlockResultingRules : BlockConditionRules
    {
        public virtual Dictionary<string, string[]> GetPropertyValues()
        {
            var result = new Dictionary<string, string[]>();

            if (!Children.IsNullOrEmpty())
            {
                result = Children
                    .OfType<ConditionPropertyValues>()
                    .SelectMany(x => x.GetPropertiesValues())
                    .ToDictionary(x => x.Key, y => y.Value);
            }

            return result;
        }

        public virtual ICollection<string> GetCategoryIds()
        {
            var result = new List<string>();

            if (!Children.IsNullOrEmpty())
            {
                result = Children
                    .OfType<ConditionProductCategory>()
                    .SelectMany(x => x.CategoryIds).ToList();
            }

            return result;
        }
    }
}
