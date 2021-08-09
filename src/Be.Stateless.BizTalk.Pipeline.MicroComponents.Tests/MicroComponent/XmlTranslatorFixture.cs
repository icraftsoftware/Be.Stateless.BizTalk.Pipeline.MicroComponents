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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.Text.Extensions;
using BTS;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class XmlTranslatorFixture : MicroComponentFixture<XmlTranslator>
	{
		[Fact]
		public void BuildXmlTranslationSetWithNoTranslationInContext()
		{
			var sut = new XmlTranslator {
				Translations = new() {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
					}
				}
			};

			sut.BuildXmlTranslationSet(MessageMock.Object).Should().Be(
				new XmlTranslationSet {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
					}
				});
		}

		[Fact]
		public void BuildXmlTranslationSetWithTranslationInContext()
		{
			MessageMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.XmlTranslations))
				.Returns(
					XmlTranslationSetConverter.Serialize(
						new() {
							Override = true,
							Items = new[] {
								new XmlNamespaceTranslation("sourceUrn5", "urn05")
							}
						}));

			var sut = new XmlTranslator {
				Translations = new() {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn2", "urn:test2")
					}
				}
			};

			sut.BuildXmlTranslationSet(MessageMock.Object).Should().Be(
				new XmlTranslationSet {
					Override = true,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn5", "urn05")
					}
				});
		}

		[Fact]
		public void ProbeAndPromoteMessageTypeIfKnown()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("urn:ns:translated#root"))
				.Returns(SchemaMetadata.For<soap_envelope_1__2.Envelope>().DocumentSpec);

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				var sut = new XmlTranslator {
					Translations = new() {
						Items = new[] {
							new XmlNamespaceTranslation("urn:ns", "urn:ns:translated")
						}
					}
				};
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, SchemaMetadata.For<soap_envelope_1__2.Envelope>().MessageType), Times.Once());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, SchemaMetadata.For<soap_envelope_1__2.Envelope>().DocumentSpec.DocSpecStrongName), Times.Once());
		}

		[Fact]
		public void ProbeAndSkipPromoteMessageTypeIfUnknown()
		{
			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				var sut = new XmlTranslator {
					Translations = new() {
						Items = new[] {
							new XmlNamespaceTranslation("urn:ns", "urn:ns:translated")
						}
					}
				};
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void RoundTripXmlSerialization()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new() { OmitXmlDeclaration = true }))
			{
				new XmlTranslator().Serialize(writer);
			}
			using (var reader = builder.GetReaderAtContent())
			{
				Invoking(() => reader.DeserializeMicroComponent()).Should().NotThrow();
			}
		}

		[Fact]
		public void TranslationsDefaultValue()
		{
			var sut = new XmlTranslator();
			sut.Translations.Should().Be(XmlTranslationSet.Empty);
		}
	}
}
