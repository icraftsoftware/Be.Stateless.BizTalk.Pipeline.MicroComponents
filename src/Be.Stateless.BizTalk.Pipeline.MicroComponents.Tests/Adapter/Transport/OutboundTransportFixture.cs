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
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Adapter.Transport
{
	public class OutboundTransportFixture
	{
		[SkippableFact]
		public void FileTransmitterClassId()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Type.GetTypeFromProgID("BizTalk.BTMFileTransport").GUID
				.Should().Be(OutboundTransport.FileTransmitterClassId);
		}

		[SkippableFact]
		public void SBMessagingTransmitterClassId()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Type.GetTypeFromProgID("Microsoft.BizTalk.Adapter.SBMessaging.SBMessagingTransmitter").GUID
				.Should().Be(OutboundTransport.SBMessagingTransmitterClassId);
		}

		[SkippableFact]
		public void SftpTransmitterClassId()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Type.GetTypeFromProgID("Microsoft.BizTalk.Adapter.Sftp.SftpTransmitter").GUID
				.Should().Be(OutboundTransport.SftpTransmitterClassId);
		}
	}
}
