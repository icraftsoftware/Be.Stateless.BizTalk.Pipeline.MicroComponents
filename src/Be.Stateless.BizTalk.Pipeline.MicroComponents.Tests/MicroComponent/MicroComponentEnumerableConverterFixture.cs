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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Dummies.MicroComponent;
using Be.Stateless.BizTalk.Schema.Annotation;
using Be.Stateless.BizTalk.Stream;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MicroComponentEnumerableConverterFixture
	{
		[Fact]
		public void CanConvertFrom()
		{
			var sut = new MicroComponentEnumerableConverter();
			sut.CanConvertFrom(typeof(string)).Should().BeTrue();
		}

		[Fact]
		public void CanConvertTo()
		{
			var sut = new MicroComponentEnumerableConverter();
			sut.CanConvertTo(typeof(string)).Should().BeTrue();
		}

		[Fact]
		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ConvertFrom()
		{
			var xml = $@"<mComponents>
  <mComponent name='{typeof(DummyMicroComponentOne).AssemblyQualifiedName}'>
    <Property-One>1</Property-One>
    <Property-Two>2</Property-Two>
  </mComponent>
  <mComponent name='{typeof(DummyMicroComponentTwo).AssemblyQualifiedName}' >
    <Property-Six>6</Property-Six>
    <Property-Ten>9</Property-Ten>
  </mComponent>
  <mComponent name='{typeof(DummyMicroComponentTen).AssemblyQualifiedName}'>
    <Encoding>utf-8</Encoding>
    <Index>10</Index>
    <Requirements>Default</Requirements>
    <Name>DummyTen</Name>
    <Plugin>{typeof(DummyXmlTranslator).AssemblyQualifiedName}</Plugin>
  </mComponent>
</mComponents>";

			var sut = new MicroComponentEnumerableConverter();

			var result = sut.ConvertFrom(xml) as IEnumerable<IMicroComponent>;

			result.Should().NotBeNull().And.BeEquivalentTo(
				new DummyMicroComponentOne { One = "1", Two = "2" },
				new DummyMicroComponentTwo { Six = "6", Ten = "9" },
				new DummyMicroComponentTen {
					Encoding = new UTF8Encoding(false),
					Index = 10,
					Requirements = XmlTranslationRequirements.Default,
					Name = "DummyTen",
					Plugin = typeof(DummyXmlTranslator)
				});
		}

		[Fact]
		public void ConvertFromEmpty()
		{
			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertFrom(string.Empty).Should().BeSameAs(Enumerable.Empty<IMicroComponent>());
		}

		[Fact]
		public void ConvertFromNull()
		{
			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertFrom(null).Should().BeSameAs(Enumerable.Empty<IMicroComponent>());
		}

		[Fact]
		public void ConvertTo()
		{
			var list = new List<IMicroComponent> {
				new DummyMicroComponentOne(),
				new DummyMicroComponentTwo(),
				new DummyMicroComponentTen()
			};

			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertTo(list, typeof(string)).Should().Be(
				"<mComponents>"
				+ $"<mComponent name='{typeof(DummyMicroComponentOne).AssemblyQualifiedName}'>"
				+ "<Property-One>one</Property-One>"
				+ "<Property-Two>two</Property-Two>"
				+ "</mComponent>"
				+ $"<mComponent name='{typeof(DummyMicroComponentTwo).AssemblyQualifiedName}'>"
				+ "<Property-Six>six</Property-Six>"
				+ "<Property-Ten>ten</Property-Ten>"
				+ "</mComponent>"
				+ $"<mComponent name='{typeof(DummyMicroComponentTen).AssemblyQualifiedName}'>"
				+ "<Encoding>utf-8 with signature</Encoding>"
				+ "<Index>10</Index>"
				+ "<Name>DummyTen</Name>"
				+ $"<Plugin>{typeof(DummyContextPropertyExtractor).AssemblyQualifiedName}</Plugin>"
				+ "<Requirements>AbsorbXmlDeclaration TranslateAttributeNamespace</Requirements>"
				+ "</mComponent>"
				+ "</mComponents>");
		}

		[Fact]
		public void ConvertToNull()
		{
			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertTo(Enumerable.Empty<IMicroComponent>(), typeof(string)).Should().BeNull();
		}

		[Fact]
		public void DeserializeComplexTypeWithCustomXmlSerialization()
		{
			var xml = $@"<mComponents>
  <mComponent name='{typeof(DummyContextPropertyExtractor).AssemblyQualifiedName}'>
    <Enabled>true</Enabled>
    <Extractors>
      <s0:Properties xmlns:s0='urn:schemas.stateless.be:biztalk:annotations:2013:01' xmlns:s1='urn'>
        <s1:Property1 xpath='*/some-node' />
        <s1:Property2 promoted='true' xpath='*/other-node' />
      </s0:Properties>
    </Extractors>
  </mComponent>
  <mComponent name='{typeof(DummyMicroComponentOne).AssemblyQualifiedName}'>
    <Property-One>1</Property-One>
    <Property-Two>2</Property-Two>
  </mComponent>
</mComponents>";

			var sut = new MicroComponentEnumerableConverter();

			var deserialized = sut.ConvertFrom(xml) as IMicroComponent[];

			deserialized.Should().BeEquivalentTo(
				new DummyContextPropertyExtractor {
					Enabled = true,
					Extractors = new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node"),
						new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
					}
				},
				new DummyMicroComponentOne { One = "1", Two = "2" }
			);
		}

		[Fact]
		public void DeserializeComplexTypeWithDefaultXmlSerialization()
		{
			var xml = $@"<mComponents>
  <mComponent name='{typeof(DummyXmlTranslator).AssemblyQualifiedName}'>
    <Enabled>true</Enabled>
    <xt:Translations override='false' xmlns:xt='urn:schemas.stateless.be:biztalk:translations:2013:07'>
      <xt:NamespaceTranslation matchingPattern='sourceUrn1' replacementPattern='urn:test1' />
      <xt:NamespaceTranslation matchingPattern='sourceUrn5' replacementPattern='urn:test5' />
    </xt:Translations>
  </mComponent>
</mComponents>";

			var sut = new MicroComponentEnumerableConverter();

			var deserialized = sut.ConvertFrom(xml) as IMicroComponent[];

			deserialized.Should().BeEquivalentTo(
				new DummyXmlTranslator {
					Enabled = true,
					Translations = new XmlTranslationSet {
						Override = false,
						Items = new[] {
							new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
							new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
						}
					}
				});
		}

		[Fact]
		public void SerializeComplexTypeWithCustomXmlSerialization()
		{
			var component = new DummyContextPropertyExtractor {
				Enabled = true,
				Extractors = new[] {
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node"),
					new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
				}
			};

			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertTo(new IMicroComponent[] { component }, typeof(string)).Should().Be(
				"<mComponents>"
				+ $"<mComponent name='{typeof(DummyContextPropertyExtractor).AssemblyQualifiedName}'>"
				+ "<Enabled>true</Enabled>"
				+ "<Extractors>"
				+ "<s0:Properties xmlns:s0='urn:schemas.stateless.be:biztalk:annotations:2013:01' xmlns:s1='urn'>"
				+ "<s1:Property1 xpath='*/some-node' />"
				+ "<s1:Property2 mode='promote' xpath='*/other-node' />"
				+ "</s0:Properties>"
				+ "</Extractors>"
				+ "</mComponent>"
				+ "</mComponents>");
		}

		[Fact]
		public void SerializeComplexTypeWithDefaultXmlSerialization()
		{
			var component = new DummyXmlTranslator {
				Enabled = true,
				Translations = new XmlTranslationSet {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
					}
				}
			};

			var sut = new MicroComponentEnumerableConverter();

			sut.ConvertTo(new IMicroComponent[] { component }, typeof(string)).Should().Be(
				"<mComponents>"
				+ $"<mComponent name='{typeof(DummyXmlTranslator).AssemblyQualifiedName}'>"
				+ "<Enabled>true</Enabled>"
				+ "<Translations override='false' xmlns='urn:schemas.stateless.be:biztalk:translations:2013:07'>"
				+ "<NamespaceTranslation matchingPattern='sourceUrn1' replacementPattern='urn:test1' />"
				+ "<NamespaceTranslation matchingPattern='sourceUrn5' replacementPattern='urn:test5' />"
				+ "</Translations>"
				+ "</mComponent>"
				+ "</mComponents>");
		}
	}
}
