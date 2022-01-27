#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Schema;
using FluentAssertions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	public class PipelineContextExtensionsFixture
	{
		[Fact]
		public void GetSchemaMetadataByTypeForKnownSchema()
		{
			var schemaMetadata = SchemaMetadata.For<Any>();

			var documentSpecMock = new Mock<IDocumentSpec>();
			documentSpecMock
				.Setup(m => m.DocSpecStrongName)
				.Returns(schemaMetadata.DocumentSpec.DocSpecStrongName);

			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(schemaMetadata.MessageType))
				.Returns(documentSpecMock.Object);

			var metadata = pipelineContextMock.Object.GetSchemaMetadataByType(schemaMetadata.MessageType, false);

			metadata.Should().BeSameAs(schemaMetadata);
		}

		[Fact]
		public void GetSchemaMetadataByTypeForString()
		{
			// this is an actual odd edge case that has been occurring on a production BTS server but not reproducible
			// IMHO pipelineContext.GetDocumentSpecByType("string") should throw instead of returning this DocSpecStrongName
			var documentSpecMock = new Mock<IDocumentSpec>();
			documentSpecMock
				.Setup(m => m.DocSpecStrongName)
				.Returns(typeof(string).AssemblyQualifiedName);

			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("string"))
				.Returns(documentSpecMock.Object);

			var metadata = pipelineContextMock.Object.GetSchemaMetadataByType("string", false);

			metadata.Should().BeSameAs(SchemaMetadata.Unknown);
		}

		[Fact]
		public void GetSchemaMetadataByTypeForUnknownDocSpecStrongName()
		{
			var schemaMetadata = SchemaMetadata.For<Any>();

			var documentSpecMock = new Mock<IDocumentSpec>();
			documentSpecMock
				.Setup(m => m.DocSpecStrongName)
				.Returns("Unknown.Type, UnknownAssembly");

			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(schemaMetadata.MessageType))
				.Returns(documentSpecMock.Object);

			var metadata = pipelineContextMock.Object.GetSchemaMetadataByType(schemaMetadata.MessageType, false);

			metadata.Should().BeSameAs(SchemaMetadata.Unknown);
		}

		[Fact]
		public void GetSchemaMetadataByTypeForUnknownSchema()
		{
			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Throws(new COMException("Finding the document specification by message type failed.", unchecked((int) HResult.ErrorSchemaNotFound)));

			var metadata = pipelineContextMock.Object.GetSchemaMetadataByType("urn:ns#root", false);

			metadata.Should().BeSameAs(SchemaMetadata.Unknown);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void TryGetDocumentSpecByTypeForNullOrEmptyDocType(string docType)
		{
			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock.Object.TryGetDocumentSpecByType(docType, out var documentSpec).Should().BeFalse();
			documentSpec.Should().BeNull();
		}

		[Fact]
		public void TryGetDocumentSpecByTypForKnownSchema()
		{
			var schemaMetadata = SchemaMetadata.For<Any>();

			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(schemaMetadata.MessageType))
				.Returns(schemaMetadata.DocumentSpec);

			pipelineContextMock.Object.TryGetDocumentSpecByType("http://schemas.microsoft.com/BizTalk/2003/Any#Root", out var documentSpec).Should().BeTrue();
			documentSpec.Should().BeEquivalentTo(schemaMetadata.DocumentSpec);
		}

		[Fact]
		public void TryGetDocumentSpecByTypForUnknownSchema()
		{
			var pipelineContextMock = new Mock<IPipelineContext>();
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Throws(new COMException("Finding the document specification by message type failed.", unchecked((int) HResult.ErrorSchemaNotFound)));

			pipelineContextMock.Object.TryGetDocumentSpecByType("urn:ns#root", out var documentSpec).Should().BeFalse();
			documentSpec.Should().BeNull();
		}
	}
}
