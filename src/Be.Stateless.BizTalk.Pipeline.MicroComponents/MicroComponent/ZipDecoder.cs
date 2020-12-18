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
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Pipeline component which decompresses the first entry of a Zip Archive. The component wraps the message's original
	/// stream in the zip decompressing stream <see cref="ZipInputStream"/>.
	/// </summary>
	/// <remarks>
	/// See <see cref="ZipInputStream"/> for details on the Zip Archive format supported.
	/// </remarks>
	public class ZipDecoder : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.BodyPart.WrapOriginalDataStream(
				originalStream => {
					if (_logger.IsDebugEnabled) _logger.Debug("Wrapping message stream in a zip-decompressing stream.");
					var substitutionStream = new ZipInputStream(originalStream);
					return substitutionStream;
				},
				pipelineContext.ResourceTracker);
			return message;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ZipDecoder));
	}
}
