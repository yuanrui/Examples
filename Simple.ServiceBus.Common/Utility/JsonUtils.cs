using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Simple.ServiceBus.Utility
{
    public static class JsonUtils
    {
        static JsonUtils()
        {
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                var setting = new JsonSerializerSettings();
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                setting.ContractResolver = new AllPropertyResolver();

                return setting;
            });
        }

        public static string Serialize<T>(T obj)
        {
            return Serialize(obj);
        }

        public static string Serialize<T>(T obj, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(obj, converters);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        internal class AllPropertyResolver : DefaultContractResolver
        {
            static MemberSerialization ResetMemberSerialization(Type type, MemberSerialization memberSerialization)
            {
                if (memberSerialization == MemberSerialization.OptIn)
                {
                    memberSerialization = MemberSerialization.OptOut;
                }

                return memberSerialization;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);

                if (jsonProperty.Ignored)
                {
                    var attrs = (IgnoreDataMemberAttribute[])member.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        jsonProperty.Ignored = false;
                    }
                }

                return jsonProperty;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, ResetMemberSerialization(type, memberSerialization));
                return properties;
            }

            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                var contract = base.CreateObjectContract(objectType);
                contract.MemberSerialization = ResetMemberSerialization(objectType, contract.MemberSerialization);
                return contract;
            }
        }
    }
}
