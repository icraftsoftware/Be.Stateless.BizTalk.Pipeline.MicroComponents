#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class XmlEnvelopeDecoder : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			if (message == null) throw new ArgumentNullException(nameof(message));

			var messageType = message.ProbeMessageType(pipelineContext);
			var schemaMetadata = pipelineContext.GetSchemaMetadataByType(messageType, false);
			if (schemaMetadata.IsEnvelopeSchema)
			{
				// rewrite empty or partial envelope to prevent disassembler from throwing
				message.BodyPart.WrapOriginalDataStream(
					originalStream => {
						if (_logger.IsDebugEnabled) _logger.Debug($"Wrapping message stream in an {nameof(XmlEnvelopeDecodingStream)}.");
						return new XmlEnvelopeDecodingStream(originalStream, schemaMetadata.BodyXPath);
					},
					pipelineContext.ResourceTracker);
			}
			return message;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XmlEnvelopeDecoder));
	}
}
