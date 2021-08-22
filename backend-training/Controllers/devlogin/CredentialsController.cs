using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using backend_training.Models;
using System.Web;
namespace backend_training.Controllers.devlogin
{
    [RoutePrefix("api/dev-login")]
    public class CredentialsController : ApiController
    {
        private mydatabaseEntities db = new mydatabaseEntities();
        APISecurity secure = new APISecurity();
        // GET: api/Credentials
        public IQueryable<tbuser> Gettbusers()
        {
            return db.tbusers;
        }
        class LoginResponse
        {
            public string message { get; set; }
            public object databulk { get; set; }
            public string email { get; set; }
        }
        LoginResponse loginresponse = new LoginResponse();
        // GET: api/Credentials/5
        [Route("login"), HttpPost]
        public IHttpActionResult Gettbuser()
        {
            try
            {
                var http = HttpContext.Current.Request;
                using (db)
                {
                    loginresponse.email = http.Form["email"];
                    string pwd = secure.Encrypt(http.Form["password"]);
                    string encrypted = string.Empty;
                    string istype;
                    string isstatus;
                    var c1 = db.tbusers.Any(x => x.email == loginresponse.email && x.isverified == "1");
                    var c2 = db.tbusers.Where(x => x.email == loginresponse.email).FirstOrDefault();
                    if(string.IsNullOrEmpty(loginresponse.email) || string.IsNullOrEmpty(pwd))
                    {
                        return Ok("empty fields");
                    }
                    else
                    {
                        if (c1)
                        {
                            encrypted = c2 == null ? "" : c2.password;
                            istype = c2.istype;
                            isstatus = c2.isstatus;
                            string decryptoriginal = secure.Decrypt(pwd);
                            string decryptrequest = secure.Decrypt(encrypted);
                            if(decryptrequest == decryptoriginal)
                            {
                                if(isstatus == "1")
                                {
                                    if(istype == "1")
                                    {
                                        //developer
                                        var devfetch = db.tbusers.Where(x => x.email == loginresponse.email)
                                            .Select(t => new
                                            {
                                                t.userID,
                                                t.firstname,
                                                t.lastname,
                                                t.istype
                                            }).ToList();
                                        loginresponse.databulk = devfetch.FirstOrDefault();
                                        loginresponse.message = "SUCCESS";
                                        return Ok(loginresponse);
                                    }
                                    return Ok("Non developer");
                                }
                                else
                                {
                                    return Ok("account disabled");
                                }
                            }
                            else
                            {
                                //wrong password
                                return Ok("Invalid password");
                            }
                        }
                        else
                        {
                            //this account not found boy
                            return Ok("No account found");
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        class token_validator_access
        {
            public string email { get; set; }
            public string token { get; set; }
        }
        // PUT: api/Credentials/5
        [Route("update-token"), HttpPut]
        public IHttpActionResult Puttbuser(string email, string token)
        {
            try
            {
                using (db)
                {
                    var check = db.tbusers.Where(x => x.email == email).FirstOrDefault();
                    if(check != null)
                    {
                        check.istoken = token;
                        db.SaveChanges();
                        return Ok("token success");
                    }
                    return Ok();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
       
        [Route("toke-scan"), HttpGet]
        public IHttpActionResult tokenscan(string email , string token)
        {
            try
            {
                using (db)
                {
                    var scan = db.tbusers.Any(x => x.email == email && x.istoken == token);
                    if (scan)
                    {
                        return Ok("valid");
                    }
                    else
                    {
                        return Ok("not valid");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: api/Credentials
        [ResponseType(typeof(tbuser))]
        public IHttpActionResult Posttbuser(tbuser tbuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tbusers.Add(tbuser);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tbuser.userID }, tbuser);
        }

        // DELETE: api/Credentials/5
        [ResponseType(typeof(tbuser))]
        public IHttpActionResult Deletetbuser(int id)
        {
            tbuser tbuser = db.tbusers.Find(id);
            if (tbuser == null)
            {
                return NotFound();
            }

            db.tbusers.Remove(tbuser);
            db.SaveChanges();

            return Ok(tbuser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tbuserExists(int id)
        {
            return db.tbusers.Count(e => e.userID == id) > 0;
        }
    }
}