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

using System.Reflection;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.Resources;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class ZipDecoderFixture : MicroComponentFixture<ZipDecoder>
	{
		[Fact]
		public void WrapsMessageStreamInZipInputStream()
		{
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.BizTalk.Resources.Zip.message.zip"));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);

			var sut = new ZipDecoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Object.BodyPart.Data.Should().BeOfType<ZipInputStream>();
		}
	}
}
