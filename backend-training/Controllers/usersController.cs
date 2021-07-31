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
namespace backend_training.Controllers
{
    [RoutePrefix("api/register-user")]
    public class usersController : ApiController
    {
        private mydatabaseEntities db = new mydatabaseEntities();

        // GET: api/users
        public IQueryable<user> Getusers()
        {
            return db.users;
        }
        class GetLoginData
        { 
            public object getbundle { get; set; }
            public string status { get; set; }
        }
        GetLoginData logs = new GetLoginData();
        // GET: api/users/5
        [Route("login"), HttpGet]
        public IHttpActionResult Getuser(string email, string password)
        {
            try
            {
                using (db)
                {
                    string usersemail = email.ToString();
                    string userspassword = password.ToString();
                    var obj = db.users.Any(x => x.email == usersemail);
                    if (obj) // if true / if false
                    {
                        var check = db.users.Where(x => x.email == usersemail && x.password == userspassword).FirstOrDefault();
                        if (check.password == userspassword)
                        {
                            if(check.istype == "1")
                            {
                                var getalldata = db.users.Where(x => x.email == usersemail && x.istype == check.istype);
                                logs.getbundle = getalldata.Select(y => new
                                {
                                    y.firstname,
                                    y.lastname, y.istype
                                }).ToList();
                                logs.status = "SUCCESS";
                                return Ok(logs);
                            }
                            else if(check.istype == "0")
                            {
                                var getalldata = db.users.Where(x => x.email == usersemail && x.istype == check.istype);
                                logs.getbundle = getalldata.Select(y => new
                                {
                                    y.firstname,
                                    y.lastname,
                                    y.istype
                                }).ToList();
                                logs.status = "SUCCESS NO ADMIN";
                                return Ok(logs);
                            }
                            else
                            {
                                logs.status = "NO USERS";
                                return Ok(logs);
                            }
                        }
                        else
                        {
                            //wrong password
                            logs.status = "WRONG";
                            return Ok(logs);
                        }
                    }
                    else
                    {
                        logs.status = "NOT EXIST";
                        return Ok(logs);
                        //not exist
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putuser(int id, user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.clientid)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        class UsersClass {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public char istype { get; set; }
        }
        UsersClass usersclass = new UsersClass();
        // POST: api/users
        [Route("register"), HttpPost]
        public IHttpActionResult Postuser()
        {
            try
            {
                using (db)
                {
                    user userstb = new user();
                    var http = HttpContext.Current.Request;
                    usersclass.firstname = http.Form["firstname"];
                    usersclass.lastname = http.Form["lastname"];
                    usersclass.email = http.Form["email"];
                    usersclass.password = http.Form["password"];
                    usersclass.istype = Convert.ToChar(http.Form["istype"]);
                    if(string.IsNullOrEmpty(usersclass.email) || string.IsNullOrEmpty(usersclass.password))
                    {
                        return Ok("empty");
                    }
                    else
                    {
                        userstb.firstname = usersclass.firstname;
                        userstb.lastname = usersclass.lastname;
                        userstb.email = usersclass.email;
                        userstb.password = usersclass.password;
                        userstb.istype = Convert.ToString(usersclass.istype);
                        db.users.Add(userstb);
                        db.SaveChanges();
                        return Ok("success");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public IHttpActionResult Deleteuser(int id)
        {
            user user = db.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool userExists(int id)
        {
            return db.users.Count(e => e.clientid == id) > 0;
        }
    }
}