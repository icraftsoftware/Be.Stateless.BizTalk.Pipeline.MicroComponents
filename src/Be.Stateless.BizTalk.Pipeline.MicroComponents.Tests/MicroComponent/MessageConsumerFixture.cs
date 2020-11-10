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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MessageConsumerFixture : MicroComponentFixture<MessageConsumer>
	{
		[Fact]
		public void MessageIsDrainedAndAbsorbed()
		{
			var content = Encoding.Unicode.GetBytes("Hello there.");
			using (var inputStream = new MemoryStream(content))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Object.BodyPart.GetOriginalDataStream().Position.Should().Be(0);

				var sut = new MessageConsumer();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				// message has been drained
				MessageMock.Object.BodyPart.GetOriginalDataStream().Position.Should().Be(content.Length);

				// generation of ack message has been discarded
				MessageMock.Verify(m => m.SetProperty(BtsProperties.AckRequired, false), Times.Once());
			}
		}
	}
}
