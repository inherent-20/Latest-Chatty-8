﻿using Latest_Chatty_8.DataModel;
using Latest_Chatty_8.Shared;
using Latest_Chatty_8.Shared.Networking;
using Latest_Chatty_8.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Latest_Chatty_8.Views
{
	[DataContract]
	public class ReplyNavParameter
	{
		[DataMember]
		public Comment Comment { get; private set; }
		[DataMember]
		public CommentThread CommentThread { get; private set; }

		public ReplyNavParameter(Comment c, CommentThread commentThread)
		{
			this.Comment = c;
			this.CommentThread = commentThread;
		}
	}
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class ReplyToCommentView : Latest_Chatty_8.Shared.LayoutAwarePage
	{
		#region Private Variables
		private ReplyNavParameter navParam;
		private bool ctrlPressed = false;

		#endregion

		#region Constructor
		public ReplyToCommentView()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Events

		async private void SendButtonClicked(object sender, RoutedEventArgs e)
		{
			await this.SendReply();
		}

		async private void AttachClicked(object sender, RoutedEventArgs e)
		{
			this.progress.IsIndeterminate = true;
			this.progress.Visibility = Windows.UI.Xaml.Visibility.Visible;
			this.postButton.IsEnabled = false;
			this.attachButton.IsEnabled = false;

			try
			{
				var photoUrl = await ChattyPics.UploadPhotoUsingPicker();
				await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
				{
					this.replyText.Text += photoUrl;
				});
			}
			finally
			{
				this.progress.IsIndeterminate = false;
				this.progress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				this.postButton.IsEnabled = true;
				this.attachButton.IsEnabled = true;
			}
		}
		#endregion

		#region Overrides
		async protected override Task<bool> CorePageKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
		{
			await base.CorePageKeyActivated(sender, args);
			//If it's not a key down event, we don't care about it.
			if (args.EventType != CoreAcceleratorKeyEventType.SystemKeyDown &&
				 args.EventType != CoreAcceleratorKeyEventType.KeyDown)
			{
				return true;
			}

			var ctrlDown = (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
			switch (args.VirtualKey)
			{
				case Windows.System.VirtualKey.Enter:
					if (ctrlPressed)
					{
						await this.SendReply();
					}
					break;
				default:
					break;
			}
			return true;
		}
		#endregion

		#region Load and Save State
		protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
			if (pageState != null)
			{
				//Load from saved state
				this.replyText.Text = pageState["ReplyText"] as string;
				this.navParam = pageState["NavParam"] as ReplyNavParameter;
			}
			else
			{
				//Loading fresh.
				this.navParam = navigationParameter as ReplyNavParameter;
			}

			if (this.navParam != null)
			{
				this.DefaultViewModel["ReplyToComment"] = this.navParam.Comment;
			}
			else
			{
				//Making a root post.  We don't need this.
				this.commentBrowser.Visibility = Visibility.Collapsed;
				this.replyGrid.RowDefinitions.RemoveAt(0);
			}
		}

		protected override void SaveState(Dictionary<String, Object> pageState)
		{
			pageState.Add("ReplyText", this.replyText.Text);
			pageState.Add("NavParam", this.navParam);
		}
		#endregion

		#region Private Helpers
		async private Task SendReply()
		{
			var button = postButton;

			try
			{
				this.bottomBar.Focus(Windows.UI.Xaml.FocusState.Programmatic);
				button.IsEnabled = false;

				this.progress.IsIndeterminate = true;
				this.progress.Visibility = Windows.UI.Xaml.Visibility.Visible;

				if (null == this.navParam)
				{
					await ChattyHelper.PostRootComment(this.replyText.Text);
				}
				else
				{
					await this.navParam.Comment.ReplyToComment(this.replyText.Text);
				}

				if (LatestChattySettings.Instance.AutoPinOnReply)
				{
					//Add the post to pinned in the background.
					var res = CoreServices.Instance.PinThread(this.navParam.CommentThread.Id);
				}

				CoreServices.Instance.PostedAComment = true;
				this.Frame.GoBack();
				return;
			}
			catch
			{
			}
			finally
			{
				this.progress.IsIndeterminate = false;
				this.progress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				button.IsEnabled = true;
			}

			var d = new MessageDialog("There was an error posting.  Please try again.", "Uh oh!");
			await d.ShowAsync();
		}
		#endregion
	}
}
