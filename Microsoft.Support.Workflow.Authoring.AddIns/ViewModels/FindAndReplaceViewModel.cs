using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.ViewModels {
    public class FindAndReplaceViewModel : NotificationObject {
        private List<TextRange> matches;
        private string lastSearchText;

        private FlowDocument document;
        private bool isReadOnly;
        private int currentMatchIndex;
        private bool canNext;
        private bool canPrev;
        private string searchText;
        private string replacementText;
        private bool isReplacementMode;

        public FlowDocument Document {
            get { return document; }
            set {
                document = value;
                RaisePropertyChanged(() => Document);

                ResetSearchResult();
            }
        }

        public bool IsReadOnly {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                RaisePropertyChanged(() => IsReadOnly);
            }
        }

        public int CurrentMatchIndex {
            get { return currentMatchIndex; }
            set {
                currentMatchIndex = value;
                RaisePropertyChanged(() => CurrentMatchIndex);

                ReplaceCommand.RaiseCanExecuteChanged();

                CanPrev = (currentMatchIndex > 0);
                CanNext = (lastSearchText == null) ||
                    ((currentMatchIndex >= 0) && (currentMatchIndex < matches.Count - 1));
            }
        }

        public bool CanNext {
            get { return canNext; }
            set {
                canNext = value;
                RaisePropertyChanged(() => CanNext);
            }
        }

        public bool CanPrev {
            get { return canPrev; }
            set {
                canPrev = value;
                RaisePropertyChanged(() => CanPrev);
            }
        }

        public string SearchText {
            get { return searchText; }
            set {
                searchText = value;
                RaisePropertyChanged(() => SearchText);

                ReplaceCommand.RaiseCanExecuteChanged();
            }
        }

        public string ReplacementText {
            get { return replacementText ?? string.Empty; }
            set {
                replacementText = value;
                RaisePropertyChanged(() => ReplacementText);
            }
        }

        public bool IsReplacementMode {
            get { return isReplacementMode; }
            set {
                isReplacementMode = value;
                RaisePropertyChanged(() => IsReplacementMode);

                ReplaceCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PrevCommand { get; private set; }
        public DelegateCommand ReplaceCommand { get; private set; }
        public DelegateCommand ReplaceAllCommand { get; private set; }

        public FindAndReplaceViewModel() {
            NextCommand = new DelegateCommand(BringToNextMatch);
            PrevCommand = new DelegateCommand(BringToPrevMatch);
            ReplaceCommand = new DelegateCommand(Replace);
            ReplaceAllCommand = new DelegateCommand(ReplaceAll);
        }

        public void Search() {
            if (string.IsNullOrEmpty(SearchText)) {
                lastSearchText = null;
                ResetSearchResult();
            }
            else {
                if (lastSearchText == SearchText) {
                    if (matches != null && matches.Any())
                        BringToNextMatch();
                }
                else {
                    lastSearchText = SearchText;
                    FlowDocumentExtension.ClearTextDecorations(Document);
                    FindMatches();
                    if (matches.Any()) {
                        CurrentMatchIndex = 0;
                        HighlightCurrentMatch();
                    }
                    else {
                        ResetSearchResult();
                        AddInMessageBoxService.NoMoreOccurrences();
                    }
                }
            }
        }

        private void FindMatches() {
            matches = new List<TextRange>();
            TextRange range = FlowDocumentExtension.GetNextMatching(Document, SearchText, null);
            if (range != null) {
                matches.Add(range);
                range = FlowDocumentExtension.GetNextMatching(Document, SearchText, range.End);
                if (range != null)
                    matches.Add(range);
            }
        }

        public void ResetSearchResult() {
            matches = null;
            lastSearchText = null;
            CurrentMatchIndex = -1;

            FlowDocumentExtension.ClearTextDecorations(Document);
        }

        private void BringToNextMatch() {
            if (CanNext) {
                if (matches == null || lastSearchText != SearchText) {
                    Search();
                }
                else {
                    TextRange range = FlowDocumentExtension.GetNextMatching(Document, SearchText, matches.Last().End);
                    if (range != null)
                        matches.Add(range);

                    CurrentMatchIndex++;
                    FlowDocumentExtension.ClearTextDecorations(Document);
                    HighlightCurrentMatch();
                }
            }
        }

        private void BringToPrevMatch() {
            if (CanPrev) {
                CurrentMatchIndex--;
                FlowDocumentExtension.ClearTextDecorations(Document);
                HighlightCurrentMatch();
            }
        }
        
        private void Replace() {
            if (IsReplacementMode && !string.IsNullOrEmpty(SearchText)) {
                if (CurrentMatchIndex < 0 || lastSearchText != SearchText)
                    Search();

                if (matches == null || !matches.Any()) {
                    AddInMessageBoxService.NoMoreOccurrences();
                    return;
                }

                string xaml = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
                string pattern = Regex.Escape(SearchText);

                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection mc = regex.Matches(xaml);
                int startIndex = mc[CurrentMatchIndex].Index;

                xaml = regex.Replace(xaml, ReplacementText, 1, startIndex);
                Document.Blocks.Clear();
                Document.Blocks.Add(new Paragraph(new Run(xaml)));

                FindMatches();
                if (matches.Any()) {
                    for (int i = 0; i < CurrentMatchIndex; i++) {
                        TextRange range = FlowDocumentExtension.GetNextMatching(Document, SearchText, matches.Last().End);
                        if (range != null)
                            matches.Add(range);
                    }
                    CurrentMatchIndex = Math.Min(CurrentMatchIndex, matches.Count - 1);
                    HighlightCurrentMatch();
                }
                else {
                    CurrentMatchIndex = -1;
                }
            }
        }

        private void ReplaceAll() {
            if (IsReplacementMode && !string.IsNullOrEmpty(SearchText)) {
                string xaml = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
                string newXaml = Regex.Replace(xaml, Regex.Escape(SearchText), ReplacementText, RegexOptions.IgnoreCase);
                lastSearchText = SearchText;
                if (newXaml == xaml) {
                    AddInMessageBoxService.NoMoreOccurrences();
                }
                else {
                    Document.Blocks.Clear();
                    Document.Blocks.Add(new Paragraph(new Run(newXaml)));
                    ResetSearchResult();
                }
            }
        }

        private void HighlightCurrentMatch() {
            if (matches != null && matches.Any())
                FlowDocumentExtension.HighlightText(Document, matches[CurrentMatchIndex]);
        }
    }
}
