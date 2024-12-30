
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using WardGames.Web.Dotnet.Http;
using System.Net;

namespace WardGames.Web.Dotnet.AWS.Lambda.APIGatewayEvents
{
    /// <summary>
    /// AWS Lambda에서 APIGatway로 들어온 Request/Response를 처리하는 핸들러들의 base class <br/>
    /// 기본 요청/응답 프로토콜은 HTTP <br/>
    /// 단, 요청 처리의 경우 APIGatewayProxyRequest를 받아서 처리하고, 응답 처리의 경우 APIGatewayProxyResponse를 반환한다. <br/>
    /// </summary>
    /// <typeparam name="TRequest">ApiRequestBase 내부 Request 데이터 타입</typeparam>
    /// <typeparam name="TResponse">ApiResponseBase 내부 Response 데이터 타입</typeparam>
    public abstract class ApiRequestHandlerBase<TRequest, TResponse>
    {
        /// <summary>
        /// DynamoDb client (사용할 경우에만 초기화)
        /// </summary>
        protected readonly IAmazonDynamoDB? _dynamoDB;

        /// <summary>
        /// 요청 데이터
        /// </summary>
        protected readonly TRequest _requestData;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="request"></param>
        protected ApiRequestHandlerBase(APIGatewayProxyRequest request)
        {
            ApiRequest<TRequest> apiRequest = APIGatewayEventHelper.GetApiRequestBody<ApiRequest<TRequest>>(request);
            _requestData = apiRequest.Data;;
        }

        /// <summary>
        /// Default constructor (DynamoDB 초기화 전용)
        /// </summary>
        /// <param name="dynamoDB">DynamoDb client</param>
        /// <param name="request">APIGatewayProxyRequest.</param>
        protected ApiRequestHandlerBase(IAmazonDynamoDB dynamoDB, APIGatewayProxyRequest request) : this(request)
        {
            _dynamoDB = dynamoDB;
        }

        /// <summary>
        /// DynamoDb 관련 요청에 맞는 Response를 생성
        /// </summary>
        /// <returns>완전한 APIGatewayProxyResponse 응답 반환</returns>
        public virtual async Task<APIGatewayProxyResponse> CreateResponse()
        {
            ApiResponse<TResponse> packedResponse;
            HttpStatusCode statusCode;

            try
            {
                TResponse response = await GenerateResponseTask();
                packedResponse = CreateSuccessApiResponse(response);
                statusCode = HttpStatusCode.OK;
            }
            catch (ApiException e)
            {
                packedResponse = CreateFailApiResponse(e.Message);
                statusCode = HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                packedResponse = CreateFailApiResponse(e.Message);
                statusCode = HttpStatusCode.InternalServerError;
            }
            
            return APIGatewayEventHelper.GenerateResponse(packedResponse, (int)statusCode);
        }

        /// <summary>
        /// 응답 생성을 수행하는 메서드 (서브 클래스에서 구체화)
        /// </summary>
        /// <returns></returns>
        protected virtual Task<TResponse> GenerateResponseTask()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 성공 응답 생성
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual ApiResponse<TResponse> CreateSuccessApiResponse(TResponse data)
        {
            return new ApiResponse<TResponse>(data).GenerateSuccessResponse();
        }

        /// <summary>
        /// 실패 응답 생성
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        protected virtual ApiResponse<TResponse> CreateFailApiResponse(string? errMsg)
        {
            return new ApiResponse<TResponse>(default!).GenerateFailedResponse(errMsg);
        }
    }
}