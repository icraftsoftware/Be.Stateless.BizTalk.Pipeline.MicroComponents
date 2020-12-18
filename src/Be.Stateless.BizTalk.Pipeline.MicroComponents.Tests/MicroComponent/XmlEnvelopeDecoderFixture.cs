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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.Text.Extensions;
using BTS;
using FluentAssertions;
using Microsoft.XLANGs.BaseTypes;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class XmlEnvelopeDecoderFixture : MicroComponentFixture<XmlEnvelopeDecoder>
	{
		[Fact]
		public void LeaveStreamWhenNotEnvelopeSchema()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				PipelineContextMock.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root")).Returns(SchemaMetadata.For<Any>().DocumentSpec);
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new XmlEnvelopeDecoder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().NotBeOfType<XmlEnvelopeDecodingStream>();
			}
		}

		[Fact]
		public void LeaveStreamWhenUnknownSchema()
		{
			using (var inputStream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new XmlEnvelopeDecoder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().NotBeOfType<XmlEnvelopeDecodingStream>();
			}
		}

		[Fact]
		public void ReplaceStreamWhenEnvelopeSchema()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				PipelineContextMock.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root")).Returns(SchemaMetadata.For<soap_envelope_1__2.Envelope>().DocumentSpec);
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new XmlEnvelopeDecoder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().BeOfType<XmlEnvelopeDecodingStream>();
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void RoundTripXmlSerialization()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new() { OmitXmlDeclaration = true }))
			{
				new XmlEnvelopeDecoder().Serialize(writer);
			}
			using (var reader = builder.GetReaderAtContent())
			{
				Invoking(() => reader.DeserializeMicroComponent()).Should().NotThrow();
			}
		}
	}
}
