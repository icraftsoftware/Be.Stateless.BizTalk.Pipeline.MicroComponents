﻿#region Copyright & License

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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Wraps the original message stream by a <see cref="MultipartFormDataContentStream"/>.
	/// </summary>
	public class MultipartFormDataContentEncoder : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			if (message == null) throw new ArgumentNullException(nameof(message));
			message.BodyPart.WrapOriginalDataStream(
				originalStream => {
					if (_logger.IsDebugEnabled) _logger.Debug("Wrapping message stream in a MultipartFormDataContentStream.");
					var multipartFormDataContentStream = ContentName.IsNullOrEmpty()
						? UseBodyPartNameAsContentName
							? new(originalStream, message.BodyPartName)
							: new MultipartFormDataContentStream(originalStream)
						: new(originalStream, ContentName);
					message.BodyPart.ContentType = multipartFormDataContentStream.ContentType;
					return multipartFormDataContentStream;
				},
				pipelineContext.ResourceTracker);
			return message;
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public string ContentName { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public bool UseBodyPartNameAsContentName { get; set; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(MultipartFormDataContentEncoder));
	}
}
