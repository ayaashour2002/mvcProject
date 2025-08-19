using Microsoft.AspNetCore.Razor.TagHelpers;
using RestaurantMS_test.Models; // عدل حسب مسار الـ MenuItem

[HtmlTargetElement("availability", Attributes = "is-available")]
public class AvailabilityTagHelper : TagHelper
{
    public bool IsAvailable { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.Attributes.SetAttribute("class", IsAvailable ? "text-green-600" : "text-red-600");
        output.Content.SetContent(IsAvailable ? "✅ Available" : "❌ Unavailable");
    }
}
