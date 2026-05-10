using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dotacp.generator
{
    /// <summary>
    /// Generates IAcpAgent, IAcpClient, AgentRpcTarget, ClientRpcTarget, and Connection classes from meta.json and schema.json
    /// </summary>
    public class InterfaceGenerator
    {
        private class MethodInfo
        {
            public string MethodPath { get; set; } = string.Empty;
            public string MethodName { get; set; } = string.Empty;
            public string RequestType { get; set; } = string.Empty;
            public string ResponseType { get; set; } = string.Empty;
            public bool IsNotification { get; set; }
            public string Description { get; set; }
        }

        private Dictionary<string, MethodInfo> _agentMethods = new Dictionary<string, MethodInfo>();
        private Dictionary<string, MethodInfo> _clientMethods = new Dictionary<string, MethodInfo>();

        public void Generate(string metaJsonPath, string schemaJsonPath, string versionFilePath, string agentDir, string clientDir,
            string protocolNamespace = "dotacp.protocol", string agentNamespace = "dotacp.agent",
            string clientNamespace = "dotacp.client")
        {
            ParseMetaAndSchema(metaJsonPath, schemaJsonPath);

            var gitRef = "";
            if (File.Exists(versionFilePath))
            {
                gitRef = File.ReadAllText(versionFilePath).Trim();
            }

            // Generate agent files
            GenerateIAcpAgent(Path.Combine(agentDir, "IAcpAgent.cs"), gitRef, protocolNamespace, agentNamespace);
            GenerateAgentRpcTarget(Path.Combine(agentDir, "AgentRpcTarget.cs"), gitRef, protocolNamespace, agentNamespace);
            GenerateAgentConnection(Path.Combine(agentDir, "Connection.cs"), gitRef, protocolNamespace, agentNamespace);

            // Generate client files
            GenerateIAcpClient(Path.Combine(clientDir, "IAcpClient.cs"), gitRef, protocolNamespace, clientNamespace);
            GenerateClientRpcTarget(Path.Combine(clientDir, "ClientRpcTarget.cs"), gitRef, protocolNamespace, clientNamespace);
            GenerateClientConnection(Path.Combine(clientDir, "Connection.cs"), gitRef, protocolNamespace, clientNamespace);
        }

        private void ParseMetaAndSchema(string metaJsonPath, string schemaJsonPath)
        {
            var metaJson = File.ReadAllText(metaJsonPath);
            var meta = JObject.Parse(metaJson);

            var schemaJson = File.ReadAllText(schemaJsonPath);
            var schema = JObject.Parse(schemaJson);

            // Parse agent methods from meta.json
            var agentMethods = meta["agentMethods"] as JObject;
            if (agentMethods != null)
            {
                foreach (var prop in agentMethods.Properties())
                {
                    var methodPath = prop.Value.ToString();
                    _agentMethods[methodPath] = new MethodInfo
                    {
                        MethodPath = methodPath,
                        MethodName = NamingHelper.ConvertToPascalCase(prop.Name)
                    };
                }
            }

            // Parse client methods from meta.json
            var clientMethods = meta["clientMethods"] as JObject;
            if (clientMethods != null)
            {
                foreach (var prop in clientMethods.Properties())
                {
                    var methodPath = prop.Value.ToString();
                    _clientMethods[methodPath] = new MethodInfo
                    {
                        MethodPath = methodPath,
                        MethodName = NamingHelper.ConvertToPascalCase(prop.Name)
                    };
                }
            }

            // Parse schema to find request/response types
            var defs = schema["$defs"] as JObject;
            if (defs != null)
            {
                foreach (var def in defs.Properties())
                {
                    var defObj = def.Value as JObject;
                    if (defObj == null)
                        continue;

                    var xMethod = defObj["x-method"]?.ToString();
                    var xSide = defObj["x-side"]?.ToString();
                    if (string.IsNullOrEmpty(xMethod) || string.IsNullOrEmpty(xSide))
                        continue;

                    var typeName = def.Name;
                    var description = defObj["description"]?.ToString();

                    // Determine if it's a request or response/notification
                    bool isRequest = typeName.EndsWith("Request");
                    bool isNotification = typeName.EndsWith("Notification");

                    if (xSide == "agent")
                    {
                        if (_agentMethods.TryGetValue(xMethod, out var methodInfo))
                        {
                            if (isRequest)
                            {
                                methodInfo.RequestType = typeName;
                                methodInfo.Description = description;
                            }
                            else if (isNotification)
                            {
                                // For notifications, the notification type is the "request" (parameter)
                                methodInfo.RequestType = typeName;
                                methodInfo.ResponseType = typeName; // Set to same to pass validation
                                methodInfo.IsNotification = true;
                                methodInfo.Description = description;
                            }
                            else
                            {
                                methodInfo.ResponseType = typeName;
                            }
                        }
                    }
                    else if (xSide == "client")
                    {
                        if (_clientMethods.TryGetValue(xMethod, out var methodInfo))
                        {
                            if (isRequest)
                            {
                                methodInfo.RequestType = typeName;
                                methodInfo.Description = description;
                            }
                            else if (isNotification)
                            {
                                // For notifications, the notification type is the "request" (parameter)
                                methodInfo.RequestType = typeName;
                                methodInfo.ResponseType = typeName; // Set to same to pass validation
                                methodInfo.IsNotification = true;
                                methodInfo.Description = description;
                            }
                            else
                            {
                                methodInfo.ResponseType = typeName;
                            }
                        }
                    }
                }
            }
        }

        private void GenerateIAcpAgent(string outputPath, string gitRef, string protocolNamespace, string agentNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {agentNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Defines the methods an ACP agent implementation must provide to handle protocol requests.");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public interface IAcpAgent");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Called after the RPC connection is established.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"connection\">The active connection that can be used for outbound calls to the client.</param>");
            sb.AppendLineLf("        void OnClientConnected(Connection connection);");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Called when the RPC connection is disconnected.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"connection\">The connection that was disconnected.</param>");
            sb.AppendLineLf("        void OnDisconnected(Connection connection);");
            sb.AppendLineLf();

            // Sort methods by name for consistent output
            foreach (var method in _agentMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;

                var methodName = GetMethodName(method);
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf("        /// <summary>");
                sb.AppendLineLf($"        /// Handles the protocol <c>{method.MethodPath}</c> {(method.IsNotification ? "notification" : "request")}.");
                sb.AppendLineLf("        /// </summary>");
                sb.AppendLineLf($"        /// <param name=\"{(method.IsNotification ? "notification" : "request")}\">The {(method.IsNotification ? "notification" : "request")} payload.</param>");
                sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels request processing.</param>");
                sb.AppendLineLf($"        /// <returns>{(method.IsNotification ? "A task that completes when handling is finished." : "The response.")}</returns>");
                sb.AppendLineLf($"        {returnType} {methodName}Async({method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default);");
                sb.AppendLineLf();
            }

            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Handles an extension method call that is not part of the core protocol.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension method name.</param>");
            sb.AppendLineLf("        /// <param name=\"request\">The extension request payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels request processing.</param>");
            sb.AppendLineLf("        /// <returns>The extension method response object.</returns>");
            sb.AppendLineLf("        Task<object> ExtMethodAsync(string method, object request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default);");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Handles an extension notification that is not part of the core protocol.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension notification name.</param>");
            sb.AppendLineLf("        /// <param name=\"notification\">The notification payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels notification handling.</param>");
            sb.AppendLineLf("        /// <returns>A task that completes when handling is finished.</returns>");
            sb.AppendLineLf("        Task ExtNotificationAsync(string method, object notification,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default);");
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private void GenerateAgentRpcTarget(string outputPath, string gitRef, string protocolNamespace, string agentNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using dotacp.shared;");
            sb.AppendLineLf("using StreamJsonRpc;");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {agentNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    internal sealed class AgentRpcTarget");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        private readonly IAcpAgent _agent;");
            sb.AppendLineLf();
            sb.AppendLineLf("        public AgentRpcTarget(IAcpAgent agent)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            _agent = agent;");
            sb.AppendLineLf("        }");

            // Sort methods by name for consistent output
            var agentTargetMethodNames = ResolveRpcTargetMethodNames(_agentMethods.Values);
            foreach (var method in _agentMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;

                var interfaceMethodName = GetMethodName(method);
                var methodName = agentTargetMethodNames[method.MethodPath];
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf();
                sb.AppendLineLf($"        [JsonRpcMethod(AgentMethods.{method.MethodName}, UseSingleObjectParameterDeserialization = true)]");
                sb.AppendLineLf($"        public {returnType} {methodName}Async(");
                sb.AppendLineLf($"            {method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default)");
                sb.AppendLineLf("        {");
                sb.AppendLineLf($"            return _agent.{interfaceMethodName}Async({(method.IsNotification ? "notification" : "request")}, cancellationToken);");
                sb.AppendLineLf("        }");
            }

            sb.AppendLineLf();
            sb.AppendLineLf("        [JsonRpcMethod(\"__acp_ext_method__\", UseSingleObjectParameterDeserialization = true)]");
            sb.AppendLineLf("        public Task<object> HandleExtensionMethodAsync(");
            sb.AppendLineLf("            ExtensionRequest request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _agent.ExtMethodAsync(request.Method, request.Arguments, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        [JsonRpcMethod(\"__acp_ext_notification__\", UseSingleObjectParameterDeserialization = true)]");
            sb.AppendLineLf("        public Task HandleExtensionNotificationAsync(");
            sb.AppendLineLf("            ExtensionRequest request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _agent.ExtNotificationAsync(request.Method, request.Arguments, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private void GenerateAgentConnection(string outputPath, string gitRef, string protocolNamespace, string agentNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using dotacp.shared;");
            sb.AppendLineLf("using StreamJsonRpc;");
            sb.AppendLineLf("using System;");
            sb.AppendLineLf("using System.Diagnostics;");
            sb.AppendLineLf("using System.IO;");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {agentNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Manages a JSON-RPC connection between an ACP agent and an ACP client.");
            sb.AppendLineLf("    /// The agent can use this connection to communicate with the Client so it behaves like a Client.");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public class Connection : IDisposable");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        private JsonRpc _rpc;");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Gets a task that completes when the underlying RPC channel is closed.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        public Task Completion => _rpc.Completion;");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Connection(IAcpAgent agent, Stream inputStream, Stream outputStream,");
            sb.AppendLineLf("            TraceSource? traceSource = null)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            var handler = new NewLineDelimitedMessageHandler(");
            sb.AppendLineLf("                inputStream, outputStream, new JsonMessageFormatter());");
            sb.AppendLineLf("            var routingHandler = new ExtensionMethodRoutingMessageHandler(handler);");
            sb.AppendLineLf("            _rpc = new JsonRpcEx(routingHandler);");
            sb.AppendLineLf("            if (traceSource != null)");
            sb.AppendLineLf("                _rpc.TraceSource = traceSource;");
            sb.AppendLineLf();
            sb.AppendLineLf("            _rpc.AddLocalRpcTarget(new AgentRpcTarget(agent));");
            sb.AppendLineLf("            _rpc.StartListening();");
            sb.AppendLineLf();
            sb.AppendLineLf("            _rpc.Disconnected += (sender, e) => agent.OnDisconnected(this);");
            sb.AppendLineLf();
            sb.AppendLineLf("            agent.OnClientConnected(this);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Task<TResponse> SendRequestAsync<TRequest, TResponse>(");
            sb.AppendLineLf("            string method, TRequest request, CancellationToken cancellationToken)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _rpc.InvokeWithParameterObjectAsync<TResponse>(");
            sb.AppendLineLf("                method, request, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Task SendNotificationAsync<TNotification>(");
            sb.AppendLineLf("            string method, TNotification notification, CancellationToken cancellationToken)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            cancellationToken.ThrowIfCancellationRequested();");
            sb.AppendLineLf("            return _rpc.NotifyWithParameterObjectAsync(method, notification);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Create a Connection to an ACP client over the given streams.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"agent\">The agent implementation that handles incoming RPC calls.</param>");
            sb.AppendLineLf("        /// <param name=\"inputStream\">The (client) input stream to write to.</param>");
            sb.AppendLineLf("        /// <param name=\"outputStream\">The (client) output stream to read from.</param>");
            sb.AppendLineLf("        /// <param name=\"traceSource\">Optional trace source used for StreamJsonRpc diagnostics.</param>");
            sb.AppendLineLf("        /// <returns>");
            sb.AppendLineLf("        /// A running <see cref=\"Connection\"/> instance, or <see langword=\"null\"/> when a required argument is <see langword=\"null\"/>.");
            sb.AppendLineLf("        /// </returns>");
            sb.AppendLineLf("        public static Connection? RunAgent(IAcpAgent agent,");
            sb.AppendLineLf("            Stream inputStream, Stream outputStream,");
            sb.AppendLineLf("            TraceSource? traceSource = null)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            if (agent == null || inputStream == null || outputStream == null)");
            sb.AppendLineLf("                return null;");
            sb.AppendLineLf();
            sb.AppendLineLf("            return new Connection(agent, inputStream, outputStream, traceSource);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();

            // Generate client method calls
            foreach (var method in _clientMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;

                var methodName = GetMethodName(method);
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf("        /// <summary>");
                sb.AppendLineLf($"        /// {(method.IsNotification ? "Sends" : "Calls")} the client <c>{method.MethodPath}</c> {(method.IsNotification ? "notification" : "method")}.");
                sb.AppendLineLf("        /// </summary>");
                sb.AppendLineLf($"        /// <param name=\"{(method.IsNotification ? "notification" : "request")}\">The {(method.IsNotification ? "notification" : "request")} payload.</param>");
                sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels the operation.</param>");
                sb.AppendLineLf($"        /// <returns>{(method.IsNotification ? "A task that completes when the notification is sent." : "The response.")}</returns>");
                sb.AppendLineLf($"        public {returnType} {methodName}Async(");
                sb.AppendLineLf($"            {method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default)");
                sb.AppendLineLf("        {");
                if (method.IsNotification)
                {
                    sb.AppendLineLf($"            return SendNotificationAsync(ClientMethods.{method.MethodName}, notification, cancellationToken);");
                }
                else
                {
                    sb.AppendLineLf($"            return SendRequestAsync<{method.RequestType}, {method.ResponseType}>(");
                    sb.AppendLineLf($"                ClientMethods.{method.MethodName}, request, cancellationToken);");
                }
                sb.AppendLineLf("        }");
                sb.AppendLineLf();
            }

            GenerateCommonMethods(sb, true);
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private void GenerateCommonMethods(StringBuilder sb, bool isAgent)
        {
            var side = isAgent ? "a client" : "an agent";
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf($"        /// Calls {side} extension method.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension method name.</param>");
            sb.AppendLineLf("        /// <param name=\"request\">The request payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels the operation.</param>");
            sb.AppendLineLf("        /// <returns>The response object.</returns>");
            sb.AppendLineLf("        public Task<object> ExtMethodAsync(string method, object request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return SendRequestAsync<object, object>(");
            sb.AppendLineLf("                \"_\" + method, request, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf($"        /// Sends {side} extension notification.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension notification name.</param>");
            sb.AppendLineLf("        /// <param name=\"notification\">The notification payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels the operation.</param>");
            sb.AppendLineLf("        /// <returns>A task that completes when the notification is sent.</returns>");
            sb.AppendLineLf("        public Task ExtNotificationAsync(string method, object notification,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return SendNotificationAsync(");
            sb.AppendLineLf("                \"_\" + method, notification, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Releases all resources used by the current instance of the class.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        public void Dispose()");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            _rpc.Dispose();");
            sb.AppendLineLf("        }");
        }

        private void GenerateIAcpClient(string outputPath, string gitRef, string protocolNamespace, string clientNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {clientNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Defines the methods an ACP client implementation must provide to handle protocol calls from an agent.");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public interface IAcpClient");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Called when the RPC connection is disconnected.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"connection\">The connection that was disconnected.</param>");
            sb.AppendLineLf("        void OnDisconnected(Connection connection);");
            sb.AppendLineLf();

            // Sort methods by name for consistent output
            bool first = true;
            foreach (var method in _clientMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;

                if (!first)
                    sb.AppendLineLf();
                first = false;

                var methodName = GetMethodName(method);
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf("        /// <summary>");
                sb.AppendLineLf($"        /// Handles the protocol <c>{method.MethodPath}</c> {(method.IsNotification ? "notification" : "request")}.");
                sb.AppendLineLf("        /// </summary>");
                sb.AppendLineLf($"        /// <param name=\"{(method.IsNotification ? "notification" : "request")}\">The {(method.IsNotification ? "notification" : "request")} payload.</param>");
                sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels request processing.</param>");
                sb.AppendLineLf($"        /// <returns>{(method.IsNotification ? "A task that completes when handling is finished." : "The response.")}</returns>");
                sb.AppendLineLf($"        {returnType} {methodName}Async({method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default);");
            }

            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Handles an extension method call that is not part of the core protocol.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension method name.</param>");
            sb.AppendLineLf("        /// <param name=\"request\">The extension request payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels request processing.</param>");
            sb.AppendLineLf("        /// <returns>The extension method response object.</returns>");
            sb.AppendLineLf("        Task<object> ExtMethodAsync(string method, object request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default);");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Handles an extension notification that is not part of the core protocol.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"method\">The extension notification name.</param>");
            sb.AppendLineLf("        /// <param name=\"notification\">The extension notification payload.</param>");
            sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels notification handling.</param>");
            sb.AppendLineLf("        /// <returns>A task that completes when handling is finished.</returns>");
            sb.AppendLineLf("        Task ExtNotificationAsync(string method, object notification,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default);");
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private void GenerateClientRpcTarget(string outputPath, string gitRef, string protocolNamespace, string clientNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using dotacp.shared;");
            sb.AppendLineLf("using StreamJsonRpc;");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {clientNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    internal sealed class ClientRpcTarget");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        private readonly IAcpClient _client;");
            sb.AppendLineLf();
            sb.AppendLineLf("        public ClientRpcTarget(IAcpClient client)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            _client = client;");
            sb.AppendLineLf("        }");

            // Sort methods by name for consistent output
            var clientTargetMethodNames = ResolveRpcTargetMethodNames(_clientMethods.Values);
            foreach (var method in _clientMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;

                var interfaceMethodName = GetMethodName(method);
                var methodName = clientTargetMethodNames[method.MethodPath];
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf();
                sb.AppendLineLf($"        [JsonRpcMethod(ClientMethods.{method.MethodName}, UseSingleObjectParameterDeserialization = true)]");
                sb.AppendLineLf($"        public {returnType} {methodName}Async(");
                sb.AppendLineLf($"            {method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default)");
                sb.AppendLineLf("        {");
                sb.AppendLineLf($"            return _client.{interfaceMethodName}Async({(method.IsNotification ? "notification" : "request")}, cancellationToken);");
                sb.AppendLineLf("        }");
            }

            sb.AppendLineLf();
            sb.AppendLineLf("        [JsonRpcMethod(\"__acp_ext_method__\", UseSingleObjectParameterDeserialization = true)]");
            sb.AppendLineLf("        public Task<object> HandleExtensionMethodAsync(");
            sb.AppendLineLf("            ExtensionRequest request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _client.ExtMethodAsync(request.Method, request.Arguments, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        [JsonRpcMethod(\"__acp_ext_notification__\", UseSingleObjectParameterDeserialization = true)]");
            sb.AppendLineLf("        public Task HandleExtensionNotificationAsync(");
            sb.AppendLineLf("            ExtensionRequest request,");
            sb.AppendLineLf("            CancellationToken cancellationToken = default)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _client.ExtNotificationAsync(request.Method, request.Arguments, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private void GenerateClientConnection(string outputPath, string gitRef, string protocolNamespace, string clientNamespace)
        {
            var sb = new StringBuilder();
            sb.AppendLineLf("// Generated from schema/meta.json and schema/schema.json. Do not edit by hand.");
            if (!string.IsNullOrEmpty(gitRef))
            {
                sb.AppendLineLf($"// Schema ref: {gitRef}");
            }
            sb.AppendLineLf();
            sb.AppendLineLf($"using {protocolNamespace};");
            sb.AppendLineLf("using dotacp.shared;");
            sb.AppendLineLf("using StreamJsonRpc;");
            sb.AppendLineLf("using System;");
            sb.AppendLineLf("using System.Diagnostics;");
            sb.AppendLineLf("using System.IO;");
            sb.AppendLineLf("using System.Threading;");
            sb.AppendLineLf("using System.Threading.Tasks;");
            sb.AppendLineLf();
            sb.AppendLineLf($"namespace {clientNamespace}");
            sb.AppendLineLf("{");
            sb.AppendLineLf("    /// <summary>");
            sb.AppendLineLf("    /// Manages a JSON-RPC connection between an ACP client and an ACP agent.");
            sb.AppendLineLf("    /// The client can use this connection to communicate with the Agent.");
            sb.AppendLineLf("    /// </summary>");
            sb.AppendLineLf("    public class Connection : IDisposable");
            sb.AppendLineLf("    {");
            sb.AppendLineLf("        private JsonRpc _rpc;");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Gets a task that completes when the underlying RPC channel is closed.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        public Task Completion => _rpc.Completion;");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Connection(IAcpClient client, Stream inputStream, Stream outputStream,");
            sb.AppendLineLf("            TraceSource? traceSource = null)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            var handler = new NewLineDelimitedMessageHandler(");
            sb.AppendLineLf("                inputStream, outputStream, new JsonMessageFormatter());");
            sb.AppendLineLf("            var routingHandler = new ExtensionMethodRoutingMessageHandler(handler);");
            sb.AppendLineLf("            _rpc = new JsonRpcEx(routingHandler);");
            sb.AppendLineLf("            if (traceSource != null)");
            sb.AppendLineLf("                _rpc.TraceSource = traceSource;");
            sb.AppendLineLf();
            sb.AppendLineLf("            _rpc.AddLocalRpcTarget(new ClientRpcTarget(client));");
            sb.AppendLineLf("            _rpc.StartListening();");
            sb.AppendLineLf();
            sb.AppendLineLf("            _rpc.Disconnected += (sender, e) => client.OnDisconnected(this);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Task<TResponse> SendRequestAsync<TRequest, TResponse>(");
            sb.AppendLineLf("            string method, TRequest request, CancellationToken cancellationToken)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            return _rpc.InvokeWithParameterObjectAsync<TResponse>(");
            sb.AppendLineLf("                method, request, cancellationToken);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        private Task SendNotificationAsync<TNotification>(");
            sb.AppendLineLf("            string method, TNotification notification, CancellationToken cancellationToken)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            cancellationToken.ThrowIfCancellationRequested();");
            sb.AppendLineLf("            return _rpc.NotifyWithParameterObjectAsync(method, notification);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();
            sb.AppendLineLf("        /// <summary>");
            sb.AppendLineLf("        /// Create a Connection to an ACP agent over the given streams.");
            sb.AppendLineLf("        /// </summary>");
            sb.AppendLineLf("        /// <param name=\"client\">The client implementation that handles incoming RPC calls.</param>");
            sb.AppendLineLf("        /// <param name=\"inputStream\">The (agent) input stream to write to.</param>");
            sb.AppendLineLf("        /// <param name=\"outputStream\">The (agent) output stream to read from.</param>");
            sb.AppendLineLf("        /// <param name=\"traceSource\">Optional trace source used for StreamJsonRpc diagnostics.</param>");
            sb.AppendLineLf("        /// <returns>");
            sb.AppendLineLf("        /// A running <see cref=\"Connection\"/> instance, or <see langword=\"null\"/> when a required argument is <see langword=\"null\"/>.");
            sb.AppendLineLf("        /// </returns>");
            sb.AppendLineLf("        public static Connection? RunClient(IAcpClient client,");
            sb.AppendLineLf("            Stream inputStream, Stream outputStream,");
            sb.AppendLineLf("            TraceSource? traceSource = null)");
            sb.AppendLineLf("        {");
            sb.AppendLineLf("            if (client == null || inputStream == null || outputStream == null)");
            sb.AppendLineLf("                return null;");
            sb.AppendLineLf();
            sb.AppendLineLf("            return new Connection(client, inputStream, outputStream, traceSource);");
            sb.AppendLineLf("        }");
            sb.AppendLineLf();

            // Generate agent method calls
            foreach (var method in _agentMethods.Values.OrderBy(m => m.MethodPath))
            {
                if (string.IsNullOrEmpty(method.RequestType) || string.IsNullOrEmpty(method.ResponseType))
                    continue;
                var methodName = GetMethodName(method);
                var returnType = method.IsNotification ? "Task" : $"Task<{method.ResponseType}>";

                sb.AppendLineLf("        /// <summary>");
                sb.AppendLineLf($"        /// {(method.IsNotification ? "Sends" : "Calls")} the agent <c>{method.MethodPath}</c> {(method.IsNotification ? "notification" : "method")}.");
                sb.AppendLineLf("        /// </summary>");
                sb.AppendLineLf($"        /// <param name=\"{(method.IsNotification ? "notification" : "request")}\">The {(method.IsNotification ? "notification" : "request")} payload.</param>");
                sb.AppendLineLf("        /// <param name=\"cancellationToken\">A token that cancels the operation.</param>");
                sb.AppendLineLf($"        /// <returns>{(method.IsNotification ? "A task that completes when the notification is sent." : "The response.")}</returns>");
                sb.AppendLineLf($"        public {returnType} {methodName}Async(");
                sb.AppendLineLf($"            {method.RequestType} {(method.IsNotification ? "notification" : "request")},");
                sb.AppendLineLf("            CancellationToken cancellationToken = default)");
                sb.AppendLineLf("        {");
                if (method.IsNotification)
                {
                    sb.AppendLineLf($"            return SendNotificationAsync(AgentMethods.{method.MethodName}, notification, cancellationToken);");
                }
                else
                {
                    sb.AppendLineLf($"            return SendRequestAsync<{method.RequestType}, {method.ResponseType}>(");
                    sb.AppendLineLf($"                AgentMethods.{method.MethodName}, request, cancellationToken);");
                }
                sb.AppendLineLf("        }");
                sb.AppendLineLf();
            }

            GenerateCommonMethods(sb, false);
            sb.AppendLineLf("    }");
            sb.AppendLineLf("}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        private string GetMethodName(MethodInfo method)
        {
            // Convert method path to method name
            // e.g., "session/new" -> "NewSession"
            // e.g., "initialize" -> "Initialize"
            // e.g., "session/cancel" -> "Cancel"

            var parts = method.MethodPath.Split('/');
            if (parts.Length == 1)
            {
                return NamingHelper.ConvertToPascalCase(parts[0]);
            }

            // For paths like "session/new", use just "NewSession" style
            var lastPart = NamingHelper.ConvertToPascalCase(parts[parts.Length - 1]);

            // Special handling based on existing patterns
            if (method.MethodPath.StartsWith("session/"))
            {
                var action = parts[parts.Length - 1];
                if (action == "new")
                    return "NewSession";
                if (action == "cancel")
                    return "Cancel";
                if (action == "load")
                    return "LoadSession";
                if (action == "fork")
                    return "ForkSession";
                if (action == "list")
                    return "ListSessions";
                if (action == "resume")
                    return "ResumeSession";
                if (action == "prompt")
                    return "Prompt";
                if (action == "update")
                    return "SessionUpdate";
                if (action == "request_permission")
                    return "RequestPermission";
                if (action == "set_config_option")
                    return "SetSessionConfigOption";
                if (action == "set_mode")
                    return "SetSessionMode";
                if (action == "set_model")
                    return "SetSessionModel";
            }

            if (method.MethodPath.StartsWith("fs/"))
            {
                return NamingHelper.ConvertToPascalCase(parts[parts.Length - 1]);
            }

            if (method.MethodPath.StartsWith("terminal/"))
            {
                var action = parts[parts.Length - 1];
                if (action == "output")
                    return "TerminalOutput";
                if (action == "wait_for_exit")
                    return "WaitForTerminalExit";
                return NamingHelper.ConvertToPascalCase(action) + "Terminal";
            }

            return lastPart;
        }

        private Dictionary<string, string> ResolveRpcTargetMethodNames(IEnumerable<MethodInfo> methods)
        {
            var orderedMethods = methods.OrderBy(m => m.MethodPath).ToList();
            var groupedNames = orderedMethods
                .GroupBy(GetMethodName)
                .ToDictionary(group => group.Key, group => group.Count());

            var resolvedNames = new Dictionary<string, string>();
            foreach (var method in orderedMethods)
            {
                var baseName = GetMethodName(method);
                resolvedNames[method.MethodPath] = groupedNames[baseName] > 1
                    ? method.MethodName
                    : baseName;
            }

            return resolvedNames;
        }
    }
}
