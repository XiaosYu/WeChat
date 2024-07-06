using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeChat.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FriendController : ControllerBaseEx
    {
        public FriendController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> ListFriends(string uid)
        {
            try
            {
                Log(new { uid }, uid);

                var result = DBContext.TbRelation.Where(s => s.RelationFrom == uid)
                                .ToList()
                                .Select(s => DBContext.TbUser.Find(s.RelationTo))
                                .ToList();

                object obj = new
                {
                    code = 200,
                    count = result.Count,
                    data = result
                };

                return Json(obj);
            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchFriend(string uid, string key)
        {
            try
            {

                Log(new { uid, key }, uid);

                var result = DBContext.TbRelation
                                .Where(s => s.RelationFrom == uid)
                                .Select(s => s.RelationTo)
                                .ToList()
                                .Select(s => DBContext.TbUser.Find(s))
                                .Where(s => s.Name.Contains(key) || s.Phone.Contains(key) || s.Quid.Contains(key))
                                .ToList();

                object obj = new
                {
                    code = 200,
                    count = result.Count,
                    data = result
                };

                return Json(obj);


            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchUser(string uid,string key)
        {
            try
            {
                Log(new { uid, key }, uid);

                var result = DBContext.TbUser.Where(s=>s.Email.Contains(key) || s.Name.Contains(key) || s.Quid.Contains(key) || s.Phone == key);

                object obj = new
                {
                    code = 200,
                    count = result.Count(),
                    data = result
                };

                return Json(obj);

            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRelation(string uid,string point)
        {
            try
            {

                Log(new {uid,point}, uid);

                //查找是否已经存在关系
                var result = DBContext.TbRelation.Where(s => s.RelationFrom == uid && s.RelationTo == point).FirstOrDefault();
                CheckNotNull(result, "二者已经是好友了");


                var relation = new TbRelation()
                {
                    RelationFrom = uid,
                    RelationTo = point
                };

                var re = new TbRelation()
                {
                    RelationFrom = point,
                    RelationTo = uid
                };

                DBContext.TbRelation.Add(relation);
                DBContext.TbRelation.Add(re);
                DBContext.SaveChanges();

                //发送成为朋友的消息
                BaseModel<AddBody> model = new();
                model.Action = "Add";
                model.Body = new AddBody();
                model.Guid = Random(10);
                model.Sender = uid;
                model.Receiever = point;
                model.Verfiy = "swu";
                var data = await SocketController.SendMessage(model, point);

                

                return Json(new { code = 200, msg = "添加成功" });


            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRelation(string uid, string point)
        {
            try
            {
                Log(new {uid,point}, uid);

                var result = DBContext.TbRelation.Where(s => uid == s.RelationFrom && s.RelationTo == point).FirstOrDefault();
                CheckNull(result, "二者还不是好友");

                var re = DBContext.TbRelation.Where(s => s.RelationTo == uid || s.RelationFrom == point).FirstOrDefault();
                if (re != null)
                {
                    DBContext.Remove(re);
                }
                DBContext.Remove(result);
                DBContext.SaveChanges();

                return Json(new { code = 200, msg = "删除成功" });
            }catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckRelation(string uid,string point)
        {
            try
            {
                Log(new { uid, point }, uid);

                var result = DBContext.TbRelation.Where(s => s.RelationFrom == uid && s.RelationTo == point).FirstOrDefault();
                CheckNull(result, "二者不是朋友");

                return Json(new { code = 200, msg = "二者是朋友" });
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
