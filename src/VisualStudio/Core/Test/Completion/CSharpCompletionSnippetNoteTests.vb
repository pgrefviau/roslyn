' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Completion.Providers
Imports Microsoft.CodeAnalysis.Editor.UnitTests.IntelliSense
Imports Microsoft.CodeAnalysis.Snippets
Imports Microsoft.CodeAnalysis.Text
Imports Roslyn.Test.Utilities

Namespace Microsoft.VisualStudio.LanguageServices.UnitTests.Completion
    Public Class CSharpCompletionSnippetNoteTests
        Private markup As XElement = <document>
                                         <![CDATA[using System;
class C
{
    $$

    void M() { }
}]]></document>

        <WorkItem(726497)>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Sub SnippetExpansionNoteAddedToDescription_ExactMatch()
            Using state = CreateCSharpSnippetExpansionNoteTestState(markup, "interface")
                state.SendTypeChars("interfac")
                state.AssertCompletionSession()
                state.AssertSelectedCompletionItem(description:="title" & vbCrLf &
                    "description" & vbCrLf &
                    String.Format(FeaturesResources.NoteTabTwiceToInsertTheSnippet, "interface"))
            End Using
        End Sub

        <WorkItem(726497)>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Sub SnippetExpansionNoteAddedToDescription_DifferentSnippetShortcutCasing()
            Using state = CreateCSharpSnippetExpansionNoteTestState(markup, "intErfaCE")
                state.SendTypeChars("interfac")
                state.AssertCompletionSession()
                state.AssertSelectedCompletionItem(description:=$"{String.Format(FeaturesResources.Keyword, "interface")}
{String.Format(FeaturesResources.NoteTabTwiceToInsertTheSnippet, "interface")}")
            End Using
        End Sub

        <WorkItem(726497)>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Sub SnippetExpansionNoteNotAddedToDescription_ShortcutIsProperSubstringOfInsertedText()
            Using state = CreateCSharpSnippetExpansionNoteTestState(markup, "interfac")
                state.SendTypeChars("interfac")
                state.AssertCompletionSession()
                state.AssertSelectedCompletionItem(description:="title" & vbCrLf &
                    "description" & vbCrLf &
                    String.Format(FeaturesResources.NoteTabTwiceToInsertTheSnippet, "interfac"))
            End Using
        End Sub

        <WorkItem(726497)>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Sub SnippetExpansionNoteNotAddedToDescription_ShortcutIsProperSuperstringOfInsertedText()
            Using state = CreateCSharpSnippetExpansionNoteTestState(markup, "interfaces")
                state.SendTypeChars("interfac")
                state.AssertCompletionSession()
                state.AssertSelectedCompletionItem(description:=String.Format(FeaturesResources.Keyword, "interface"))
            End Using
        End Sub

        <WorkItem(726497)>
        <Fact, Trait(Traits.Feature, Traits.Features.Completion)>
        Public Sub SnippetExpansionNoteAddedToDescription_DisplayTextDoesNotMatchShortcutButInsertionTextDoes()
            Using state = CreateCSharpSnippetExpansionNoteTestState(markup, "InsertionText")

                state.SendTypeChars("DisplayTex")
                state.AssertCompletionSession()
                state.AssertSelectedCompletionItem(description:=String.Format(FeaturesResources.NoteTabTwiceToInsertTheSnippet, "InsertionText"))
            End Using
        End Sub

        Private Function CreateCSharpSnippetExpansionNoteTestState(xElement As XElement, ParamArray snippetShortcuts As String()) As TestState
            Dim state = TestState.CreateCSharpTestState(
                xElement,
                New ICompletionProvider() {New MockCompletionProvider(New TextSpan(31, 10))},
                Nothing,
                New List(Of Type) From {GetType(TestCSharpSnippetInfoService)})

            Dim testSnippetInfoService = DirectCast(state.Workspace.Services.GetLanguageServices(LanguageNames.CSharp).GetService(Of ISnippetInfoService)(), TestCSharpSnippetInfoService)
            testSnippetInfoService.SetSnippetShortcuts(snippetShortcuts)

            Return state
        End Function
    End Class
End Namespace
