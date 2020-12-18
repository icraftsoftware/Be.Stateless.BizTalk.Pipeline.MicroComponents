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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Namespaces;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using SBMessaging;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Propagates message type and correlation id over Azure ServiceBus queues inwards and outwards.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For inbound messages, <see cref="SBMessagingProperties.CorrelationId">SBMessagingProperties.CorrelationId</see> and
	/// <c>MessageType</c> &#8212;in namespace declared by <c>SB-Messaging</c> adapter's <see
	/// cref="SBMessagingProperties.CustomBrokeredMessagePropertyNamespace"/>&#8212; if any, are respectively promoted into
	/// BizTalk message context as <c>BizTalkFactoryProperties.CorrelationId</c> and <see
	/// cref="BtsProperties.MessageType">BtsProperties.MessageType</see>.
	/// </para>
	/// <para>
	/// For outbound messages, <c>BizTalkFactoryProperties.CorrelationId</c> and <see
	/// cref="BtsProperties.MessageType">BtsProperties.MessageType</see>, if any, are respectively propagated as
	/// <see cref="SBMessagingProperties.CorrelationId">SBMessagingProperties.CorrelationId</see>
	/// and <c>MessageType</c> in namespace declared by <c>SB-Messaging</c> adapter's <see
	/// cref="SBMessagingProperties.CustomBrokeredMessagePropertyNamespace"/>.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class SBMessagingContextPropagator : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var customBrokeredMessagePropertyNamespace = message.GetProperty(SBMessagingProperties.CustomBrokeredMessagePropertyNamespace)
				?? throw new InvalidOperationException($"{nameof(CustomBrokeredMessagePropertyNamespace)} has no value defined in SB-Messaging adapter configuration.");
			if (message.Direction().IsInbound())
			{
				var correlationId = message.GetProperty(SBMessagingProperties.CorrelationId);
				// use the native BTS API instead of the message.PromoteCorrelationId(correlationId) to have no dependency on
				// BizTalk.Schemas which would reversed the desired dependency order, i.e. from an artifact component
				// (BizTalk.Schemas) to a runtime one (BizTalk.Pipeline.MicroComponents itself)
				if (!correlationId.IsNullOrEmpty()) message.Context.Promote(nameof(SBMessagingProperties.CorrelationId), PropertySchemaNamespaces.BizTalkFactory, correlationId);
				var messageType = (string) message.Context.Read(nameof(BizTalkFactoryProperties.MessageType), customBrokeredMessagePropertyNamespace);
				if (!messageType.IsNullOrEmpty()) message.Promote(BtsProperties.MessageType, messageType);
			}
			else
			{
				// use the native BTS API instead of the message.GetProperty(BizTalkFactoryProperties.CorrelationId) to have no
				// dependency on BizTalk.Schemas which would reversed the desired dependency order, i.e. from an artifact component
				// (BizTalk.Schemas) to a runtime one (BizTalk.Pipeline.MicroComponents itself)
				var correlationId = (string) message.Context.Read(nameof(SBMessagingProperties.CorrelationId), BizTalkFactoryProperties.MessageType.Namespace);
				if (!correlationId.IsNullOrEmpty()) message.SetProperty(SBMessagingProperties.CorrelationId, correlationId);
				var messageType = message.GetOrProbeMessageType(pipelineContext);
				if (!messageType.IsNullOrEmpty()) message.Context.Write(nameof(BizTalkFactoryProperties.MessageType), customBrokeredMessagePropertyNamespace, messageType);
			}
			return message;
		}

		#endregion
	}
}
