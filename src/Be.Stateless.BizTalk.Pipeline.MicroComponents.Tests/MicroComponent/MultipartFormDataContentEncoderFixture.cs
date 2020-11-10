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

using System.IO;
using Be.Stateless.BizTalk.Stream;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.IO;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MultipartFormDataContentEncoderFixture : MicroComponentFixture<MultipartFormDataContentEncoder>
	{
		[Fact]
		public void ContentBodyPartHasName()
		{
			using (var dataStream = new StringStream(XML_BODY))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				MessageMock.Setup(m => m.BodyPartName).Returns("implicit");

				var sut = new MultipartFormDataContentEncoder { UseBodyPartNameAsContentName = true };
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.ContentType.Should().MatchRegex(MULTIPART_FORM_DATA_BOUNDARY_PATTERN);
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					content.Should().MatchRegex("^(\\-\\-" + BOUNDARY_PATTERN + ")\r\nContent\\-Disposition: form\\-data; name=implicit\r\n[\\w\\W]+\r\n\\1\\-\\-\r\n$");
				}
			}
		}

		[Fact]
		public void ContentHasName()
		{
			using (var dataStream = new StringStream(XML_BODY))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var sut = new MultipartFormDataContentEncoder { ContentName = "explicit" };
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.ContentType.Should().MatchRegex(MULTIPART_FORM_DATA_BOUNDARY_PATTERN);
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					content.Should().MatchRegex("^(\\-\\-" + BOUNDARY_PATTERN + ")\r\nContent\\-Disposition: form\\-data; name=explicit\r\n[\\w\\W]+\r\n\\1\\-\\-\r\n$");
				}
			}
		}

		[Fact]
		public void ContentIsMultipart()
		{
			using (var dataStream = new StringStream(XML_BODY))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var sut = new MultipartFormDataContentEncoder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.ContentType.Should().MatchRegex(MULTIPART_FORM_DATA_BOUNDARY_PATTERN);
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					content.Should().MatchRegex("^(\\-\\-" + BOUNDARY_PATTERN + ")\r\nContent\\-Disposition: form\\-data\r\n[\\w\\W]+\r\n\\1\\-\\-\r\n$");
				}
			}
		}

		[Fact]
		public void WrapsMessageStreamInMultipartFormDataContentStream()
		{
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(new StringStream(XML_BODY));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);

			var sut = new MultipartFormDataContentEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Object.BodyPart.Data.Should().BeOfType<MultipartFormDataContentStream>();
		}

		private const string BOUNDARY_PATTERN = "[a-f\\d]{8}\\-[a-f\\d]{4}\\-[a-f\\d]{4}\\-[a-f\\d]{4}\\-[a-f\\d]{12}";
		private const string MULTIPART_FORM_DATA_BOUNDARY_PATTERN = "multipart/form\\-data; boundary=\"" + BOUNDARY_PATTERN + "\"";
		private const string XML_BODY = "<tns:Unknown xmlns:tns='urn:schemas.stateless.be:unknown:2012:11'><tns:Element>text</tns:Element></tns:Unknown>";
	}
}
