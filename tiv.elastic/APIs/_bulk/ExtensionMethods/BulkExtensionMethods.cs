using System;
using System.Text;
using tiv.elasticClient.APIs.Index.Enums;
using tiv.elasticClient.ExtensionFunctions;

namespace tiv.elasticClient.APIs._bulk.ExtensionMethods
{
    public static class BulkHelper
    {
        public static void AddHitToBulkOperation(this StringBuilder bulkOperation, string action, string document = "", string _index = "", string _type = "", string _id = "")
        {
            if (document.IsNullOrEmpty() && !action.CompareNoCase(IndexDocumentActionType.Delete)) throw new ArgumentException($"Document context must always be specifified unless action type is {IndexDocumentActionType.Delete}");
            if (action.CompareWithCase(IndexDocumentActionType.Delete) && (_index.IsNullOrEmpty() || _type.IsNullOrEmpty() || _id.IsNullOrEmpty()) ) throw new ArgumentException($"{nameof(_index)}, {nameof(_type)}, and {nameof(_id)} must be specified when action type is {IndexDocumentActionType.Delete}");
            
            bulkOperation.Append($@"{{ ""{action}"" : {{ ");

            var index = $@" ""{nameof(_index)}"" : ""{_index}"" ";
            var type = $@" ""{nameof(_type)}"" : ""{_type}"" ";
            var id = $@" ""{nameof(_id)}"" : ""{_id}"" ";
            var needsComma = false;

            // Index should be specified on the URL.
            if (!_index.IsNullOrEmpty())
            {
                bulkOperation.Append(index);
                needsComma = true;
            }

            // Document type should be specified on the URL.
            if (!_type.IsNullOrEmpty())
            {
                if (needsComma) bulkOperation.Append(", ");
                bulkOperation.Append(type);
                needsComma = true;
            }

            // ID will be specified, index and create don't need IDs as they auto generate.
            if(!_id.IsNullOrEmpty())
            {
                if (needsComma) bulkOperation.Append(", ");
                bulkOperation.Append(id);
            }

            bulkOperation.AppendLine(" }}");

            if (!document.IsNullOrEmpty() && !action.CompareNoCase(IndexDocumentActionType.Delete)) bulkOperation.AppendLine(document);

        }
    }
}
