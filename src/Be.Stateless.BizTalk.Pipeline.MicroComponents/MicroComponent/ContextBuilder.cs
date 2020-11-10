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
using System.Xml.Serialization;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.Extensions;
using Be.Stateless.Xml;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Delegates building of message context to a <see cref="IContextBuilder"/> component plugin.
	/// </summary>
	public class ContextBuilder : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			message.ResolvePluginType(BizTalkFactoryProperties.ContextBuilderTypeName, BuilderType)
				.AsPlugin<IContextBuilder>()
				.IfNotNull(
					contextBuilder => {
						if (ExecutionTime == PluginExecutionTime.Deferred)
						{
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Scheduling context builder plugin '{0}' for deferred execution.", BuilderType);
							message.BodyPart.WrapOriginalDataStream(
								originalStream => {
									var substitutionStream = new EventingReadStream(originalStream);
									substitutionStream.AfterLastReadEvent += (src, args) => {
										if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing context builder plugin '{0}' that was scheduled for deferred execution.", BuilderType);
										contextBuilder.Execute(message.Context);
									};
									return substitutionStream;
								},
								pipelineContext.ResourceTracker);
						}
						else
						{
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing context builder plugin '{0}' that is scheduled for immediate execution.", BuilderType);
							contextBuilder.Execute(message.Context);
						}
					});
			return message;
		}

		#endregion

		/// <summary>
		/// The type name of the context builder plugin that will be called upon.
		/// </summary>
		[XmlElement("Builder", typeof(RuntimeTypeXmlSerializer))]
		public Type BuilderType { get; set; }

		/// <summary>
		/// The plugin execution mode, either <see cref="PluginExecutionTime.Immediate"/> or <see
		/// cref="PluginExecutionTime.Deferred"/>.
		/// </summary>
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public PluginExecutionTime ExecutionTime { get; set; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ContextBuilder));
	}
}
