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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageContextMicroComponentExtensions
	{
		public static void EnsureFileOutboundTransport(this IBaseMessageContext context)
		{
			var outboundTransportClassId = context.GetProperty(BtsProperties.OutboundTransportCLSID).IfNotNullOrEmpty(g => new Guid(g));
			if (outboundTransportClassId != FileAdapterOutboundTransportClassId)
				throw new InvalidOperationException("Outbound file transport is required on this leg of the message exchange pattern.");
		}

		public static void EnsureSftpOutboundTransport(this IBaseMessageContext context)
		{
			var outboundTransportClassId = context.GetProperty(BtsProperties.OutboundTransportCLSID).IfNotNullOrEmpty(g => new Guid(g));
			if (outboundTransportClassId != SftpAdapterOutboundTransportClassId)
				throw new InvalidOperationException("Outbound SFTP transport is required on this leg of the message exchange pattern.");
		}

		internal static readonly Guid FileAdapterOutboundTransportClassId = new Guid("9d0e4341-4cce-4536-83fa-4a5040674ad6");
		internal static readonly Guid SftpAdapterOutboundTransportClassId = new Guid("c166a7e5-4f4c-4b02-a6f2-8be07e1fa786");
	}
}
