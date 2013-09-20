
namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.DirectoryServices;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Practices.Prism.ViewModel;
    using System.Runtime.InteropServices;

    public class UserInfoPaneViewModel : NotificationObject
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserInfoPaneViewModel()
        {
            GetUserNameAndPicture();
        }

        private string userName;
        private BitmapImage userImage;
        private const string defaultActiveDirectoryPath = "GC://dc=domain,dc=corp,dc=microsoft,dc=com";
        private const string defaultDomainPrefix = "domain";
        private const string defaultThumbnailPrefix = "thumbnailPhoto";
        private static readonly string defaultUserFilter = "(&(objectClass=user)(anr=" + Environment.UserName + "))";
        
        /// <summary>
        /// Name of the current user
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                base.RaisePropertyChanged(() => UserName);
            }
        }

        /// <summary>
        /// Display picture of the current user
        /// </summary>
        public BitmapImage UserImage
        {
            get { return userImage; }
            set
            {
                userImage = value;
                RaisePropertyChanged(()=>UserImage);
            }
        }

        /// <summary>
        /// Gets the name of the current user and the corresponding display pic.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122", Justification = "Method is private and has been reviewed. Users "
            + "will not be able to use this method to call IsValidIdentifier().")]
       private void GetUserNameAndPicture()
        {
            userName = Environment.UserDomainName + @"\" + Environment.UserName;

            string directoryPath = defaultActiveDirectoryPath.Replace(defaultDomainPrefix, Environment.UserDomainName);

            using (var dsSearcher = new DirectorySearcher(directoryPath))
            {

                dsSearcher.Filter = defaultUserFilter;

                try
                {
                    SearchResult result = dsSearcher.FindOne();

                    using (var user = new DirectoryEntry(result.Path))
                    {
                        var data = user.Properties[defaultThumbnailPrefix].Value as byte[];
                        
                        if (data != null)
                        {
                            using (var stream = new MemoryStream(data))
                            {
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = stream;
                                image.EndInit();
                                image.Freeze();
                                UserImage = image;
                            }
                        }
                    }
                }
                catch (COMException)
                {
                    //User picture couldn't be retrieved
                    //It will be loaded from a static resource
                }
            }
        }
    }
}

