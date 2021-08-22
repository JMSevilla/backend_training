using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using backend_training.Models;
using System.Web;
namespace backend_training.Controllers
{
    [RoutePrefix("api/data-info")]
    public class ReactServerSideController : ApiController
    {
        private dbfsEntities db = new dbfsEntities();
        infoClass inf = new infoClass();
        class Response
        {
            public string response { get; set; }
        }
        Response resp = new Response();
        [Route("store"), HttpPost]
        public IHttpActionResult add()
        {
            try
            {
                var request_receiver = HttpContext.Current.Request;
                inf.firstname = request_receiver.Form["firstname"];
                inf.lastname = request_receiver.Form["lastname"];
                inf.createdAt = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd"));
                if(string.IsNullOrEmpty(inf.firstname) || string.IsNullOrEmpty(inf.lastname))
                {
                    resp.response = "empty";
                    return Ok(resp);
                }
                else
                {
                    using (db)
                    {
                        infotb info = new infotb();
                        info.firstname = inf.firstname;
                        info.lastname = inf.lastname;
                        info.createdAt = inf.createdAt;
                        db.infotbs.Add(info);
                        db.SaveChanges();
                        resp.response = "success";
                        return Ok(resp);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
