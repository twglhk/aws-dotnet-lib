
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using WardGames.Web.Dotnet.Http;
using WardGames.Web.Dotnet.WebSocket;

namespace WardGames.Web.Dotnet.AWS.Lambda.APIGatewayEvents
{
    /// <summary>
    /// AWS Lambda에서 APIGatway 중 로 들어온 Request/Response를 처리하는 핸들러들의 base class <br/>
    /// </summary>
    /// <typeparam name="WssRequest"></typeparam>
    /// <typeparam name="WssResponse"></typeparam>
    public abstract class WssRequestHandlerBase<WssRequest, WssResponse> : ApiRequestHandlerBase<WssRequest, WssResponse>
    {
        private string _action;

#pragma warning disable CS8618 // Non-nullable field is initialized through InitializeAction.
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected WssRequestHandlerBase(APIGatewayProxyRequest request, string action) : base(request)
        {
            InitializeAction(action);
        }

        /// <summary>
        /// Default constructor (DynamoDB 초기화 전용)
        /// </summary>
        /// <param name="dynamoDB"></param>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected WssRequestHandlerBase(IAmazonDynamoDB dynamoDB, APIGatewayProxyRequest request, string action) : base(dynamoDB, request)
        {
            InitializeAction(action);
        }
#pragma warning restore CS8618

        private void InitializeAction(string action)
        {
            if (string.IsNullOrEmpty(action))
                throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        /// <summary>
        /// 성공 WebSocketResponse를 생성
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override ApiResponse<WssResponse> CreateSuccessApiResponse(WssResponse data)
        {
            return new WebSocketResponse<WssResponse>(_action, data).GenerateSuccessResponse();
        }

        /// <summary>
        /// 실패 WebSocketResponse를 생성
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        protected override ApiResponse<WssResponse> CreateFailApiResponse(string? errMsg)
        {
            return new WebSocketResponse<WssResponse>(_action, default!).GenerateFailedResponse(errMsg);
        }
    }
}