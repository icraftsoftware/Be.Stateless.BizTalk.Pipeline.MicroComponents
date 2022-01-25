﻿#region Copyright & License

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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class TransportRetriesDisablerFixture : MicroComponentFixture<TransportRetriesDisabler>
	{
		[Fact]
		public void DisableRetryIfPropertyExistsInContextAndIsTrue()
		{
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.DisableTransportRetries)).Returns(true);

			var sut = new TransportRetriesDisabler();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BtsProperties.RetryCount, 0), Times.Once);
		}

		[Fact]
		public void DoesNotDisableRetryIfPropertyDoesNotExistInContext()
		{
			var sut = new TransportRetriesDisabler();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BtsProperties.RetryCount, It.IsAny<int>()), Times.Never);
		}

		[Fact]
		public void DoesNotDisableRetryIfPropertyExistsInContextAndIsFalse()
		{
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.DisableTransportRetries)).Returns(false);

			var sut = new TransportRetriesDisabler();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BtsProperties.RetryCount, It.IsAny<int>()), Times.Never);
		}
	}
}
