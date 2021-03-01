#region Copyright & License

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class SftpOutboundTransportLocationBuilderFixture
	{
		#region Setup/Teardown

		public SftpOutboundTransportLocationBuilderFixture()
		{
			MessageContextMock = new MessageContextMock();
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns("{C166A7E5-4F4C-4B02-A6F2-8BE07E1FA786}");
			MessageContextMock
				.Setup(c => c.GetProperty(SftpProperties.FolderPath))
				.Returns("/Files/Drops/Party");
			MessageContextMock
				.Setup(c => c.GetProperty(SftpProperties.TargetFileName))
				.Returns(@"%MessageID%.xml");
		}

		#endregion

		[Fact]
		public void OutboundTransportLocationHasFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"File.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.IsDynamicSend, true));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.FolderPath, "/Files/Drops/Party"));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.TargetFileName, @"File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"Folder\File.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.IsDynamicSend, true));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.FolderPath, "/Files/Drops/Party/Folder"));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.TargetFileName, @"File.txt"));
		}

		[Fact]
		public void OutboundTransportLocationHasRootedFolderAndFile()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"\Folder\File.txt");

			new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object);

			MessageContextMock.Verify(c => c.SetProperty(BtsProperties.IsDynamicSend, true));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.FolderPath, "/Folder"));
			MessageContextMock.Verify(c => c.SetProperty(SftpProperties.TargetFileName, @"File.txt"));
		}

		[Fact]
		public void ThrowsWhenNoOutboundTransportLocation()
		{
			Invoking(() => new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Target sub folder and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
		}

		[Fact]
		public void ThrowsWhenNotUsedWithMicrosoftSftpAdapter()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns("{9D0E4341-4CCE-4536-83FA-4A5040674AD6}");

			Invoking(() => new SftpOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Outbound SFTP transport is required on this leg of the message exchange pattern.");
		}

		private MessageContextMock MessageContextMock { get; }
	}
}
