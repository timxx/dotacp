using dotacp.protocol;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;

namespace dotacp.shared
{
    internal class JsonRpcEx : JsonRpc
    {
        public JsonRpcEx(IJsonRpcMessageHandler handler)
            : base(handler)
        {
        }

        protected override JsonRpcError.ErrorDetail CreateErrorDetails(JsonRpcRequest request, Exception exception)
        {
            if (exception is AcpException acpEx)
            {
                return new JsonRpcError.ErrorDetail
                {
                    Code = (JsonRpcErrorCode)acpEx.Code,
                    Message = acpEx.Message,
                    Data = acpEx.ErrorData,
                };
            }

            return base.CreateErrorDetails(request, exception);
        }
    }
}
