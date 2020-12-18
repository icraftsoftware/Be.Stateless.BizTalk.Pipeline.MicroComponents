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

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.BizTalk.Dummies.MicroComponent;
using Be.Stateless.IO;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	public class MicroComponentXmlReaderExtensionsFixture
	{
		[Fact]
		public void DeserializeDummyMicroComponentWithCustomXmlSerialization()
		{
			var microComponentType = typeof(DummyMicroComponentWithCustomXmlSerialization);
			var xml = $"<mComponent name=\"{microComponentType.AssemblyQualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microComponent = reader.DeserializeMicroComponent();
				microComponent.Should().BeOfType(microComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		public void DeserializeDummyMicroComponentWithDefaultXmlSerialization()
		{
			var microComponentType = typeof(DummyMicroComponentWithDefaultXmlSerialization);
			var xml = $"<mComponent name=\"{microComponentType.AssemblyQualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microComponent = reader.DeserializeMicroComponent();
				microComponent.Should().BeOfType(microComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		public void DeserializeDummyMicroComponentWithVerboseCustomXmlSerialization()
		{
			var microComponentType = typeof(DummyMicroComponentWithCustomXmlSerialization);
			var xml = $"<mComponent name=\"{microComponentType.AssemblyQualifiedName}\"></mComponent>";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microComponent = reader.DeserializeMicroComponent();
				microComponent.Should().BeOfType(microComponentType);
				reader.EOF.Should().BeTrue();
			}
		}

		[Fact]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void DeserializeThrowsWhenNotMicroComponent()
		{
			var qualifiedName = GetType().AssemblyQualifiedName;
			var xml = $"<mComponent name=\"{qualifiedName}\" />";
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				Action(() => reader.DeserializeMicroComponent())
					.Should().Throw<ConfigurationErrorsException>()
					.WithMessage($"{qualifiedName} does not implement {nameof(IMicroComponent)}.");
			}
		}
	}
}
