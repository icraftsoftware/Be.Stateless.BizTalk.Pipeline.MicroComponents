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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class FileOutboundTransportLocationBuilderFixture
	{
		#region Setup/Teardown

		public FileOutboundTransportLocationBuilderFixture()
		{
			MessageContextMock = new();
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(Adapter.Transport.OutboundTransport.FileTransmitterClassId.ToString("D"));
		}

		#endregion

		[Fact]
		public void DoesNothingWhenNotFileTransmitter()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("outbound-transport");
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(Adapter.Transport.OutboundTransport.SBMessagingTransmitterClassId.ToString("D"));

			Invoking(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().NotThrow();

			MessageContextMock.Verify(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation), Times.Never);
			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void OutboundTransportLocationHasFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"C:\Files\Drops\Party\%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"File.txt");

			new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, @"C:\Files\Drops\Party\File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"C:\Files\Drops\Party\%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"Folder\File.txt");

			new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, @"C:\Files\Drops\Party\Folder\File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasRootedFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"C:\Files\Drops\Party\%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"\Folder\File.txt");

			new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, @"\Folder\File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationIsUncPath()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"\\server\%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"File.txt");

			new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, @"\\server\File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationIsUncPathWithSubDirectory()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"\\server\SubDirectory\%MessageID%.xml");
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"File.txt");

			new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.OutboundTransportLocation, @"\\server\SubDirectory\File.txt"));
		}

		[Fact]
		public void ThrowsWhenNoOutboundTransportLocation()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"C:\Files\Drops\Party\%MessageID%.xml");

			Invoking(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Target sub path and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
		}

		[Fact]
		public void ThrowsWhenNoRootOutboundTransportLocation()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("file.txt");

			Invoking(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Root path was expected to be found in BtsProperties.OutboundTransportLocation context property.");
		}

		private MessageContextMock MessageContextMock { get; }
	}
}
