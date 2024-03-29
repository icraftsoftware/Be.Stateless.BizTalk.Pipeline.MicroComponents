﻿#region Copyright & License

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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class SftpOutboundTransportLocationBuilder : IContextBuilder
	{
		#region IContextBuilder Members

		public void Execute(IBaseMessageContext context)
		{
			if (!context.OutboundTransport().IsSftpTransmitter()) return;

			var builder = new UriBuilder(context.GetProperty(BtsProperties.OutboundTransportLocation));
			var folder = context.GetProperty(SftpProperties.FolderPath);
			var subFolderAndFile = context.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation);
			if (subFolderAndFile.IsNullOrEmpty())
				throw new InvalidOperationException("Target sub folder and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
			builder.Path = Path.Combine(folder, subFolderAndFile);
			context.SetProperty(BtsProperties.OutboundTransportLocation, builder.ToString());
		}

		#endregion
	}
}
