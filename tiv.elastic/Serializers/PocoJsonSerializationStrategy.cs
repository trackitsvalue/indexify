using System.Collections.Generic;
using System.Linq;
using SimpleJson;

namespace tiv.elastic.Serializers
{
    public class IgnoreNullValuesJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        protected override bool TrySerializeUnknownTypes(object input, out object output)
        {
            bool returnValue = base.TrySerializeUnknownTypes(input, out output);

            var outputObj = output as IDictionary<string, object>;

            if (outputObj != null)
            {
                output = outputObj.Where(o => o.Value != null).ToDictionary(o => o.Key, o => o.Value);
            }

            return returnValue;
        }
    }
}
