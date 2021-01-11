﻿#region Copyright & License

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
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Stream;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class SBMessagingContextPropagatorFixture : MicroComponentFixture<SBMessagingContextPropagator>
	{
		[Fact]
		public void BizTalkPropertiesAreOnlyPropagatedOutward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.Context.Read(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace)).Returns(Guid.NewGuid().ToString);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BrokeredMessageTypeIsNotPromotedInwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BrokeredMessageTypeIsPromotedInward()
		{
			const string messageType = "urn:ns#root";
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageType)).Returns(messageType);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, messageType), Times.Once);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CorrelationIdIsNotPromotedInwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Promote(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Context.Write(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CorrelationIdIsNotPropagatedOutwardWhenEmpty()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
			}
		}

		[Fact]
		public void CorrelationIdIsPromotedInward()
		{
			var token = Guid.NewGuid().ToString();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(token);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Promote(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, token), Times.Once);
			MessageMock.Verify(m => m.Context.Write(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void CorrelationIdIsPropagatedOutward()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				var token = Guid.NewGuid().ToString();
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.Context.Read(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace)).Returns(token);

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, token), Times.Once);
			}
		}

		[Fact]
		public void MessageTypeIsNotProbedWhenKnown()
		{
			using (var probeStreamMockInjectionScope = new ProbeStreamMockInjectionScope())
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				probeStreamMockInjectionScope.Mock.VerifyGet(ps => ps.MessageType, Times.Never());
			}
		}

		[Fact]
		public void MessageTypeIsNotPropagatedOutwardWhenPayloadIsNotXml()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("non xml payload")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MessageType, It.IsAny<string>()), Times.Never);
			}
		}

		[Fact]
		public void MessageTypeIsProbedWhenUnknown()
		{
			using (var probeStreamMockInjectionScope = new ProbeStreamMockInjectionScope())
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("non xml payload")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				probeStreamMockInjectionScope.Mock.VerifyGet(ps => ps.MessageType, Times.Once);
			}
		}

		[Fact]
		public void MessageTypeIsPropagatedOutward()
		{
			const string messageType = "urn:ns#root";
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns(messageType);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.MessageType, messageType), Times.Once);
		}

		[Fact]
		public void SBMessagingPropertiesAreOnlyPromotedInward()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(Guid.NewGuid().ToString);
				MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageType)).Returns("urn:ns#root");

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(
					m => m.Context.Promote(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, It.IsAny<string>()),
					Times.Never);
				MessageMock.Verify(m => m.Context.Write(SBMessagingProperties.CorrelationId.Name, BizTalkFactoryProperties.MessageType.Namespace, It.IsAny<string>()), Times.Never);
				MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
				MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			}
		}
	}
}
