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
using System.Text;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Dummies.MicroComponent
{
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Required by XML serialization")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by XML serialization")]
	public class DummyMicroComponentTen : IMicroComponent
	{
		public DummyMicroComponentTen()
		{
			Encoding = new UTF8Encoding(true);
			Index = 10;
			Requirements = XmlTranslationRequirements.AbsorbXmlDeclaration | XmlTranslationRequirements.TranslateAttributeNamespace;
			Name = "DummyTen";
			Plugin = typeof(DummyContextPropertyExtractor);
		}

		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			throw new NotSupportedException();
		}

		#endregion

		[XmlElement(typeof(EncodingXmlSerializer))]
		public Encoding Encoding { get; set; }

		public int Index { get; set; }

		public string Name { get; set; }

		[XmlElement(typeof(RuntimeTypeXmlSerializer))]
		public Type Plugin { get; set; }

		public XmlTranslationRequirements Requirements { get; set; }
	}
}
