using Amazon.DynamoDBv2.DocumentModel;
namespace WardGames.Web.Dotnet.AWS.DynamoDB
{
    /// <summary>
    /// DynamoDB가 요구하는 Document로 데이터를 변환할 수 있는 interface
    /// </summary>
    public interface IDocumentable
    {
        /// <summary>
        /// DynamoDB의 Document로부터 데이터를 초기화
        /// </summary>
        /// <param name="document">DynamoDB document</param>
        public void FromDocument(Document document);
        
        /// <summary>
        /// DynamoDB의 Document로 데이터를 변환
        /// </summary>
        public Document ToDocument();
    }
}
