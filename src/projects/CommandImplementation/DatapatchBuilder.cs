﻿/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace JoinLineCommandImplementation
{
    public static class DatapatchBuilder
    {
        private static readonly string wrapperHeader =
@"-- SSDT-CD Datapatch Wrapper v1.0
-- ---------------------------------------------
-- | The content was changed by a tool         |
-- ---------------------------------------------
";
        public static void WrapScriptAsDatapatch(ITextView textView, IEditorOperations editorOperations)
        {
            var matchCount = 20;
            var header = textView.TextViewLines.FormattedSpan.Snapshot.GetText(0, matchCount);

            if (wrapperHeader.Substring(0, matchCount) == header)
            {
                return;
            }

            editorOperations.MoveToStartOfDocument(extendSelection: false);
            editorOperations.InsertNewLine();
            editorOperations.ReplaceAllMatches("'", "''", true, true, false);

            editorOperations.MoveToStartOfDocument(extendSelection: false);
            textView.TextBuffer.Insert(0, wrapperHeader);

            editorOperations.MoveToEndOfLine(extendSelection: false);
            editorOperations.InsertNewLine();
            editorOperations.InsertText("EXEC sp_execute_script @sql ='");
            editorOperations.InsertNewLine();

            editorOperations.DeleteHorizontalWhiteSpace();
            editorOperations.MoveToEndOfDocument(extendSelection: false);
            editorOperations.InsertNewLine();
            editorOperations.InsertText($"', @author = '{System.Environment.UserName}'");
            editorOperations.InsertNewLine();

            var selectedSpan = textView.Selection.SelectedSpans[0];
            textView.TextBuffer.Replace(selectedSpan, selectedSpan.GetText().Replace("\r\n", " "));

            editorOperations.MoveToEndOfLine(extendSelection: false);
            editorOperations.Delete();
            editorOperations.DeleteHorizontalWhiteSpace();
        }
    }
}