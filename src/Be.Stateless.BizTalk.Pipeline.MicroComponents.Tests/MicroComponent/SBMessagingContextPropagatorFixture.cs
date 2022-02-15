#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Namespaces;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Stream;
using BTS;
using Moq;
using SBMessaging;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class SBMessagingContextPropagatorFixture : MicroComponentFixture<SBMessagingContextPropagator>
	{
		[Fact]
		public void BizTalkCorrelationIdIsPropagatedOutward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");
			MessageMock.Setup(m => m.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)).Returns(CUSTOM_BROKERED_MESSAGE_NAMESPACE);

			var correlationId = Guid.NewGuid().ToString();
			MessageMock.Setup(m => m.Context.Read(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory)).Returns(correlationId);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, correlationId), Times.Once);
		}

		[Fact]
		public void BizTalkCorrelationIdIsPropagatedOutwardUnlessEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");
			MessageMock.Setup(m => m.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)).Returns(CUSTOM_BROKERED_MESSAGE_NAMESPACE);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BizTalkMessageTypeForOutboundMessagesIsProbed()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			using (var probeStreamMockInjectionScope = new ProbeStreamMockInjectionScope())
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("payload")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				probeStreamMockInjectionScope.Mock.VerifyGet(ps => ps.MessageType, Times.Once);
			}
		}

		[Fact]
		public void BizTalkMessageTypeForOutboundMessagesIsProbedUnlessAlreadyKnown()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");
			MessageMock.Setup(m => m.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)).Returns(CUSTOM_BROKERED_MESSAGE_NAMESPACE);
			using (var probeStreamMockInjectionScope = new ProbeStreamMockInjectionScope())
			{
				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				probeStreamMockInjectionScope.Mock.VerifyGet(ps => ps.MessageType, Times.Never());
			}
		}

		[Fact]
		public void BizTalkMessageTypeIsPropagatedOutward()
		{
			const string messageType = "urn:ns#root";
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns(messageType);
			MessageMock.Setup(m => m.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)).Returns(CUSTOM_BROKERED_MESSAGE_NAMESPACE);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Write(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE, messageType), Times.Once);
		}

		[Fact]
		public void BizTalkMessageTypeIsPropagatedOutwardUnlessUnknownAndNotXml()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("non xml payload")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new SBMessagingContextPropagator();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Verify(m => m.Context.Write(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE, It.IsAny<string>()), Times.Never);
			}
		}

		[Fact]
		public void BizTalkPropertiesAreOnlyPropagatedOutward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.Context.Read(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory)).Returns(Guid.NewGuid().ToString);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Context.Promote(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Context.Write(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BrokeredCorrelationIdIsPropagatedInward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var correlationId = Guid.NewGuid().ToString();
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(correlationId);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Promote(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory, correlationId), Times.Once);
		}

		[Fact]
		public void BrokeredCorrelationIdIsPropagatedInwardUnlessEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Promote(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BrokeredMessageTypeIsNotPropagatedInward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.Context.Read(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void BrokeredPropertiesAreOnlyPropagatedInward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("ns#root");
			MessageMock.Setup(m => m.GetProperty(WcfProperties.CustomBrokeredPropertyNamespace)).Returns(CUSTOM_BROKERED_MESSAGE_NAMESPACE);

			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(Guid.NewGuid().ToString);
			MessageMock.Setup(m => m.Context.Read(nameof(MessageType), CUSTOM_BROKERED_MESSAGE_NAMESPACE)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Context.Promote(nameof(CorrelationId), PropertySchemaNamespaces.BizTalkFactory, It.IsAny<string>()), Times.Never);
		}

		private const string CUSTOM_BROKERED_MESSAGE_NAMESPACE = "urn:custom-brokered-message-namespace";
	}
}
