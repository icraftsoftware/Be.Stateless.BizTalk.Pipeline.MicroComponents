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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Pipeline component which compresses the incoming data into a Zip Archive. The component wraps the message's original
	/// stream in the zip-compressing stream <see cref="ZipOutputStream"/>.
	/// </summary>
	/// <remarks>
	/// Zip-compress outbound message's body part stream.
	/// </remarks>
	/// <seealso cref="ZipOutputStream"/>
	public class ZipEncoder : IMicroComponent
	{
		#region IMicroComponent Members

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			if (message == null) throw new ArgumentNullException(nameof(message));
			var location = message.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation);
			if (location.IsNullOrEmpty())
				throw new InvalidOperationException("BizTalkFactoryProperties.OutboundTransportLocation has to be set in context in order to determine zip entry name.");

			message.BodyPart.WrapOriginalDataStream(
				originalStream => {
					if (_logger.IsDebugEnabled) _logger.Debug("Wrapping message stream in a zip-compressing stream.");
					var zipEntryName = Path.GetFileName(location);
					return new ZipOutputStream(originalStream, zipEntryName);
				},
				pipelineContext.ResourceTracker);

			var zipLocation = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + ".zip");
			message.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, zipLocation);

			return message;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ZipEncoder));
	}
}
