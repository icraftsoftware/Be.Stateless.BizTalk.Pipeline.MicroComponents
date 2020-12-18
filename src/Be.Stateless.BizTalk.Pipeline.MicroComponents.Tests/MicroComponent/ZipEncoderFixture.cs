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
using System.IO.Compression;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.IO;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class ZipEncoderFixture : MicroComponentFixture<ZipEncoder>
	{
		[Fact]
		public void OutboundTransportLocationAbsolutePathIsKeptUnchanged()
		{
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"\file.txt");

			var sut = new ZipEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, @"\file.zip"));
		}

		[Fact]
		public void OutboundTransportLocationFileExtensionIsChangedToZip()
		{
			const string location = @"\\unc\folder\file.txt";
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(location);

			var sut = new ZipEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, location.Replace(".txt", ".zip")));
		}

		[Fact]
		public void OutboundTransportLocationWithOnlyFilename()
		{
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"file.txt");

			var sut = new ZipEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, "file.zip"));
		}

		[Fact]
		public void WrapsMessageStreamInZipOutputStream()
		{
			const string location = "sftp://host/folder/file.txt";
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(location);
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(new StringStream("content"));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);

			var sut = new ZipEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Object.BodyPart.Data.Should().BeOfType<ZipOutputStream>();
			var archive = new ZipArchive(MessageMock.Object.BodyPart.Data);
			archive.Entries[0].Name.Should().Be(System.IO.Path.GetFileName(location));
		}

		[Fact]
		public void ZipEntryNameIsDerivedFromOutboundTransportLocation()
		{
			var sut = new ZipEncoder();

			Invoking(() => sut.Execute(PipelineContextMock.Object, MessageMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("BizTalkFactoryProperties.OutboundTransportLocation has to be set in context in order to determine zip entry name.");
		}
	}
}
