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

using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Kernel;
using Be.Stateless.BizTalk.Dummies.MicroComponent;
using FluentAssertions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class MicroPipelineFixture
	{
		[Fact]
		public void ExecuteRunsThroughMicroComponentEnumerable()
		{
			var pipelineContextMock = new Mock<IPipelineContext>();
			var messageMock1 = new Unit.Message.Mock<IBaseMessage>();
			var messageMock2 = new Unit.Message.Mock<IBaseMessage>();
			var messageMock3 = new Unit.Message.Mock<IBaseMessage>();

			var microComponentMockOne = new Mock<IMicroComponent>();
			microComponentMockOne
				.Setup(mc => mc.Execute(pipelineContextMock.Object, messageMock1.Object)).Returns(messageMock2.Object)
				.Verifiable();
			var microComponentMockTwo = new Mock<IMicroComponent>();
			microComponentMockTwo
				.Setup(mc => mc.Execute(pipelineContextMock.Object, messageMock2.Object)).Returns(messageMock3.Object)
				.Verifiable();

			var sut = new MicroPipeline {
				Components = new[] {
					microComponentMockOne.Object,
					microComponentMockTwo.Object
				}
			};

			sut.Execute(pipelineContextMock.Object, messageMock1.Object).Should().BeSameAs(messageMock3.Object);

			microComponentMockOne.Verify();
			microComponentMockTwo.Verify();
		}

		[Fact]
		public void ExecuteWhenMicroComponentEnumerableIsEmpty()
		{
			var messageMock = new Unit.Message.Mock<IBaseMessage>();
			var sut = new MicroPipeline();
			sut.Execute(new Mock<IPipelineContext>().Object, messageMock.Object).Should().BeSameAs(messageMock.Object);
		}

		[Fact]
		[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
		public void LoadConfiguration()
		{
			var specimenContext = new SpecimenContext(CreateAutoFixture());
			var microComponents = new[] {
				specimenContext.Create<IMicroComponent>(),
				specimenContext.Create<IMicroComponent>(),
				specimenContext.Create<IMicroComponent>()
			};
			var configuration = MicroComponentEnumerableConverter.Serialize(microComponents);

			var sut = new MicroPipeline();
			sut.LoadConfiguration(configuration);

			sut.Components.Should().BeEquivalentTo(microComponents);
		}

		[Fact]
		public void SaveConfiguration()
		{
			var specimenContext = new SpecimenContext(CreateAutoFixture());
			var microComponents = new[] {
				specimenContext.Create<IMicroComponent>(),
				specimenContext.Create<IMicroComponent>(),
				specimenContext.Create<IMicroComponent>()
			};

			var sut = new MicroPipeline { Components = microComponents };
			var configuration = sut.SaveConfiguration();

			configuration.Should().Be(MicroComponentEnumerableConverter.Serialize(microComponents));
		}

		private Fixture CreateAutoFixture()
		{
			var fixture = new Fixture();
			fixture.Register<IMicroComponent>(() => fixture.Create<DummyMicroComponentOne>());
			return fixture;
		}
	}
}
