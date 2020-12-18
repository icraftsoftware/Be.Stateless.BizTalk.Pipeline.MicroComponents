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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class FileOutboundTransportLocationBuilder : IContextBuilder
	{
		#region IContextBuilder Members

		public void Execute(IBaseMessageContext context)
		{
			context.EnsureFileOutboundTransport();

			var outboundTransportLocation = context.GetProperty(BtsProperties.OutboundTransportLocation);
			var rootPath = Path.GetDirectoryName(outboundTransportLocation);
			if (rootPath.IsNullOrEmpty() && IO.Path.IsNetworkPath(outboundTransportLocation))
			{
				var fileName = Path.GetFileName(outboundTransportLocation);
				if (!fileName.IsNullOrEmpty()) rootPath = outboundTransportLocation.Remove(outboundTransportLocation.Length - fileName.Length);
			}
			if (rootPath.IsNullOrEmpty()) throw new InvalidOperationException("Root path was expected to be found in BtsProperties.OutboundTransportLocation context property.");

			var subPathAndFile = context.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation);
			if (subPathAndFile.IsNullOrEmpty())
				throw new InvalidOperationException(
					"Target sub path and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
			context.SetProperty(BtsProperties.OutboundTransportLocation, Path.Combine(rootPath!, subPathAndFile));
		}

		#endregion
	}
}
