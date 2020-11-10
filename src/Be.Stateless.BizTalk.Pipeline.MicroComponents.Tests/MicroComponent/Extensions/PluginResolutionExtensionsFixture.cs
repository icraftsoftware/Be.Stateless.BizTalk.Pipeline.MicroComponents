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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using FluentAssertions;
using Microsoft.BizTalk.Message.Interop;
using Xunit;
using MessageMock = Be.Stateless.BizTalk.Unit.Message.Mock<Microsoft.BizTalk.Message.Interop.IBaseMessage>;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	public class PluginResolutionExtensionsFixture
	{
		[Fact]
		public void AsPluginReturnsInstance()
		{
			var type = typeof(ContextualMessageBodyStreamFactory);

			var resolvedPlugin = type.AsPlugin<IMessageBodyStreamFactory>();

			resolvedPlugin.Should().NotBeNull().And.BeAssignableTo<IMessageBodyStreamFactory>();
		}

		[Fact]
		public void AsPluginReturnsNull()
		{
			var resolvedPluginType = ((Type) null).AsPlugin<IMessageBodyStreamFactory>();

			resolvedPluginType.Should().BeNull();
		}

		[Fact]
		public void AsPluginThrowsNothingWhenExpectedRuntimeType()
		{
			var type = typeof(ContextualMessageBodyStreamFactory);

			Action(() => type.AsPlugin<IMessageBodyStreamFactory>()).Should().NotThrow();
		}

		[Fact]
		public void AsPluginThrowsWhenNotExpectedRuntimeType()
		{
			var type = GetType();

			Action(() => type.AsPlugin<IMessageBodyStreamFactory>()).Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"The plugin type '{GetType().AssemblyQualifiedName}' does not support the type '{typeof(IMessageBodyStreamFactory).AssemblyQualifiedName}'.");
		}

		[Fact]
		public void OfPluginTypeReturnsNull()
		{
			var resolvedPluginType = ((Type) null).OfPluginType<IMessageBodyStreamFactory>();

			resolvedPluginType.Should().BeNull();
		}

		[Fact]
		public void OfPluginTypeReturnsPluginType()
		{
			var type = typeof(ContextualMessageBodyStreamFactory);

			var resolvedPluginType = type.OfPluginType<IMessageBodyStreamFactory>();

			resolvedPluginType.Should().BeSameAs(type);
		}

		[Fact]
		public void OfPluginTypeThrowsNothingWhenExpectedRuntimeType()
		{
			var type = typeof(ContextualMessageBodyStreamFactory);

			Action(() => type.OfPluginType<IMessageBodyStreamFactory>()).Should().NotThrow();
		}

		[Fact]
		public void OfPluginTypeThrowsWhenNotExpectedRuntimeType()
		{
			var type = GetType();

			Action(() => type.OfPluginType<IMessageBodyStreamFactory>()).Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"The plugin type '{GetType().AssemblyQualifiedName}' does not support the type '{typeof(IMessageBodyStreamFactory).AssemblyQualifiedName}'.");
		}

		[Fact]
		public void ResolvePluginTypeReturnsConfiguredPluginType()
		{
			var messageMock = new MessageMock();

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, typeof(ConfiguredMessageBodyStreamFactory));

			resolvedPluginType.Should().Be(typeof(ConfiguredMessageBodyStreamFactory));
		}

		[Fact]
		public void ResolvePluginTypeReturnsContextualPluginType()
		{
			var messageMock = new MessageMock();
			messageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName))
				.Returns(typeof(ContextualMessageBodyStreamFactory).AssemblyQualifiedName);

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, typeof(ConfiguredMessageBodyStreamFactory));

			resolvedPluginType.Should().Be(typeof(ContextualMessageBodyStreamFactory));
		}

		[Fact]
		public void ResolvePluginTypeReturnsNull()
		{
			var messageMock = new MessageMock();

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, null);

			resolvedPluginType.Should().BeNull();
		}

		private class ContextualMessageBodyStreamFactory : IMessageBodyStreamFactory
		{
			#region IMessageBodyStreamFactory Members

			public System.IO.Stream Create(IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		private class ConfiguredMessageBodyStreamFactory : IMessageBodyStreamFactory
		{
			#region IMessageBodyStreamFactory Members

			public System.IO.Stream Create(IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}
