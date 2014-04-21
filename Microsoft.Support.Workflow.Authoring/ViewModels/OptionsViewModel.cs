using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Views;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.ViewModels {
    public class OptionsViewModel : ViewModelBase {
        private static IOptionPage[] pages {
            get {
                return new IOptionPage[] {
                    new DefaultValueSettingsView(), 
                    new CDSIntegrationView()
                };
            }
        }

        private IOptionPage[] optionPages;
        public IOptionPage[] OptionPages {
            get { return optionPages; }
            set {
                optionPages = value;
                RaisePropertyChanged(() => OptionPages);
            }
        }

        private IOptionPage selectedOptionPage;
        public IOptionPage SelectedOptionPage {
            get { return selectedOptionPage; }
            set {
                selectedOptionPage = value;
                RaisePropertyChanged(() => SelectedOptionPage);
            }
        }

        public bool HasSaved {
            get { return OptionPages.All(p => p.HasSaved); }
        }

        public OptionsViewModel(string initPage = null) {
            OptionPages = pages;
            SelectedOptionPage = string.IsNullOrEmpty(initPage) ? OptionPages.First() :
                OptionPages.Single(p => p.Title == initPage);

            foreach (IOptionPage p in OptionPages) {
                p.HasSavedChanged += HasSavedChanged;
            }
        }

        public void Save() {
            Utility.DoTaskWithBusyCaption(TextResources.Saving, () => {
                foreach (IOptionPage pair in OptionPages.Where(p => !p.HasSaved)) {
                    try {
                        pair.Save();
                    }
                    catch (UserFacingException ex) {
                        MessageBoxService.Show(ex.Message, TextResources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex) {
                        MessageBoxService.ShowException(ex, TextResources.SaveSettingsFailureMsg);
                    }
                    if (!pair.HasSaved) {
                        SelectedOptionPage = pair;
                        break;
                    }
                }
                RaisePropertyChanged(() => HasSaved);
            });
        }

        public void Close() {
            foreach (IOptionPage p in OptionPages) {
                p.HasSavedChanged -= HasSavedChanged;
            }
        }

        private void HasSavedChanged(object sender, EventArgs e) {
            RaisePropertyChanged(() => HasSaved);
        }
    }
}
