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

using System.Collections.Generic;
using System.Linq;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Runs a sequence of micro components, i.e. components implementing <see cref="IMicroComponent"/>, similarly to what a
	/// regular Microsoft BizTalk Server pipeline would do if the micro components were regular pipeline components.
	/// </summary>
	public class MicroPipeline
	{
		public MicroPipeline()
		{
			Components = Enumerable.Empty<IMicroComponent>();
		}

		/// <summary>
		/// List of micro components that will be run in sequence by the micro pipeline.
		/// </summary>
		public IEnumerable<IMicroComponent> Components { get; set; }

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return Components.Aggregate(
				message,
				(inputMessage, microPipelineComponent) => microPipelineComponent.Execute(pipelineContext, inputMessage));
		}

		public void LoadConfiguration(string configuration)
		{
			Components = MicroComponentEnumerableConverter.Deserialize(configuration);
		}

		public string SaveConfiguration()
		{
			return MicroComponentEnumerableConverter.Serialize(Components);
		}
	}
}
