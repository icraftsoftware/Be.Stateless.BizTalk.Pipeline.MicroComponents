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
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.Xml.Serialization;
using Be.Stateless.Xml.Serialization.Extensions;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	public static class MicroComponentExtensions
	{
		public static void Serialize(this IMicroComponent component, XmlWriter writer)
		{
			if (component == null) throw new ArgumentNullException(nameof(component));
			var overrides = new XmlAttributeOverrides();
			overrides.Add(component.GetType(), new XmlAttributes { XmlRoot = new XmlRootAttribute(Constants.MICRO_COMPONENT_ELEMENT_NAME) });
			var serializer = CachingXmlSerializerFactory.Create(component.GetType(), overrides);
			using (var microComponentXmlWriter = new MicroComponentXmlWriter(writer, component))
			{
				serializer.SerializeWithoutDefaultNamespaces(microComponentXmlWriter, component);
			}
		}
	}
}
