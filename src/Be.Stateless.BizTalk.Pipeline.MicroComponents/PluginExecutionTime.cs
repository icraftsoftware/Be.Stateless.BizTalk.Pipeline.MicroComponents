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

using Be.Stateless.BizTalk.MicroComponent;

namespace Be.Stateless.BizTalk
{
	/// <summary>
	/// The time at which a micro component's plugin will be executed. A plugin can take various forms ranging, for instance,
	/// from an <see cref="IMessageBodyStreamFactory"/> to an <see cref="IContextBuilder"/>.
	/// </summary>
	/// <remarks>
	/// The execution time can either be <see cref="Immediate"/>, in which case the plugin will be executed as soon as its
	/// hosting micro component &#8212;e.g. the <see cref="ContextBuilder"/> or the <see
	/// cref="MessageBodyStreamFactory"/>&#8212; starts being executed, or <see cref="Deferred"/>, in which case its hosting
	/// micro component will wait for the message stream to be exhausted to execute the plugin.
	/// </remarks>
	public enum PluginExecutionTime
	{
		/// <summary>
		/// Executes the plugin as soon as its hosting micro component starts being executed.
		/// </summary>
		Immediate = 0,

		/// <summary>
		/// Executes the plugin only after the message stream has been exhausted.
		/// </summary>
		Deferred = 1
	}
}
