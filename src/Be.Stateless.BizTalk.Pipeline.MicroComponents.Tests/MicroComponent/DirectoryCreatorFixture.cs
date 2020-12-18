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
using Be.Stateless.BizTalk.Unit.MicroComponent;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class DirectoryCreatorFixture : MicroComponentFixture<DirectoryCreator>
	{
		[Fact]
		public void NoDirectoryToCreate()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(BaseMessageContextMicroComponentExtensions.FileAdapterOutboundTransportClassId.ToString("D"));
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns("file.txt");

			var sut = new DirectoryCreator();
			Invoking(() => sut.Execute(PipelineContextMock.Object, MessageMock.Object))
				.Should().Throw<ArgumentNullException>()
				.WithMessage("Value cannot be null.\r\nParameter name: path");
		}

		[Fact]
		public void UnsupportedOutboundTransportType()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns("ee0e71a6-8945-4dd3-8770-f9e5495ddc7b");

			var sut = new DirectoryCreator();
			Invoking(() => sut.Execute(PipelineContextMock.Object, MessageMock.Object))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Outbound file transport is required on this leg of the message exchange pattern.");
		}
	}
}
