namespace WeChat.WebApi.Servers.SMS
{
    public class SMessage
    {
		private MmsHttpClient _Client = new MmsHttpClient();
		private string _url_send;
		private string _url_surperplus;
		private string _uid;
		private string _account;
		private string _pwd;
		public SMessage()
		{
			_url_send = "http://47.92.128.19:8088/v2sms.aspx";
			_url_surperplus = "http://47.92.128.19:8088/sms.aspx";
			_uid = "2957";
			_account = "lll147258369";
			_pwd = "021228";
		}
		public string SendMessage(string msg, string sendtime, params string[] phone)
		{
			string time = DateTime.Now.ToString("yyyyMMddHHmmss");
			string content = msg;
			string sign = ToMd5(_account + _pwd + time);

			string phones = string.Empty;
			foreach (string s in phone)
			{
				phones += s;
				phones += ",";
			}
			phones = phones.Remove(phones.Length - 1, 1);

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("userid={0}", _uid);
			sb.AppendFormat("&account={0}", _account);
			sb.AppendFormat("&password={0}", _pwd);
			sb.AppendFormat("&timestamp={0}", time);
			sb.AppendFormat("&mobile={0}", phones);
			sb.AppendFormat("&content={0}", content);
			sb.AppendFormat("&sendTime={0}", sendtime);
			sb.AppendFormat("&action={0}", "send");
			sb.AppendFormat("&sign={0}", sign);
			sb.AppendFormat("&extno={0}", "");

			if (true)
			{
				sb.AppendFormat("&rt={0}", "json");
			}
			string res = _Client.Request(_url_send, MmsHttpClient.Method.POST, sb.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded;charset=utf-8");
			return res;
		}
		public JsonOverage GetSurplus()
		{
			JsonOverage jo = new JsonOverage();
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("userid={0}", _uid);
			sb.AppendFormat("&account={0}", _account);
			sb.AppendFormat("&password={0}", _pwd);
			sb.AppendFormat("&action={0}", "overage");

			if (true)
			{
				sb.AppendFormat("&rt={0}", "json");
			}
			string res = _Client.Request(_url_surperplus, MmsHttpClient.Method.POST, sb.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded;charset=utf-8");
			jo = JsonConvert.DeserializeObject<JsonOverage>(res);
			return jo;
		}

		public JsonMessage TurntoObject(string mjs)
		{
			JsonMessage xm = new JsonMessage();
			xm = JsonConvert.DeserializeObject<JsonMessage>(mjs);
			return xm;
		}
		private string ToMd5(string src)
		{
			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				byte[] srcBytes = Encoding.UTF8.GetBytes(src);
				byte[] desBytes = md5.ComputeHash(srcBytes);
				return BitConverter.ToString(desBytes).Replace("-", "").ToLower();
			}
		}
		public string ToText(string sign, string text)
		{
			return string.Format("【{0}】{1}", sign, text);
		}
	}
}
