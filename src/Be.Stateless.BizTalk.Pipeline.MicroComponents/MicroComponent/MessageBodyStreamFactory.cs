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
using System.Xml.Serialization;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Micro component that replaces the <see cref="System.IO.Stream"/> of the current message's <see
	/// cref="IBaseMessage.BodyPart"/> by a new one whose creation is delegated to either a contextual or statically
	/// configurable <see cref="IMessageBodyStreamFactory"/> plugin.
	/// </summary>
	public class MessageBodyStreamFactory : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			message.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, FactoryType)
				.AsPlugin<IMessageBodyStreamFactory>()
				.IfNotNull(messageBodyStreamFactory => message.BodyPart.SetDataStream(messageBodyStreamFactory.Create(message), pipelineContext.ResourceTracker));
			return message;
		}

		#endregion

		/// <summary>
		/// The type of the <see cref="IMessageBodyStreamFactory"/> plugin that will be called to create the <see
		/// cref="System.IO.Stream"/> of the message's <see cref="IBaseMessage.BodyPart"/>.
		/// </summary>
		[XmlElement("Factory", typeof(RuntimeTypeXmlSerializer))]
		public Type FactoryType { get; set; }
	}
}
