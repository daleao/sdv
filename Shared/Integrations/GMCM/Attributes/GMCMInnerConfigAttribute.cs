namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Tells the GMCM generator to search inside a class for more config properties.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class GMCMInnerConfigAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMInnerConfigAttribute"/> class.</summary>
    public GMCMInnerConfigAttribute()
    {
        this.PageId = string.Empty;
        this.PageTitleKey = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="GMCMInnerConfigAttribute"/> class within a new GMCM page.</summary>
    /// <param name="pageId">The public ID of the page.</param>
    /// <param name="pageTitleKey">The translation key for the page title.</param>
    /// <param name="linkToParentPage">Whether to provide a link to the previous page at the top of the new page.</param>
    public GMCMInnerConfigAttribute(string pageId, string? pageTitleKey = null, bool linkToParentPage = false)
    {
        this.PageId = pageId;
        this.PageTitleKey = pageTitleKey ?? pageId;
        this.LinkToParentPage = linkToParentPage;
    }

    /// <summary>Gets the public ID of the page.</summary>
    public string PageId { get; }

    /// <summary>Gets the translation key.</summary>
    public string PageTitleKey { get; }

    /// <summary>Gets a value indicating whether to provide a link to the previous page at the top of the new apage.</summary>
    public bool LinkToParentPage { get; }
}
