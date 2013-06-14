//-----------------------------------------------------------------------
// <copyright file="PublishingReply.cs" company="Microsoft">
// Copyright
// Publishing Reply
// </copyright>
//-----------------------------------------------------------------------
namespace CWF.DataContracts
{
	#region References
	using System;
	using System.Runtime.Serialization;
	using CWF.DataContracts;
	#endregion References    
	
	#region Publishing Reply
	
	/// <summary>
	/// Reply from publishing
	/// </summary>
	[DataContract]
	public class PublishingReply : ReplyHeader
	{
		/// <summary>
		/// Intiialized to 1.0.0.0
		/// </summary>
		private Version _publishedVersion = new Version();

		/// <summary>
		/// The location on the server where we published the files
		/// </summary>
		[DataMember]
		public string PublishedLocation { get; set; }

		/// <summary>
		/// The version that was published
		/// </summary>
		[DataMember]
		public string PublishedVersion
		{
			get
			{
                return _publishedVersion != null ? _publishedVersion.ToString() : null;
			}

			set
			{
				Version.TryParse(value, out _publishedVersion);
			}
		}

		/// <summary>
		/// Any publishing errors
		/// </summary>
		[DataMember]
		public string PublishErrors { get; set; }
			
	}

	#endregion Publishing Reply
}