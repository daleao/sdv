namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System;

#endregion using directives

internal record GMCMPage(string PageId, string PageTitleKey, Type ParentConfigType, bool LinkToParentPage = false);
