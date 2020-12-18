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
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MessageTypeExtractorFixture : MicroComponentFixture<MessageTypeExtractor>
	{
		[Fact]
		public void DoesNotWriteMessageTypeInContextForUnknownSchema()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new MessageTypeExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MessageType, It.IsAny<string>()), Times.Never);
			}
		}

		[Fact]
		public void WritesMessageTypeInContextForKnownSchema()
		{
			var schemaMetadata = SchemaMetadata.For<Any>();

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				PipelineContextMock.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root")).Returns(schemaMetadata.DocumentSpec);
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new MessageTypeExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MessageType, schemaMetadata.DocumentSpec.DocType), Times.Once);
			}
		}
	}
}
