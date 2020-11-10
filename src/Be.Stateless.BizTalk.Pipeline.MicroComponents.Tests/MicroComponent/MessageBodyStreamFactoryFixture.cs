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
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MessageBodyStreamFactoryFixture : MicroComponentFixture<MessageBodyStreamFactory>
	{
		[Fact]
		public void MessageFactoryPluginIsExecuted()
		{
			var sut = new MessageBodyStreamFactory { FactoryType = typeof(MessageFactoryMockWrapper) };

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageFactoryMockWrapper.MessageFactoryMock.Verify(m => m.Create(It.IsAny<IBaseMessage>()), Times.Once());
		}

		private class MessageFactoryMockWrapper : IMessageBodyStreamFactory
		{
			static MessageFactoryMockWrapper()
			{
				MessageFactoryMock = new Mock<IMessageBodyStreamFactory>();
				MessageFactoryMock
					.Setup(m => m.Create(It.IsAny<IBaseMessage>()))
					.Returns(new MemoryStream());
			}

			#region IMessageBodyStreamFactory Members

			public System.IO.Stream Create(IBaseMessage message)
			{
				return MessageFactoryMock.Object.Create(message);
			}

			#endregion

			public static readonly Mock<IMessageBodyStreamFactory> MessageFactoryMock;
		}
	}
}
