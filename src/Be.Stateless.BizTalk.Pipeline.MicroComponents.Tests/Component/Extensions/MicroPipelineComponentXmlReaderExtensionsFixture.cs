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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.IO;
using FluentAssertions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	public class MicroPipelineComponentXmlReaderExtensionsFixture
	{
		[Fact]
		public void DeserializeDummyMicroPipelineComponentWithCustomXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithCustomXmlSerialization);
			var xml = $"<mComponent name=\"{microPipelineComponentType.AssemblyQualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				microPipelineComponent.Should().BeOfType(microPipelineComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		public void DeserializeDummyMicroPipelineComponentWithDefaultXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithDefaultXmlSerialization);
			var xml = $"<mComponent name=\"{microPipelineComponentType.AssemblyQualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				microPipelineComponent.Should().BeOfType(microPipelineComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		public void DeserializeDummyMicroPipelineComponentWithVerboseCustomXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithCustomXmlSerialization);
			var xml = $"<mComponent name=\"{microPipelineComponentType.AssemblyQualifiedName}\"></mComponent>";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				microPipelineComponent.Should().BeOfType(microPipelineComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void DeserializeThrowsWhenNotMicroPipelineComponent()
		{
			var qualifiedName = GetType().AssemblyQualifiedName;
			var xml = $"<mComponent name=\"{qualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				Action(() => reader.DeserializeMicroPipelineComponent())
					.Should().Throw<ConfigurationErrorsException>()
					.WithMessage($"{qualifiedName} does not implement {nameof(IMicroComponent)}.");
			}
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithDefaultXmlSerialization : IMicroComponent
		{
			#region IMicroComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithCustomXmlSerialization : IMicroComponent, IXmlSerializable
		{
			#region IMicroComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IXmlSerializable Members

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader) { }

			public void WriteXml(XmlWriter writer)
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}
