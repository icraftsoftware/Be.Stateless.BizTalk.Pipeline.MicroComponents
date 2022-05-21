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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using BTS;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using SBMessaging;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Propagates message type and correlation id over inbound and outbound Azure ServiceBus queues.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For inbound messages, <see cref="SBMessagingProperties.CorrelationId">SBMessagingProperties.CorrelationId</see>, if any,
	/// is promoted into BizTalk Server message context as <c>BizTalkFactoryProperties.CorrelationId</c> property.
	/// </para>
	/// <para>
	/// For outbound messages, <c>BizTalkFactoryProperties.CorrelationId</c> and <see
	/// cref="BtsProperties.MessageType">BtsProperties.MessageType</see>, if any, are respectively propagated as
	/// <see cref="SBMessagingProperties.CorrelationId">SBMessagingProperties.CorrelationId</see>
	/// and <c>MessageType</c> in namespace declared by <c>SB-Messaging</c> adapter's <see
	/// cref="CustomBrokeredMessagePropertyNamespace"/> configuration property.
	/// </para>
	/// </remarks>
	public class SBMessagingContextPropagator : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (message.Direction().IsInbound())
			{
				// As no InboundTransportCLSID is written in context, there is no easy way to determine if InboundTransport is
				// SBMessagingReceiver; assumes thus that only it could write SBMessagingProperties.CorrelationId in context.
				message.GetProperty(SBMessagingProperties.CorrelationId)
					.IfNotNullOrEmpty(message.PromoteBizTalkFactoryCorrelationId);
				// don't propagate BtsProperties.MessageType; assumes XmlDisassembler will determine and promote the actual message type
			}
			else if (message.OutboundTransport().IsSBMessagingTransmitter())
			{
				message.GetBizTalkFactoryCorrelationId()
					.IfNotNullOrEmpty(id => message.SetProperty(SBMessagingProperties.CorrelationId, id));
				message.GetOrProbeMessageType(pipelineContext)
					.IfNotNullOrEmpty(type => message.Context.Write(nameof(MessageType), message.GetWcfCustomBrokeredPropertyNamespace(), type));
			}
			return message;
		}

		#endregion
	}
}
