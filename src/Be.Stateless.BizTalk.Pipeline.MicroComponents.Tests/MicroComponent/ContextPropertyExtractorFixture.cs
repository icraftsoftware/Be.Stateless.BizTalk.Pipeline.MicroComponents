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

using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schema.Annotation;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Schema;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using FluentAssertions;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class ContextPropertyExtractorFixture : MicroComponentFixture<ContextPropertyExtractor>
	{
		[Fact]
		public void BuildPropertyExtractorCollectionGivesPrecedenceToSchemaExtractorsOverPipelineExtractors()
		{
			// has to be called before ContextPropertyAnnotationMockInjectionScope overrides SchemaMetadata.For<>() factory method
			var schemaMetadata = SchemaMetadata.For<Any>();

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			using (var contextPropertyAnnotationMockInjectionScope = new PropertyExtractorAnnotationMockInjectionScope())
			{
				contextPropertyAnnotationMockInjectionScope.Extractors = new PropertyExtractorCollection(
					new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/to", ExtractionMode.Demote),
					new XPathExtractor(BtsProperties.Operation.QName, "/letter/*/salutations"));

				PipelineContextMock.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root")).Returns(schemaMetadata.DocumentSpec);
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(BtsProperties.OutboundTransportLocation.QName, "/letter/*/paragraph")
					}
				};
				var extractors = sut.BuildPropertyExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				extractors.Should().BeEquivalentTo(
					new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/to", ExtractionMode.Demote),
					new XPathExtractor(BtsProperties.Operation.QName, "/letter/*/salutations"),
					new XPathExtractor(BtsProperties.OutboundTransportLocation.QName, "/letter/*/paragraph")
				);
			}
		}

		[Fact]
		public void BuildPropertyExtractorCollectionYieldsPipelineExtractorsWhenNoSchemaExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(BtsProperties.OutboundTransportLocation.QName, "/letter/*/paragraph")
					}
				};
				var extractors = sut.BuildPropertyExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				extractors.Should().BeEquivalentTo(sut.Extractors);
			}
		}

		[Fact]
		public void Deserialize()
		{
			var xml = $"<mComponent name=\"{typeof(ContextPropertyExtractor).AssemblyQualifiedName}\"><Extractors>"
				+ PropertyExtractorCollectionConverter.Serialize(
					new PropertyExtractorCollection(
						ExtractorPrecedence.Pipeline,
						new ConstantExtractor(BizTalkFactoryProperties.ContextBuilderTypeName, "context-builder")))
				+ "</Extractors></mComponent>";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var propertyExtractor = (ContextPropertyExtractor) reader.DeserializeMicroComponent();
				propertyExtractor.Extractors.Precedence.Should().Be(ExtractorPrecedence.Pipeline);
				propertyExtractor.Extractors.Should().BeEquivalentTo(
					new ConstantExtractor(BizTalkFactoryProperties.ContextBuilderTypeName, "context-builder")
				);
			}
		}

		[Fact]
		public void DoesNothingWhenNoSchemaNorPipelineExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<unknown></unknown>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#unknown");

				var sut = new ContextPropertyExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().BeSameAs(inputStream);
				MessageMock.Object.BodyPart.Data.Should().NotBeOfType<XPathMutatorStream>();

				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Once); // probing for MessageType calls AsMarkable() which wraps stream

				MessageMock.Verify(m => m.Context.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
				MessageMock.Verify(m => m.Context.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			}
		}

		[Fact]
		public void MessageContextManipulationsAreDelegatedToPropertyExtractors()
		{
			const string content = "<s1:letter xmlns:s1=\"urn-one\">"
				+ "<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>"
				+ "<s1:body>"
				+ "<s1:paragraph>paragraph-one</s1:paragraph>"
				+ "<s1:footer>trail</s1:footer>"
				+ "</s1:body>" +
				"</s1:letter>";

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			using (var contextPropertyAnnotationMockInjectionScope = new PropertyExtractorAnnotationMockInjectionScope())
			{
				var propertyExtractorMock = new Mock<PropertyExtractor>(BizTalkFactoryProperties.OutboundTransportLocation.QName, ExtractionMode.Clear) {
					CallBase = true
				};
				var constantExtractorMock = new Mock<ConstantExtractor>(BtsProperties.OutboundTransportLocation.QName, "OutboundTransportLocation", ExtractionMode.Write) {
					CallBase = true
				};
				var xpathExtractorMock = new Mock<XPathExtractor>(BtsProperties.Operation.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph']", ExtractionMode.Write) {
					CallBase = true
				};

				contextPropertyAnnotationMockInjectionScope.Extractors = new PropertyExtractorCollection(
					propertyExtractorMock.Object,
					constantExtractorMock.Object
				);

				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn-one#letter");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(SBMessagingProperties.Label.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Promote),
						xpathExtractorMock.Object,
						new XPathExtractor(BizTalkFactoryProperties.MapTypeName.QName, "/*[local-name()='letter']/*/*[local-name()='footer']")
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				MessageMock.Object.BodyPart.Data.Drain();

				propertyExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context));
				constantExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context));
				xpathExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context, "paragraph-one", ref It.Ref<string>.IsAny));

				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, null));
				MessageMock.Verify(m => m.SetProperty(BtsProperties.Operation, "paragraph-one"));
				MessageMock.Verify(m => m.SetProperty(BtsProperties.OutboundTransportLocation, "OutboundTransportLocation"));
				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MapTypeName, "trail"));
				MessageMock.Verify(m => m.Promote(SBMessagingProperties.Label, "inquiry"));
			}
		}

		[Fact]
		public void OriginalDataStreamIsNotWrappedWhenThereIsNoXPathExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] { new ConstantExtractor(BtsProperties.Operation.QName, "operation.name") }
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().BeSameAs(inputStream);
				MessageMock.Object.BodyPart.Data.Should().NotBeOfType<XPathMutatorStream>();

				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Once); // probing for MessageType calls AsMarkable() which wraps stream
			}
		}

		[Fact]
		public void OriginalDataStreamIsWrappedInXPathMutatorStreamWhenThereIsXPathExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextPropertyExtractor {
					Extractors = new[] { new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation.QName, "/letter/*/from", ExtractionMode.Promote) }
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Should().NotBeSameAs(inputStream);
				MessageMock.Object.BodyPart.Data.Should().BeOfType<XPathMutatorStream>();

				// twice: 1st when probing for MessageType calls AsMarkable() which wraps stream, 2nd when wrapping in XPathMutatorStream
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Exactly(2));
			}
		}

		[Fact]
		public void Serialize()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var component = new ContextPropertyExtractor {
					Extractors = new PropertyExtractorCollection(
						ExtractorPrecedence.Pipeline,
						new ConstantExtractor(BizTalkFactoryProperties.XmlTranslations, "xml-translations"))
				};
				component.Serialize(writer);
			}
			builder.ToString().Should().Be(
				$"<mComponent name=\"{typeof(ContextPropertyExtractor).AssemblyQualifiedName}\"><Extractors>"
				+ PropertyExtractorCollectionConverter.Serialize(
					new PropertyExtractorCollection(
						ExtractorPrecedence.Pipeline,
						new ConstantExtractor(BizTalkFactoryProperties.XmlTranslations, "xml-translations")))
				+ "</Extractors></mComponent>");
		}
	}
}
