﻿#region Copyright & License

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class SftpOutboundTransportLocationBuilderFixture
	{
		#region Setup/Teardown

		public SftpOutboundTransportLocationBuilderFixture()
		{
			MessageContextMock = new();
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("sftp://sftp.world.com:22/files/drops/party/%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(Adapter.Transport.OutboundTransport.SftpTransmitterClassId.ToString("D"));
			MessageContextMock
				.Setup(c => c.GetProperty(SftpProperties.FolderPath))
				.Returns("/files/drops/party");
		}

		#endregion

		[Fact]
		public void DoesNothingWhenNotSftpTransmitter()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("outbound-transport");
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(Adapter.Transport.OutboundTransport.SBMessagingTransmitterClassId.ToString("D"));

			Invoking(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().NotThrow();

			MessageContextMock.Verify(c => c.GetProperty(SftpProperties.FolderPath), Times.Never);
			MessageContextMock.Verify(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation), Times.Never);
			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void OutboundTransportLocationHasFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"file.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(
				c => c.SetProperty(
					BtsProperties.OutboundTransportLocation,
					"sftp://sftp.world.com:22/files/drops/party/file.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"folder\file.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(
				c => c.SetProperty(
					BtsProperties.OutboundTransportLocation,
					"sftp://sftp.world.com:22/files/drops/party/folder/file.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasRootedFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"\folder\file.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(
				c => c.SetProperty(
					BtsProperties.OutboundTransportLocation,
					"sftp://sftp.world.com:22/folder/file.txt"));
		}

		[Fact]
		public void ThrowsWhenNoOutboundTransportLocation()
		{
			Invoking(() => new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Target sub folder and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
		}

		private MessageContextMock MessageContextMock { get; }
	}
}
