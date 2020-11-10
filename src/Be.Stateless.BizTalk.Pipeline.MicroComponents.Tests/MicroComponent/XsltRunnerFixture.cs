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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dummies.Transform;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Stream.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Stream;
using Be.Stateless.Reflection;
using FluentAssertions;
using Microsoft.BizTalk.Edi.BaseArtifacts;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class XsltRunnerFixture : MicroComponentFixture<XsltRunner>
	{
		[Fact]
		public void DoesNothingWhenNoXslt()
		{
			using (var probeStreamMockInjectionScope = new ProbeStreamMockInjectionScope())
			using (var transformStreamMockInjectionScope = new TransformStreamMockInjectionScope())
			{
				var sut = new XsltRunner();
				sut.MapType.Should().BeNull();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				probeStreamMockInjectionScope.Mock.VerifyGet(ps => ps.MessageType, Times.Never());
				transformStreamMockInjectionScope.Mock.Verify(ps => ps.Apply(It.IsAny<Type>()), Times.Never());
			}
		}

		[Fact]
		public void EncodingDefaultsToUtf8WithoutSignature()
		{
			new XsltRunner().Encoding.Should().Be(new UTF8Encoding(false));
		}

		[Fact]
		[SuppressMessage("ReSharper", "PossibleInvalidCastException")]
		public void ReplacesMessageOriginalDataStreamWithTransformResult()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("http://schemas.microsoft.com/Edi/EdifactServiceSchema#UNB"))
				.Returns(SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec);

			var sut = new XsltRunner {
				Encoding = Encoding.UTF8,
				MapType = typeof(IdentityTransform)
			};

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<UNB xmlns='http://schemas.microsoft.com/Edi/EdifactServiceSchema'></UNB>")))
			using (var transformedStream = dataStream.Transform().Apply(sut.MapType))
			using (var transformStreamMockInjectionScope = new TransformStreamMockInjectionScope())
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				transformStreamMockInjectionScope.Mock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMockInjectionScope.Mock.Object);
				transformStreamMockInjectionScope.Mock
					.Setup(ts => ts.Apply(sut.MapType, sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMockInjectionScope.Mock.VerifyAll();

				MessageMock.Object.BodyPart.Data.Should().BeOfType<MarkableForwardOnlyEventingReadStream>();
				Reflector.GetField((MarkableForwardOnlyEventingReadStream) MessageMock.Object.BodyPart.Data, "m_data").Should().BeSameAs(transformedStream);
			}
		}

		[Fact]
		public void XsltEntailsMessageTypeIsPromoted()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("http://schemas.microsoft.com/Edi/EdifactServiceSchema#UNB"))
				.Returns(SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec);

			var sut = new XsltRunner {
				Encoding = Encoding.UTF8,
				MapType = typeof(IdentityTransform)
			};

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<UNB xmlns='http://schemas.microsoft.com/Edi/EdifactServiceSchema'></UNB>")))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, SchemaMetadata.For<EdifactServiceSchema.UNB>().MessageType), Times.Once());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec.DocSpecStrongName), Times.Once());
		}

		[Fact]
		public void XsltEntailsMessageTypeIsPromotedOnlyIfOutputMethodIsXml()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("http://schemas.microsoft.com/Edi/EdifactServiceSchema#UNB"))
				.Returns(SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec);

			var sut = new XsltRunner {
				Encoding = Encoding.UTF8,
				MapType = typeof(AnyToText)
			};

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<UNB xmlns='http://schemas.microsoft.com/Edi/EdifactServiceSchema'></UNB>")))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, SchemaMetadata.For<EdifactServiceSchema.UNB>().MessageType), Times.Never());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec.DocSpecStrongName), Times.Never());
		}

		[Fact]
		public void XsltFromContextHasPrecedenceOverConfiguredOne()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("http://schemas.microsoft.com/Edi/EdifactServiceSchema#UNB"))
				.Returns(SchemaMetadata.For<EdifactServiceSchema.UNB>().DocumentSpec);

			var sut = new XsltRunner {
				Encoding = Encoding.UTF8,
				MapType = typeof(TransformBase)
			};

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<UNB xmlns='http://schemas.microsoft.com/Edi/EdifactServiceSchema'></UNB>")))
			using (var transformedStream = dataStream.Transform().Apply(typeof(IdentityTransform)))
			using (var transformStreamMockInjectionScope = new TransformStreamMockInjectionScope())
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				MessageMock
					.Setup(m => m.GetProperty(BizTalkFactoryProperties.MapTypeName))
					.Returns(typeof(IdentityTransform).AssemblyQualifiedName);

				var transformStreamMock = transformStreamMockInjectionScope.Mock;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(typeof(IdentityTransform), sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.Verify(ts => ts.Apply(sut.MapType, sut.Encoding), Times.Never());
				transformStreamMock.VerifyAll();
			}
		}
	}
}
