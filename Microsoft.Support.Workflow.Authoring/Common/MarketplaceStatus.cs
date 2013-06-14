namespace Microsoft.Support.Workflow.Authoring.Common
{

    /// <summary>
    /// The status that a StoreActivity or ActivityLibrary can be in the marketplace.
    /// </summary>
    public enum MarketplaceStatus
    {
          /// <summary>
          /// The user has saved the workflow to their private store.
          /// </summary>
        Private,

        /// <summary>
        /// The workflow is public in the marketplace.
        /// </summary>
        Public,

        /// <summary>
        /// The workflow has reached EOL/is retired and no longer useable in the Marketplace.
        /// </summary>
        Retired,
    }
}
