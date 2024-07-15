namespace WeChat.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBaseEx
    {
        public UserController()
        { 

        }
        static public List<VerifyCodeModel<IActionResult>> RegisterCodes = new();

        [HttpPost]
        public async Task<IActionResult> Login(string uid, string pwd)
        {
            try
            {
                Log(new { uid, pwd }, uid);

                if(uid.Length == 11)
                {
                    //说明是手机号登录
                    var sp = DBContext.TbUser.Where(s => s.Phone == uid).FirstOrDefault();
                    CheckNull(sp, "密码错误或账号不存在");

                    uid = sp.Quid;
                }
           
                //获取MD5
                var md5 = ComputeMD5(uid, pwd);

                //查找登录名
                var result = DBContext.TbAccount.Where(s => s.Hash == md5).FirstOrDefault();
                CheckNull(result, "密码错误或账号不存在");

                //查找用户值
                var user = DBContext.TbUser.Find(result.Quid);
                CheckNull(user, "内部数据库出错");

                //构造返回值
                object data = new
                {
                    code = 200,
                    uid = user.Quid,
                    name = user.Name,
                    sign = user.Sign,
                    phone = user.Phone,
                    email = user.Email
                };

                return Json(data);

            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string phone, string pwd, string name)
        {
            try
            {
                Log(new { phone, pwd, name }, phone);
                
                //判断号码是否合理
                if(!Regex.Match(phone, @"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$").Success)
                {
                    throw new Exception("手机号不存在或者不属于大陆号码");
                }

                //确定是否有已注册的账号
                var tmp = DBContext.TbUser.Where(s => s.Phone == phone).FirstOrDefault();
                CheckNotNull(tmp, "该手机号已经注册了,请换个号码试试吧");

                //确定是否已发送短信
                var sms = RegisterCodes.Where(s => s.Phone == phone && s.RegisterTime.AddSeconds(5) > DateTime.Now).FirstOrDefault();
                CheckNotNull(sms, "发送短信频繁,请在5分钟后重试");

                //构造返回model
                VerifyCodeModel<IActionResult> model = new();
                //var code = Random(6, "-1");
                var code = "000000";
                model.Code = code;
                model.Phone = phone;
                model.RegisterTime = DateTime.Now;
                model.CallBack = () =>
                {
                    var context = Factory.Create<DB_WeChatContext>();
                    TbAccount account = new();
                    account.Quid = Random(10,"-1");
                    account.Hash = ComputeMD5(account.Quid, pwd);
                    TbUser user = new();
                    user.Quid = account.Quid;
                    user.Name = name;
                    user.Phone = phone;
                    user.Email = " ";
                    user.Sign = "暂无签名";
                    context.TbAccount.Add(account);
                    context.TbUser.Add(user);
                    context.SaveChanges();
                    object data = new
                    {
                        code = 200,
                        uid = user.Quid,
                        name = user.Name,
                        phone = user.Phone
                    };
                    context.Dispose();
                    return Json(data);             
                };
                RegisterCodes.Add(model);

                //发送短信

                /* 这一段代码需要注册发信服务(这个是收费的!!!!)，现在固定下来仅用000000验证
                SMessage message = new();
                var data = message.TurntoObject(message.SendMessage($"【SpaceServer】验证码为:{code},在5分钟内有效", "", phone));
                if(data.returnstatus != "Success")
                {
                    throw new Exception("短信服务繁忙或错误,请联系管理员");
                }
                */

                return Json(new { code = 200, msg = "验证码已发送" });


            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reset(string phone,string newpwd)
        {
            try
            {

                Log(new { phone, newpwd }, phone);

                //查找uid是否存在
                var user = DBContext.TbUser.Where(s => s.Phone == phone).FirstOrDefault();
                CheckNull(user, "该用户不存在");

                //确定是否已发送短信
                var sms = RegisterCodes.Where(s => s.Phone == user.Phone && s.RegisterTime.AddSeconds(5) > DateTime.Now).FirstOrDefault();
                CheckNotNull(sms, "发送短信频繁,请在5分钟后重试");

                //构建model
                VerifyCodeModel<IActionResult> model = new();
                var code = Random(6, "-1");
                model.Phone = user.Phone;
                model.RegisterTime = DateTime.Now;
                model.Code = code;
                model.CallBack = () =>
                {
                    var context = Factory.Create<DB_WeChatContext>();
                    var account = context.TbAccount.Find(user.Quid);
                    CheckNull(account, "遇到未知错误,请重新注册项目");
                    account.Hash = ComputeMD5(user.Quid, newpwd);
                    context.SaveChanges();
                    return Json(new { code = 200, msg = "重置密码成功" });
                };
                RegisterCodes.Add(model);

                //发送短信
                SMessage message = new();
                var data = message.TurntoObject(message.SendMessage($"【SpaceServer】验证码为:{code},在5分钟内有效", "", user.Phone));
                if (data.returnstatus != "Success")
                {
                    throw new Exception("短信服务繁忙或错误,请联系管理员");
                }

                return Json(new { code = 200, msg = "验证码已发送" });
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Verfiy(string phone,string code)
        {
            try
            {
                Log(new { phone, code }, phone);

                //查找验证码集合
                var codes = RegisterCodes.Where(s => s.Phone == phone && s.Code == code && s.RegisterTime.AddMinutes(5) > DateTime.Now).FirstOrDefault();
                CheckNull(codes, "验证码不存在");

                //验证成功
                RegisterCodes.Remove(codes);

                return codes.CallBack();

                

            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Modify(string uid,string name = "",string sign = "")
        {
            try
            {
                Log(new {uid,name, sign}, uid);

                var result = DBContext.TbUser.Find(uid);
                CheckNull(result, "未找到用户名");

                result.Name = name == "" ? result.Name : name;
                result.Sign = sign == "" ? result.Sign : sign;

                DBContext.SaveChanges();

                return Json(new { code = 200, result = "修改成功" });

            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUserNames(string uid)
        {
            try
            {
                Log(new { uid }, uid);

                var result = DBContext.TbUser.ToList().Select(s => new { uid = s.Quid, name = s.Name }).ToList();

                return Json(new { code = 200, data = result });

            }catch(Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
