using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeChat.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OnlineController : ControllerBaseEx
    {
        public OnlineController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> IsOnline(string uid,string point)
        {
            try
            {
                Log(new { uid, point }, uid);

                if(SocketController.Users.Keys.Contains(point))
                {
                    return Json(new { code = 200, msg = "该用户在线" });
                }
                else
                {
                    throw new Exception("该用户离线");
                }


            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnlineUser(string uid)
        {
            try
            {
                Log(new { uid }, uid);

                var ids = SocketController.Users.Keys
                            .ToList()
                            .Select(s => DBContext.TbUser.Find(s))
                            .ToList();

                object obj = new
                {
                    code = 200,
                    count = ids.Count,
                    data = ids
                };
                return Json(obj);


            }catch(Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
