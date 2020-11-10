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
using Be.Stateless.BizTalk.MicroComponent;

namespace Be.Stateless.BizTalk.Unit.MicroComponent
{
	/// <summary>
	/// Base class that all fixtures written against <c>xUnit</c> should inherit when testing <see
	/// cref="IMicroComponent"/>-derived classes.
	/// </summary>
	/// <remarks>
	/// This is essentially a <c>NUnit</c> tailored version of the <see cref="MicroComponentFixtureBase{T}"/> base class.
	/// </remarks>
	/// <seealso cref="MicroComponentFixtureBase{T}"/>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public abstract class MicroComponentFixture<T> : MicroComponentFixtureBase<T> where T : IMicroComponent
	{
		protected MicroComponentFixture()
		{
			Initialize();
		}
	}
}
