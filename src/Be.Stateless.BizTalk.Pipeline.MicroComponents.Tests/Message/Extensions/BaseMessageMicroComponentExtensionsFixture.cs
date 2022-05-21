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

using Be.Stateless.BizTalk.Adapter.Transport;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public class BaseMessageMicroComponentExtensionsFixture
	{
		[Fact]
		public void OutboundTransportForInboundTraffic()
		{
			var messageMock = new MessageMock();

			messageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation))
				.Returns("inbound-direction");

			messageMock.Object.OutboundTransport().Should().BeSameAs(OutboundTransport.None);
		}

		[Fact]
		public void OutboundTransportForOutboundTraffic()
		{
			var messageMock = new MessageMock();

			messageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("outbound-direction");
			messageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportCLSID))
				.Returns(OutboundTransport.SBMessagingTransmitterClassId.ToString("D"));

			messageMock.Object.OutboundTransport()
				.Should().NotBeNull()
				.And.Subject.As<OutboundTransport>().IsSBMessagingTransmitter().Should().BeTrue();
		}
	}
}
