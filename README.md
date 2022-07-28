# Be.Stateless.BizTalk.Pipeline.MicroComponents

##### Build Pipelines

[![][pipeline.mr.badge]][pipeline.mr]

[![][pipeline.ci.badge]][pipeline.ci]

##### Latest Release

[![][nuget.badge]][nuget]

[![][nuget.unit.badge]][nuget.unit]

[![][nuget.nunit.badge]][nuget.nunit]

[![][nuget.xunit.badge]][nuget.xunit]

[![][release.badge]][release]

##### Release Preview

[![][nuget.preview.badge]][nuget.preview]

[![][nuget.unit.preview.badge]][nuget.unit.preview]

[![][nuget.nunit.preview.badge]][nuget.nunit.preview]

[![][nuget.xunit.preview.badge]][nuget.xunit.preview]

##### Documentation

[![][doc.main.badge]][doc.main]

[![][doc.this.badge]][doc.this]

[![][help.badge]][help]

[![][help.unit.badge]][help.unit]

[![][help.nunit.badge]][help.nunit]

[![][help.xunit.badge]][help.xunit]

## Overview

`Be.Stateless.BizTalk.Pipeline.MicroComponents` is part of the [BizTalk.Factory Runtime][biztalk.factory.runtime] Package. This component provides various general purpose micro components meant to be run by the `BizTalk.Factory`'s [MicroPipelineComponent][micro-pipeline-component], see [Be.Stateless.BizTalk.Pipeline.Components][be.stateless.biztalk.pipeline.components], [Be.Stateless.BizTalk.Pipelines][be.stateless.biztalk.pipelines], [Be.Stateless.BizTalk.Dsl.Pipeline][be.stateless.biztalk.dsl.pipeline], and [Be.Stateless.BizTalk.Dsl.Binding][be.stateless.biztalk.dsl.binding] for more information about micro pipelines.

> **Remark** Being part of the `BizTalk.Factory Runtime`, this component's deployment only requires its assembly to be GAC-deployed; it consequently has no `BizTalkMgmtDb` footprint at all as it is not deployed as a Microsoft BizTalk Server® resource.

<!-- badges -->

[doc.main.badge]: https://img.shields.io/static/v1?label=BizTalk.Factory%20SDK&message=User's%20Guide&color=8CA1AF&logo=readthedocs
[doc.main]: https://www.stateless.be/ "BizTalk.Factory SDK User's Guide"
[doc.this.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.Pipeline.MicroComponents&message=User's%20Guide&color=8CA1AF&logo=readthedocs
[doc.this]: https://www.stateless.be/BizTalk/Pipeline/MicroComponents "Be.Stateless.BizTalk.Pipeline.MicroComponents User's Guide"
[github.badge]: https://img.shields.io/static/v1?label=Repository&message=Be.Stateless.BizTalk.Pipeline.MicroComponents&logo=github
[github]: https://github.com/icraftsoftware/Be.Stateless.BizTalk.Pipeline.MicroComponents "Be.Stateless.BizTalk.Pipeline.MicroComponents GitHub Repository"
[help.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.Pipeline.MicroComponents&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/Pipeline/MicroComponents/README.md "Be.Stateless.BizTalk.Pipeline.MicroComponents Developer Help"
[help.nunit.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help.nunit]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/Pipeline/MicroComponents/NUnit/README.md "Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit Developer Help"
[help.unit.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help.unit]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/Pipeline/MicroComponents/Unit/README.md "Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit Developer Help"
[help.xunit.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help.xunit]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/Pipeline/MicroComponents/XUnit/README.md "Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit Developer Help"
[nuget.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.Pipeline.MicroComponents.svg?label=Be.Stateless.BizTalk.Pipeline.MicroComponents&style=flat&logo=nuget
[nuget]: https://www.nuget.org/packages/Be.Stateless.BizTalk.Pipeline.MicroComponents "Be.Stateless.BizTalk.Pipeline.MicroComponents NuGet Package"
[nuget.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.Pipeline.MicroComponents?logo=nuget
[nuget.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.Pipeline.MicroComponents&protocolType=NuGet "Be.Stateless.BizTalk.Pipeline.MicroComponents Preview NuGet Package"
[nuget.nunit.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit.svg?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit&style=flat&logo=nuget
[nuget.nunit]: https://www.nuget.org/packages/Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit "Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit NuGet Package"
[nuget.nunit.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit?logo=nuget
[nuget.nunit.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit&protocolType=NuGet "Be.Stateless.BizTalk.Pipeline.MicroComponents.NUnit Preview NuGet Package"
[nuget.unit.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit.svg?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit&style=flat&logo=nuget
[nuget.unit]: https://www.nuget.org/packages/Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit "Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit NuGet Package"
[nuget.unit.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit?logo=nuget
[nuget.unit.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit&protocolType=NuGet "Be.Stateless.BizTalk.Pipeline.MicroComponents.Unit Preview NuGet Package"
[nuget.xunit.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit.svg?label=Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit&style=flat&logo=nuget
[nuget.xunit]: https://www.nuget.org/packages/Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit "Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit NuGet Package"
[nuget.xunit.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit?logo=nuget
[nuget.xunit.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit&protocolType=NuGet "Be.Stateless.BizTalk.Pipeline.MicroComponents.XUnit Preview NuGet Package"
[pipeline.ci.badge]: https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.BizTalk.Pipeline.MicroComponents%20Continuous%20Integration?branchName=master&label=Continuous%20Integration%20Build
[pipeline.ci]: https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=66&branchName=master "Be.Stateless.BizTalk.Pipeline.MicroComponents Continuous Integration Build Pipeline"
[pipeline.mr.badge]: https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.BizTalk.Pipeline.MicroComponents%20Manual%20Release?branchName=master&label=Manual%20Release%20Build
[pipeline.mr]: https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=67&branchName=master "Be.Stateless.BizTalk.Pipeline.MicroComponents Manual Release Build Pipeline"
[release.badge]: https://img.shields.io/github/v/release/icraftsoftware/Be.Stateless.BizTalk.Pipeline.MicroComponents?label=Release&logo=github
[release]: https://github.com/icraftsoftware/Be.Stateless.BizTalk.Pipeline.MicroComponents/releases/latest "Be.Stateless.BizTalk.Pipeline.MicroComponents Release"

<!-- links -->

[biztalk.factory.runtime]: https://www.stateless.be/BizTalk/Factory/Runtime "BizTalk.Factory Runtime"
[be.stateless.biztalk.dsl.binding]: https://www.stateless.be/BizTalk/Dsl/Binding/
[be.stateless.biztalk.dsl.pipeline]: https://www.stateless.be/BizTalk/Dsl/Pipeline/
[be.stateless.biztalk.pipeline.components]: https://www.stateless.be/BizTalk/Pipeline/Components/
[be.stateless.biztalk.pipelines]: https://www.stateless.be/BizTalk/Pipelines/
[micro-pipeline-component]: https://github.com/icraftsoftware/Be.Stateless.BizTalk.Pipeline.Components/blob/master/src/Be.Stateless.BizTalk.Pipeline.Components/Component/MicroPipelineComponent.cs

<!--
cSpell:ignore BizTalkMgmtDb
-->
