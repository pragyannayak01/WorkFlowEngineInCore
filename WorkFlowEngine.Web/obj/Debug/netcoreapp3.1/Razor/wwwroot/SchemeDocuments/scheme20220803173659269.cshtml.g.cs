#pragma checksum "D:\Projects\WFEWorkFlowEngine2023\WorkFlowEngine.Web\wwwroot\SchemeDocuments\scheme20220803173659269.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "335da1fbb1a58e35c6a582fbb5cf1e3c386ddefa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.wwwroot_SchemeDocuments_scheme20220803173659269), @"mvc.1.0.view", @"/wwwroot/SchemeDocuments/scheme20220803173659269.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"335da1fbb1a58e35c6a582fbb5cf1e3c386ddefa", @"/wwwroot/SchemeDocuments/scheme20220803173659269.cshtml")]
    public class wwwroot_SchemeDocuments_scheme20220803173659269 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral(@"<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""father-lang""></span>  <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" id=""txtparent"" autocomplete=""off"" class=""form-control"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""ageondeath-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""number"" id=""txtageyear"" autocomplete=""off"" class=""form-control"" onKeyPress=""if(this.value.length==2) return false;"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""locationdeath-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <select id=""ddldist""");
            BeginWriteAttribute("asp-items", " asp-items=\"", 877, "\"", 946, 1);
#nullable restore
#line 25 "D:\Projects\WFEWorkFlowEngine2023\WorkFlowEngine.Web\wwwroot\SchemeDocuments\scheme20220803173659269.cshtml"
WriteAttributeValue("", 889, new SelectList(ViewBag.District, "DistId", "DistName"), 889, 57, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"form-control\">\r\n            <option");
            BeginWriteAttribute("value", " value=\"", 990, "\"", 998, 0);
            EndWriteAttribute();
            WriteLiteral(@">Select</option>
        </select>
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"" id=""divhospital"">
    <div class=""form-group"">
        <label>
            <span class=""hospitalname-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" autocomplete=""off"" id=""txtdhospital"" class=""form-control"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"" id=""divdother"">
    <div class=""form-group"">
        <label>
            <span class=""other-lang""></span> place name <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" autocomplete=""off"" id=""txtdother"" class=""form-control"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-6"">
    <div class=""form-group"">
        <label>
            <span class=""typetest-lang""></span> <span class=""text-danger"">*</span>
        </label>

        <select id=""ddltypeoftest"" class=""form-control"">
            <option");
            BeginWriteAttribute("value", " value=\"", 1961, "\"", 1969, 0);
            EndWriteAttribute();
            WriteLiteral(@">Select</option>
            <option value=""1"">RT-PCR</option>
            <option value=""2"">Rapid Antigen</option>
            <option value=""3"">Any Medical or Clinical Test</option>
            <option value=""4"">Clinically Diagnosed</option>
        </select>
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""causeofdeath-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" autocomplete=""off"" id=""txtcauseofdeath"" class=""form-control"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""regno-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" id=""txtadeathregdno"" autocomplete=""off"" class=""form-control"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""state-lang""></span> <");
            WriteLiteral("span class=\"text-danger\">*</span>\r\n        </label><br />\r\n        <select id=\"ddlstate\"");
            BeginWriteAttribute("asp-items", "  asp-items=\"", 3082, "\"", 3152, 1);
#nullable restore
#line 82 "D:\Projects\WFEWorkFlowEngine2023\WorkFlowEngine.Web\wwwroot\SchemeDocuments\scheme20220803173659269.cshtml"
WriteAttributeValue("", 3095, new SelectList(ViewBag.States, "StateId", "StateName"), 3095, 57, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"form-control\">\r\n            <option");
            BeginWriteAttribute("value", " value=\"", 3196, "\"", 3204, 0);
            EndWriteAttribute();
            WriteLiteral(@">Select</option>
        </select>
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""aadhaarno-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" id=""txtaadhaarno"" autocomplete=""off"" class=""form-control"" maxlength=""12"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""district-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <select id=""ddldistrict""");
            BeginWriteAttribute("asp-items", "  asp-items=\"", 3797, "\"", 3868, 1);
#nullable restore
#line 100 "D:\Projects\WFEWorkFlowEngine2023\WorkFlowEngine.Web\wwwroot\SchemeDocuments\scheme20220803173659269.cshtml"
WriteAttributeValue("", 3810, new SelectList(ViewBag.Districts, "DistId", "DistName"), 3810, 58, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"form-control\">\r\n            <option");
            BeginWriteAttribute("value", " value=\"", 3912, "\"", 3920, 0);
            EndWriteAttribute();
            WriteLiteral(@">Select</option>
        </select>
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""address-lang""></span>
        </label>
        <input type=""text"" id=""txthouseno"" autocomplete=""off"" class=""form-control"" maxlength=""50"">
    </div>
</div>
<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""relationdeceased-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <select id=""ddlrelation""  class=""form-control"">
            <option");
            BeginWriteAttribute("Value", " Value=\"", 4524, "\"", 4532, 0);
            EndWriteAttribute();
            WriteLiteral(@">Select</option>
            <option Value=""Father"">Father</option>
            <option Value=""Mother"">Mother</option>
            <option Value=""Spouse"">Spouse</option>
            <option Value=""Son"">Son</option>
            <option Value=""Widowed Daugther-in-law"">Widowed Daugther-in-law</option>
            <option Value=""Dependant Sister"">Dependant Sister</option>
            <option Value=""Dependant Brother"">Dependant Brother</option>
            <option Value=""Unmarried Daughter"">Unmarried Daughter</option>
            <option Value=""Dependant Married Daughter"">Dependant Married Daughter</option>
        </select>
    </div>
</div>

<div class=""col-12 col-lg-6 col-xl-4"">
    <div class=""form-group"">
        <label>
            <span class=""mobileaplcnt-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""tel"" id=""txtmobno"" autocomplete=""off"" class=""form-control"" maxlength=""10"" pattern=""[0-9]{10}"" />
    </div>
</div>
<div class=""col-12 col-lg-6 col");
            WriteLiteral(@"-xl-4"">
    <div class=""form-group"">
        <label id=""documentno"">
            <span class=""aadhaarno-lang""></span> <span class=""text-danger"">*</span>
        </label>
        <input type=""text"" id=""txtdocumentno"" autocomplete=""off"" class=""form-control"" maxlength=""12"" />
    </div>
</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591