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

namespace Be.Stateless.BizTalk.Component
{
	[SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "HResult are uint")]
	[SuppressMessage("Design", "CA1008:Enums should have zero value")]
	public enum HResult : uint
	{
		/// <summary>
		/// Error Code = 'Finding the document specification by message type "..." failed. Verify the schema deployed properly.'
		/// </summary>
		ErrorSchemaNotFound = 0xC0C01300 // -2147024774
	}
}
