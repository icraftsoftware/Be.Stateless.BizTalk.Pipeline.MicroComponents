#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageMicroComponentExtensions
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static void EnsureFileOutboundTransport(this IBaseMessage message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.Context.EnsureFileOutboundTransport();
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static void EnsureSftpOutboundTransport(this IBaseMessage message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.Context.EnsureSftpOutboundTransport();
		}

		public static string GetOrProbeMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			return message.GetOrProbeMessageType(pipelineContext.ResourceTracker);
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
	}
}
