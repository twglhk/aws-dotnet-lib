using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB에 저장하는 데이터 형식 중, enum, value 쌍으로 되어 있는 Dictioanry 형태의 데이터를 Map으로 변환하는 컨버터
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DynamoDbEnumDictionaryPropertyConverter<TEnum, TValue> : IPropertyConverter
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Converts a DynamoDBEntry object to a Dictionary object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DynamoDBEntry ToEntry(object value)
        {
            var dictionary = value as Dictionary<TEnum, TValue>;
            if (dictionary == null) throw new ArgumentOutOfRangeException(nameof(value));

            var dynamoDBMap = new Document();
            foreach (var kvp in dictionary)
            {
                var valueEntry = DynamoDBEntryConversion.V2.ConvertToEntry(typeof(TValue), kvp.Value);
                dynamoDBMap.Add(kvp.Key.ToString(), valueEntry);
            }

            return dynamoDBMap;
        }

        /// <summary>
        /// Converts a Dictionary object to a DynamoDBEntry object.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object FromEntry(DynamoDBEntry entry)
        {
            var map = entry.AsDocument();
            var dictionary = new Dictionary<TEnum, TValue>();

            foreach (var kvp in map)
            {
                var enumKey = (TEnum)Enum.Parse(typeof(TEnum), kvp.Key);
                var value = DynamoDBEntryConversion.V2.ConvertFromEntry<TValue>(kvp.Value);
                dictionary.Add(enumKey, value);
            }

            return dictionary;
        }
    }
}