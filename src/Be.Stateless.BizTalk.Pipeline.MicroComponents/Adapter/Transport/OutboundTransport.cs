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
using Microsoft.BizTalk.Adapter.SBMessaging;
using Microsoft.BizTalk.Adapter.Sftp;

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
		internal static readonly Guid SBMessagingTransmitterClassId = typeof(SBMessagingTransmitter).GUID;
		internal static readonly Guid SftpTransmitterClassId = typeof(SftpTransmitter).GUID;
		private readonly Guid _clsId;
	}
}
