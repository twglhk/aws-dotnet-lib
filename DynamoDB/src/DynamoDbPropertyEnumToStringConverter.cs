using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB에 저장하는 데이터 형식 중, Enum으로 구성되어 있는 형태의 데이터를 변환하는 컨버터
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class DynamoDbPropertyEnumToStringConverter<TEnum> : IPropertyConverter
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Converts a DynamoDBEntry to Enum.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object FromEntry(DynamoDBEntry entry)
        {
            string stringValue = entry.AsString();

            if (Enum.TryParse<TEnum>(stringValue, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Cannot convert '{stringValue}' to {typeof(TEnum).Name}");
        }

        /// <summary>
        /// Converts an Enum to DynamoDBEntry.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DynamoDBEntry ToEntry(object value)
        {
            if (value is TEnum enumValue)
            {
                return new Primitive { Value = enumValue.ToString() };
            }

            throw new ArgumentException($"Invalid type for conversion. Expected {typeof(TEnum).Name}, got {value.GetType().Name}");
        }
    }
}