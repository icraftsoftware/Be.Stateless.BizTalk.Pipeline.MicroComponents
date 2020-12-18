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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Builder.Send
{
	public class FileOutboundTransportLocationBuilderFixture
	{
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

			Action(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Target sub path and file name were expected to be found in BizTalkFactoryProperties.OutboundTransportLocation context property.");
		}

		[Fact]
		public void ThrowsWhenNoRootOutboundTransportLocation()
		{
			Action(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Root path was expected to be found in BtsProperties.OutboundTransportLocation context property.");
		}

		[Fact]
		public void ThrowsWhenNotUsedWithFileAdapter()
		{
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns("{C166A7E5-4F4C-4B02-A6F2-8BE07E1FA786}");

			Action(() => new FileOutboundTransportLocationBuilder().Execute(MessageContextMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Outbound file transport is required on this leg of the message exchange pattern.");
		}

		public FileOutboundTransportLocationBuilderFixture()
		{
			MessageContextMock = new MessageContextMock();
			MessageContextMock
				.Setup(c => c.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns("{9D0E4341-4CCE-4536-83FA-4A5040674AD6}");
		}

		private MessageContextMock MessageContextMock { get; }
	}
}
