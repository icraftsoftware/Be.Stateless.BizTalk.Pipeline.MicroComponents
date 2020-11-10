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
using System.Xml.Serialization;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Xml.Extensions;
using Be.Stateless.Xml.Serialization;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	public static class MicroPipelineComponentXmlReaderExtensions
	{
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "typeof(IXmlSerializable).IsAssignableFrom(component) has been checked before cast.")]
		public static IMicroComponent DeserializeMicroPipelineComponent(this XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			reader.AssertStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
			var microPipelineComponentType = Type.GetType(reader.GetMandatoryAttribute(Constants.MICRO_COMPONENT_TYPE_ATTRIBUTE_NAME), true);
			if (!typeof(IMicroComponent).IsAssignableFrom(microPipelineComponentType))
				throw new ConfigurationErrorsException($"{microPipelineComponentType.AssemblyQualifiedName} does not implement {nameof(IMicroComponent)}.");

			// reset position to an element as required to call ReadSubtree()
			reader.MoveToElement();
			var microPipelineComponentXmlSubtree = reader.ReadSubtree();

			IMicroComponent component;
			if (typeof(IXmlSerializable).IsAssignableFrom(microPipelineComponentType))
			{
				component = (IMicroComponent) Activator.CreateInstance(microPipelineComponentType);
				// relieve micro pipeline components from having to deal with surrounding mComponent XML element
				microPipelineComponentXmlSubtree.MoveToContent();
				reader.ReadStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
				((IXmlSerializable) component).ReadXml(microPipelineComponentXmlSubtree);
			}
			else
			{
				var overrides = new XmlAttributeOverrides();
				overrides.Add(microPipelineComponentType, new XmlAttributes { XmlRoot = new XmlRootAttribute(Constants.MICRO_COMPONENT_ELEMENT_NAME) });
				var serializer = CachingXmlSerializerFactory.Create(microPipelineComponentType, overrides);
				component = (IMicroComponent) serializer.Deserialize(microPipelineComponentXmlSubtree);
			}

			reader.Skip();
			return component;
		}

		public static bool IsMicroPipelineComponent(this XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			return reader.IsStartElement(Constants.MICRO_COMPONENT_ELEMENT_NAME);
		}
	}
}
