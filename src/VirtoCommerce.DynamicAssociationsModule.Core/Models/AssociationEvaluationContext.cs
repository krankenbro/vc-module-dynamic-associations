using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DynamicAssociationsModule.Core.Model
{
    public class AssociationEvaluationContext : ValueObject
    {
        public string StoreId { get; set; }

        public string[] ProductsToMatch { get; set; }

        public string Group { get; set; }

        public int Take { get; set; } = 20;

        public int Skip { get; set; } = 0;
    }
}
