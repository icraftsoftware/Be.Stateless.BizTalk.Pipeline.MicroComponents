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
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public static class PipelineContextExtensions
	{
		/// <summary>
		/// Returns the <see cref="ISchemaMetadata"/> associated to the XML schema of messages of a given <see
		/// cref="DocumentSpec"/> type.
		/// </summary>
		/// <param name="pipelineContext">
		/// The pipeline context from which the <see cref="DocumentSpec"/> can be queried.
		/// </param>
		/// <param name="docType">
		/// The <see cref="DocumentSpec"/> type of the messages for which the <see cref="ISchemaMetadata"/> are to be returned.
		/// </param>
		/// <returns>
		/// The <see cref="ISchemaMetadata"/> associated to the XML Schema.
		/// </returns>
		public static ISchemaMetadata GetSchemaMetadataByType(this IPipelineContext pipelineContext, string docType)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			var docSpec = pipelineContext.GetDocumentSpecByType(docType);
			var schemaType = Type.GetType(docSpec.DocSpecStrongName, true);
			return SchemaMetadata.For(schemaType);
		}

		/// <summary>
		/// Returns the <see cref="ISchemaMetadata"/> associated to the XML schema of messages of a given <see
		/// cref="DocumentSpec"/> type.
		/// </summary>
		/// <param name="pipelineContext">
		/// The pipeline context from which the <see cref="DocumentSpec"/> can be queried.
		/// </param>
		/// <param name="docType">
		/// The <see cref="DocumentSpec"/> type of the messages for which the <see cref="ISchemaMetadata"/> are to be returned.
		/// </param>
		/// <param name="throwOnError">
		/// <c>false</c> to swallow <see cref="COMException"/> and return a <see cref="SchemaMetadata.Unknown"/> should the
		/// document specification not to be found; it will however be logged as a warning. <c>true</c> to let any exception
		/// through.
		/// </param>
		/// <returns>
		/// The <see cref="ISchemaMetadata"/> associated to the XML Schema.
		/// </returns>
		public static ISchemaMetadata GetSchemaMetadataByType(this IPipelineContext pipelineContext, string docType, bool throwOnError)
		{
			if (throwOnError) return pipelineContext.GetSchemaMetadataByType(docType);
			var schemaType = pipelineContext.TryGetDocumentSpecByType(docType, out var documentSpec)
				? Type.GetType(documentSpec.DocSpecStrongName, false)
				: null;
			return SchemaMetadata.For(schemaType);
		}

		public static bool TryGetDocumentSpecByType(this IPipelineContext pipelineContext, string docType, out IDocumentSpec documentSpec)
		{
			if (pipelineContext == null) throw new ArgumentNullException(nameof(pipelineContext));
			try
			{
				documentSpec = docType.IsNullOrEmpty()
					? null
					: pipelineContext.GetDocumentSpecByType(docType);
				return documentSpec != null;
			}
			catch (COMException exception)
			{
				documentSpec = null;
				if ((uint) exception.ErrorCode == (uint) HResult.ErrorSchemaNotFound) return false;
				if (_logger.IsWarnEnabled) _logger.Warn($"{nameof(TryGetDocumentSpecByType)}({docType}) has failed.", exception);
				throw;
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(PipelineContextExtensions));
	}
}
