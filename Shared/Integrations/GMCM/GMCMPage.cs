namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System;

#endregion using directives

public record GMCMPage(string PageId, string PageTitleKey, Type ParentConfigType, bool LinkToParentPage = false);
