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
using Be.Stateless.BizTalk.Adapter.Transport;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class DirectoryCreatorFixture : MicroComponentFixture<DirectoryCreator>
	{
		[Fact]
		public void CreateDirectory()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"c:\file\message.xml");
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(OutboundTransport.FileTransmitterClassId.ToString("D"));

			var sut = new Mock<DirectoryCreator> { CallBase = true };
			sut.Object.Execute(PipelineContextMock.Object, MessageMock.Object);

			sut.Verify(m => m.CreateDirectory(@"c:\file"));
		}

		[Fact]
		public void CreateDirectoryIsNotCalledForInboundTraffic()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation))
				.Returns("inbound-direction");

			var sut = new Mock<DirectoryCreator> { CallBase = true };

			Invoking(() => sut.Object.Execute(PipelineContextMock.Object, MessageMock.Object)).Should().NotThrow();

			sut.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CreateDirectoryIsNotCalledForOtherTransmitterThanFile()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("outbound-direction");
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(OutboundTransport.SBMessagingTransmitterClassId.ToString("D"));

			var sut = new Mock<DirectoryCreator> { CallBase = true };

			Invoking(() => sut.Object.Execute(PipelineContextMock.Object, MessageMock.Object)).Should().NotThrow();

			sut.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CreateDirectoryThrows()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("file.txt");
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(OutboundTransport.FileTransmitterClassId.ToString("D"));

			var sut = new DirectoryCreator();
			Invoking(() => sut.Execute(PipelineContextMock.Object, MessageMock.Object))
				.Should().Throw<ArgumentException>()
				.WithMessage("Path cannot be the empty string or all whitespace.");
		}
	}
}
