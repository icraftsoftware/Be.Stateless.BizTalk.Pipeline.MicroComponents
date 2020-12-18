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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.MicroComponent;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Dummies.MicroComponent
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by XML serialization")]
	public class DummyMicroComponentTwo : IMicroComponent, IXmlSerializable
	{
		public DummyMicroComponentTwo()
		{
			Six = "six";
			Ten = "ten";
		}

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

		public void ReadXml(XmlReader reader)
		{
			Six = reader.ReadElementContentAsString("Property-Six", string.Empty);
			Ten = reader.ReadElementContentAsString("Property-Ten", string.Empty);
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Property-Six", Six);
			writer.WriteElementString("Property-Ten", Ten);
		}

		#endregion

		public string Six { get; set; }

		public string Ten { get; set; }
	}
}
