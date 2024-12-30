using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
namespace WardGames.Web.Dotnet.AWS.Lambda.APIGatewayEvents;

/// <summary>
/// Lambda에서 APIGatewayEvents를 처리할 때 유용하게 사용할 수 있는 Helper 함수를 모아둔 클래스
/// </summary>
public class APIGatewayEventHelper
{
    /// <summary>
    /// lambda의 입력으로 APIGatewayProxyRequest를 받을 때, 예외 검사 후 Body를 Deserialize하여 반환하는 Helper
    /// </summary>
    /// <param name="request">APIGatewayProxyRequest 요청</param>
    /// <typeparam name="T">반환 클래스</typeparam>
    /// <returns>Request의 body</returns>
    public static T GetApiRequestBody<T>(APIGatewayProxyRequest request)
    {
        if (request == null)
            throw new ArgumentNullException("APIGatewayProxyRequest is null");

        if (string.IsNullOrEmpty(request.Body))
            throw new ArgumentNullException("APIGatewayProxyRequest/body is null");

        T? requestBody = JsonConvert.DeserializeObject<T>(request.Body);
        if (requestBody == null)
            throw new ArgumentException("Deserialization resulted in a null");

        return requestBody;
    }

    /// <summary>
    /// APIGatewayProxyResponse 응답 생성
    /// </summary>
    /// <param name="data">응답에 포함할 데이터</param>
    /// <param name="statusCode">응답 상태 코드</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public static APIGatewayProxyResponse GenerateResponse(object data, int statusCode)
    {
        return new APIGatewayProxyResponse
        {
            Body = JsonConvert.SerializeObject(data),
            StatusCode = statusCode,
        };
    }
}
