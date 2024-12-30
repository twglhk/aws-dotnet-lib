using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB에 저장하는 데이터 형식 중, 커스텀 데이터의 List로 구성되어 있는 형태의 데이터를 변환하는 컨버터
    /// </summary>
    public class DynamoDbListPropertyConverter<T> : IPropertyConverter where T : IDocumentable
    {
        /// <summary>
        /// DynamoDB의 데이터를 커스텀 클래스의 리스트로 변환
        /// </summary>
        /// <param name="entry">DynamoDB에서 로드한 데이터</param>
        /// <returns>DB에 저장된 클래스들의 List 반환</returns>
        public object? FromEntry(DynamoDBEntry entry)
        {
            if (entry == null)
                return null;

            DynamoDBList? listEntry = entry as DynamoDBList;
            if (listEntry == null)
                return null;

            List<T> resultList = new List<T>();
            foreach (var item in listEntry.Entries)
            {
                Document? document = item as Document;
                if (document == null)
                    continue;
                T result = Activator.CreateInstance<T>();
                result.FromDocument(document);
                resultList.Add(result);
            }
            return resultList;
        }

        /// <summary>
        /// 커스텀 데이터의 List를 DynamoDB의 Document로 변환 <br/>
        /// List는 DynamoDBEntry의 하위 클래스인 DynamoDBList를 사용
        /// </summary>
        /// <param name="value">변환할 커스텀 데이터의 List</param>
        /// <returns>DynamoDB에 저장될 수 있는 데이터 형태의 인스턴스</returns>
        public DynamoDBEntry? ToEntry(object value)
        {
            if (value == null)
                return null;

            List<T>? list = value as List<T>;
            if (list == null)
                return null;

            DynamoDBList listEntry = new DynamoDBList();
            foreach (var item in list)
            {
                var document = item.ToDocument();
                listEntry.Add(document);
            }

            return listEntry;
        }
    }
}