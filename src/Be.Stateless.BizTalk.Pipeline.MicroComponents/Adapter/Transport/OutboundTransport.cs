#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

namespace Be.Stateless.BizTalk.Adapter.Transport
{
	public class OutboundTransport
	{
		internal static OutboundTransport None { get; } = new(Guid.Empty);

		internal OutboundTransport(Guid clsId)
		{
			_clsId = clsId;
		}

		public bool IsFileTransmitter()
		{
			return _clsId == FileTransmitterClassId;
		}

		public bool IsSftpTransmitter()
		{
			return _clsId == SftpTransmitterClassId;
		}

		public bool IsSBMessagingTransmitter()
		{
			return _clsId == SBMessagingTransmitterClassId;
		}

		internal static readonly Guid FileTransmitterClassId = new("9d0e4341-4cce-4536-83fa-4a5040674ad6");
		internal static readonly Guid SBMessagingTransmitterClassId = new("1dc332a4-f68e-48ab-8644-745c8d0f9cc7");
		internal static readonly Guid SftpTransmitterClassId = new("c166a7e5-4f4c-4b02-a6f2-8be07e1fa786");
		private readonly Guid _clsId;
	}
}
