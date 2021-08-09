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

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.Xml.Extensions;
using Be.Stateless.Xml.Serialization;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	public static class MicroComponentXmlReaderExtensions
	{
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "typeof(IXmlSerializable).IsAssignableFrom(component) has been checked before cast.")]
		public static IMicroComponent DeserializeMicroComponent(this XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			reader.AssertStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
			var microComponentType = Type.GetType(reader.GetMandatoryAttribute(Constants.MICRO_COMPONENT_TYPE_ATTRIBUTE_NAME), true);
			if (!typeof(IMicroComponent).IsAssignableFrom(microComponentType))
				throw new ConfigurationErrorsException($"{microComponentType.AssemblyQualifiedName} does not implement {nameof(IMicroComponent)}.");

			// reset position to an element as required to call ReadSubtree()
			reader.MoveToElement();
			var microComponentXmlSubtree = reader.ReadSubtree();

			IMicroComponent component;
			if (typeof(IXmlSerializable).IsAssignableFrom(microComponentType))
			{
				component = (IMicroComponent) Activator.CreateInstance(microComponentType);
				// relieve micro components from having to deal with surrounding mComponent XML element
				microComponentXmlSubtree.MoveToContent();
				reader.ReadStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
				((IXmlSerializable) component).ReadXml(microComponentXmlSubtree);
			}
			else
			{
				var overrides = new XmlAttributeOverrides();
				overrides.Add(microComponentType, new() { XmlRoot = new(Constants.MICRO_COMPONENT_ELEMENT_NAME) });
				var serializer = CachingXmlSerializerFactory.Create(microComponentType, overrides);
				component = (IMicroComponent) serializer.Deserialize(microComponentXmlSubtree);
			}

			reader.Skip();
			return component;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		public static bool IsMicroComponent(this XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			return reader.IsStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
		}
	}
}
