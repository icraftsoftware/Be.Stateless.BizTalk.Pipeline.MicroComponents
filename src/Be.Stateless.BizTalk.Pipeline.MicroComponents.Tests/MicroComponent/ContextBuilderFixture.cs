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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class ContextBuilderFixture : MicroComponentFixture<ContextBuilder>
	{
		#region Setup/Teardown

		public ContextBuilderFixture()
		{
			BuilderMock = new();
		}

		#endregion

		[Fact]
		public void ContextBuilderPluginExecutionIsDeferred()
		{
			using (var inputStream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextBuilder {
					BuilderType = typeof(DummyBuilder),
					ExecutionTime = PluginExecutionTime.Deferred
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Never());

				MessageMock.Object.BodyPart.Data.Drain();
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Once());
			}
		}

		[Fact]
		public void ContextBuilderPluginExecutionIsImmediate()
		{
			using (var inputStream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextBuilder {
					BuilderType = typeof(DummyBuilder),
					ExecutionTime = PluginExecutionTime.Immediate
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Once());

				BuilderMock.Invocations.Clear();
				MessageMock.Object.BodyPart.Data.Drain();
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Never());
			}
		}

		private class DummyBuilder : IContextBuilder
		{
			#region IContextBuilder Members

			public void Execute(IBaseMessageContext context)
			{
				BuilderMock.Object.Execute(context);
			}

			#endregion
		}

		private static Mock<IContextBuilder> BuilderMock { get; set; }
	}
}
