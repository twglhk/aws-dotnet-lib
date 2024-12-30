using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB에 저장하는 데이터 형식 중, 커스텀 데이터의 Dictionary로 구성되어 있는 형태의 데이터를 변환하는 컨버터
    /// </summary>
    public class DynamoDbDictionaryPropertyConverter<TKey, TValue> : IPropertyConverter where TKey : IConvertible where TValue : IDocumentable
    {
        /// <summary>
        /// DynamoDB의 데이터를 커스텀 클래스의 Dictionary로 변환
        /// </summary>
        /// <param name="entry">DynamoDB에서 로드한 데이터</param>
        /// <returns>DB에 저장된 클래스들의 Dictionary 반환</returns>
        public object? FromEntry(DynamoDBEntry entry)
        {
            if (entry == null)
                return null;

            Document? map = entry as Document;
            if (map == null) return null;

            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            foreach (var item in map)
            {
                TKey key = (TKey)Convert.ChangeType(item.Key, typeof(TKey));
                Document? document = item.Value as Document;
                if (document == null)
                    continue;
                TValue value = Activator.CreateInstance<TValue>();
                value.FromDocument(document);
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 커스텀 데이터의 Dictionary를 DynamoDB의 Document로 변환
        /// </summary>
        /// <param name="value">변환할 커스텀 데이터의 Dictionary</param>
        /// <returns>DynamoDB에 저장될 수 있는 데이터 형태의 인스턴스</returns>
        public DynamoDBEntry? ToEntry(object value)
        {
            if (value == null)
                return null;

            Dictionary<TKey, TValue>? dictionary = value as Dictionary<TKey, TValue>;
            if (dictionary == null)
                return null;

            Document map = new Document();
            foreach (var item in dictionary)
            {
                Document valueDocument = item.Value.ToDocument();
                map.Add(item.Key.ToString(), valueDocument);
            }
            return map;
        }
    }
}

