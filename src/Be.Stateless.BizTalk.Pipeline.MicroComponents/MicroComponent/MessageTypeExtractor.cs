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

using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Probe the current message for its type and write it in the <see
	/// cref="BizTalkFactoryProperties.MessageType">BizTalkFactoryProperties.MessageType</see> context property.
	/// </summary>
	/// <remarks>
	/// This component will always probe the current message for its type and will consequently always install a <see
	/// cref="MarkableForwardOnlyEventingReadStream"/> around the current message's body part stream.
	/// </remarks>
	public class MessageTypeExtractor : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var messageType = message.ProbeMessageType(pipelineContext);
			if (pipelineContext.TryGetDocumentSpecByType(messageType, out var docSpec))
			{
				message.SetProperty(BizTalkFactoryProperties.MessageType, docSpec.DocType);
			}
			return message;
		}

		#endregion
	}
}
