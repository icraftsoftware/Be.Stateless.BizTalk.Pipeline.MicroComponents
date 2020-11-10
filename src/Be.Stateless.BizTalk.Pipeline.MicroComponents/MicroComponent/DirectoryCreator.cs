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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class DirectoryCreator : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			message.EnsureFileOutboundTransport();
			var location = Path.GetDirectoryName(message.GetProperty(BtsProperties.OutboundTransportLocation));
			if (ImpersonationEnabled)
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Impersonating file adapter's configured user to create directory.");
				Delegate.InvokeAs(
					message.GetProperty(FileProperties.Username),
					message.GetProperty(FileProperties.Password),
					() => CreateDirectory(location));
			}
			else
			{
				CreateDirectory(location);
			}
			return message;
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
		public bool ImpersonationEnabled { get; set; }

		[SuppressMessage("Performance", "CA1822:Mark members as static")]
		private void CreateDirectory(string location)
		{
			if (Directory.Exists(location))
			{
				if (_logger.IsDebugEnabled) _logger.Debug($"Directory {location} already exists, skipping creation.");
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.Debug($"Directory {location} does not exist, attempting creation.");
				Directory.CreateDirectory(location);
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(DirectoryCreator));
	}
}
