#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Adapter.Transport;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Namespaces;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using SBMessaging;
using WCF;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageMicroComponentExtensions
	{
		public static string GetOrProbeMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			return message.GetOrProbeMessageType(pipelineContext.ResourceTracker);
		}

		public static OutboundTransport OutboundTransport(this IBaseMessage message)
		{
			return message.Direction().IsOutbound()
				? new(new Guid(message.GetProperty(BtsProperties.OutboundTransportCLSID)))
				: Adapter.Transport.OutboundTransport.None;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public static string ProbeMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			return message.ProbeMessageType(pipelineContext.ResourceTracker);
		}

		public static void ProbeAndPromoteMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			var messageType = message.ProbeMessageType(pipelineContext);
			var docSpec = pipelineContext.GetDocumentSpecByType(messageType);
			message.Promote(BtsProperties.MessageType, docSpec.DocType);
			message.Promote(BtsProperties.SchemaStrongName, docSpec.DocSpecStrongName);
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public static void TryProbeAndPromoteMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			var messageType = message.ProbeMessageType(pipelineContext);
			if (pipelineContext.TryGetDocumentSpecByType(messageType, out var docSpec))
			{
				message.Promote(BtsProperties.MessageType, docSpec.DocType);
				message.Promote(BtsProperties.SchemaStrongName, docSpec.DocSpecStrongName);
			}
		}

		internal static string GetBizTalkFactoryCorrelationId(this IBaseMessage message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			// use the native BTS API not to have any dependency on BizTalk.Schemas which would reverse the desired dependency
			// order that should be from an artifact component (BizTalk.Schemas) to a runtime one (BizTalk.Pipeline.MicroComponents)
			return (string) message.Context.Read(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory);
		}

		internal static string GetWcfCustomBrokeredPropertyNamespace(this IBaseMessage message)
		{
			return message.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)
				?? throw new InvalidOperationException(
					$"Cannot find {nameof(CustomBrokeredPropertyNamespace)} property in message context. Verify that {nameof(CustomBrokeredMessagePropertyNamespace)} has been configured for the outbound SB-Messaging adapter.");
		}

		internal static void PromoteBizTalkFactoryCorrelationId(this IBaseMessage message, string id)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			// use the native BTS API not to have any dependency on BizTalk.Schemas which would reverse the desired dependency
			// order that should be from an artifact component (BizTalk.Schemas) to a runtime one (BizTalk.Pipeline.MicroComponents)
			if (!id.IsNullOrEmpty()) message.Context.Promote(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory, id);
		}
	}
}
