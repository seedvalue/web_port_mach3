using System;
using System.Text;
using UnityEngine;

public static class MailHelper
{
	public static void SendMail(string recipient, string subject, string body)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("mailto:");
		stringBuilder.Append(recipient);
		stringBuilder.Append("?subject=");
		stringBuilder.Append(Uri.EscapeDataString(subject));
		stringBuilder.Append("&body=");
		stringBuilder.Append(Uri.EscapeDataString(body));
		Application.OpenURL(stringBuilder.ToString());
	}
}
