﻿using Latest_Chatty_8.DataModel;
using Latest_Chatty_8.Shared.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Latest_Chatty_8.Shared
{
	public static class ChattyHelper
	{
		async public static Task<bool> ReplyToComment(this Comment commentToReplyTo, string content)
		{
			return await ChattyHelper.PostComment(content, commentToReplyTo.Id.ToString());
		}

		async public static Task<bool> PostRootComment(string content)
		{
			return await ChattyHelper.PostComment(content);
		}

		async private static Task<bool> PostComment(string content, string parentId = null)
		{
			if (content.Length <= 5)
			{
				var dlg = new Windows.UI.Popups.MessageDialog("Post something longer.");
				await dlg.ShowAsync();
				return false;
			}

			var dataToSend = string.Format("text={0}&parentId={1}", Uri.EscapeDataString(content), parentId != null ? parentId : "0");

			//:TODO: Handle failures better.
			var response = await POSTHelper.Send(Locations.PostUrl, dataToSend, true);

			return true;
		}
	}
}
