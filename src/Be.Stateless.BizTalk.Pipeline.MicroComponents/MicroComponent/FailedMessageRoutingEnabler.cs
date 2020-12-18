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

using Be.Stateless.BizTalk.ContextProperties.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Enables routing of failed messages and prevents routing failure reports from being generated.
	/// </summary>
	public class FailedMessageRoutingEnabler : IMicroComponent
	{
		public FailedMessageRoutingEnabler()
		{
			EnableFailedMessageRouting = true;
			SuppressRoutingFailureReport = true;
		}

		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (EnableFailedMessageRouting) message.RouteMessageOnFailure();
			if (SuppressRoutingFailureReport) message.SuppressRoutingFailureDiagnosticInfo();
			return message;
		}

		#endregion

		/// <summary>
		/// Enables or disables routing of failed messages and whether to avoid suspended message instances.
		/// </summary>
		public bool EnableFailedMessageRouting { get; set; }

		/// <summary>
		/// Whether to prevent the generation of a routing failure report upon message routing failure.
		/// </summary>
		public bool SuppressRoutingFailureReport { get; set; }
	}
}
