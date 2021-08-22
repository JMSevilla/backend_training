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
    [RoutePrefix("api/adding")]
    public class ReactBackendController : ApiController
    {
        private mydatabaseEntities db = new mydatabaseEntities();
        class Response
        {
            public string message { get; set; }
        }
        [Route("get-data"), HttpGet]
        public IHttpActionResult getdata()
        {
            try
            {
                using (db)
                {
                    var obj = db.tbinformations.ToList();
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("remove-data"), HttpDelete]
        public IHttpActionResult removedata(int id)
        {
            if(id <= 0)
            {
                return Ok("invalid id");
            }
            else
            {
                using (db)
                {
                    var remover = db.tbinformations.Where(x => x.id == id).FirstOrDefault();
                    db.Entry(remover).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return Ok("success");
                }
            }
        }

        [Route("update-data"), HttpPut]
        public IHttpActionResult updatedata()
        {
            try
            {
                var http = HttpContext.Current.Request;
                string firstname = http.Form["firstname"];
                int id = Convert.ToInt32(http.Form["id"]);
                string lastname = http.Form["lastname"];
                if(string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname))
                {
                    return Ok("empty");
                }
                else if(id <= 0)
                {
                    return Ok("invalid id");
                }
                else
                {
                    using (db)
                    {
                        var checker = db.tbinformations.Where(x => x.id == id).FirstOrDefault();
                        if(checker != null)
                        {
                            checker.firstname = firstname;
                            checker.lastname = lastname;
                            db.SaveChanges();
                            return Ok("success");
                        }
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        Response resp = new Response();
        [Route("add-form"), HttpPost]
        public IHttpActionResult add()
        {
            try
            {
                var http = HttpContext.Current.Request;
                string firstname = http.Form["firstname"];
                string lastname = http.Form["lastname"];
                //validation
                if(string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname))
                {
                    resp.message = "empty";
                    return Ok(resp);
                } 
                else
                {
                    using (db)
                    {
                        tbinformation info = new tbinformation();
                        info.firstname = firstname;
                        info.lastname = lastname;
                        info.createdAt = Convert.ToDateTime(System.DateTime.Now.ToString
                            ("yyyy/MM/dd"));
                        db.tbinformations.Add(info);
                        db.SaveChanges();
                        resp.message = "SUCCESS";
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
