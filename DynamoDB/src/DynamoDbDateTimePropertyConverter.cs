using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB에 저장하는 데이터 형식 중, DateTime으로 구성되어 있는 형태의 데이터를 변환하는 컨버터
    /// </summary>
    public class DynamoDbDateTimePropertyConverter : IPropertyConverter
    {
        /// <summary>
        /// Converts a DateTime object to a DynamoDBEntry object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DynamoDBEntry ToEntry(object value)
        {
            if (value is not DateTime dateTimeValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be a DateTime object.");
            }

            string dateTimeString = dateTimeValue.ToUniversalTime().ToString("o");
            return new Primitive { Value = dateTimeString };
        }

        /// <summary>
        /// Converts a DynamoDBEntry object to a DateTime object.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object FromEntry(DynamoDBEntry entry)
        {
            if (entry is not Primitive primitive || !(primitive.Value is string strValue) || string.IsNullOrEmpty(strValue))
            {
                throw new ArgumentOutOfRangeException(nameof(entry), "Entry must be a non-empty string.");
            }

            if (DateTime.TryParse(strValue, out DateTime dateTimeValue))
            {
                return dateTimeValue.ToUniversalTime();
            }
            else
            {
                throw new FormatException("Failed to parse DateTime from the provided string.");
            }
        }
    }
}

