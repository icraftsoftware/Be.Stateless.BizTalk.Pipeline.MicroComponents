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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Schema.Annotation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Dummies.MicroComponent
{
	public class DummyContextPropertyExtractor : IMicroComponent
	{
		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			throw new NotSupportedException();
		}

		#endregion

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public bool Enabled { get; set; }

		[XmlIgnore]
		public PropertyExtractorCollection Extractors { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("Extractors")]
		public PropertyExtractorCollectionSerializerSurrogate ExtractorSerializerSurrogate
		{
			get => new(Extractors);
			set => Extractors = value;
		}
	}
}
